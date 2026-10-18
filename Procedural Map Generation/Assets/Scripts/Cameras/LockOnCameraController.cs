using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(LockOnSystem))]
public class LockOnCameraController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineCamera gameplayCamera; //CinemachineFollowTayla
    [SerializeField] private int activePriority = 20;
    [SerializeField] private bool disableLegacyCameras = true; // desativa a bosta do LockCam

    [Header("Camera livre (aplicado igual em todas as cenas)")]
    [SerializeField] private bool applyFreeSettings = true;
    [SerializeField, Range(30f, 90f)] private float fieldOfView = 50f;
    [Tooltip("Distancia da camera ao player (OrbitStyle precisa estar em Sphere) !!!!!!!!!!!!!!! .")]
    [SerializeField] private float freeRadius = 6.5f;
    [SerializeField] private Vector3 freePositionDamping = new Vector3(0.25f, 0.5f, 0.25f); //Segundos para a camera alcancar o player
    [SerializeField] private Vector2 freeComposerDamping = new Vector2(0.2f, 0.35f); // Suavizacao do enquadramento do player na tela

    [Header("Lock-on")]
    [SerializeField] private float lockPitch = 16f; // grau da cam
    [SerializeField] private float shoulderYaw = 8f; // desvio lateral
    [SerializeField] private float lockRadiusBase = 6.5f;
    [SerializeField] private float lockRadiusPerMeter = 0.35f; // dist entre tayla e inimigo
    [SerializeField] private float lockRadiusMax = 11f;
    [SerializeField, Range(0f, 1f)] private float framingBias = 0.4f;
    [SerializeField] private Vector3 playerFocusOffset = new Vector3(0f, 1.4f, 0f);
    [SerializeField] private Vector3 enemyFocusOffset = new Vector3(0f, 1.2f, 0f);
    [SerializeField] private Vector2 lockComposerDamping = new Vector2(0.1f, 0.25f);

    [Header("Suavizacao do lock (maior = mais rapido)")]
    [SerializeField] private float yawSharpness = 7f;
    [SerializeField] private float pitchSharpness = 5f;
    [SerializeField] private float radiusSharpness = 4f;
    [SerializeField] private float framingSharpness = 8f;

    [Header("Sem alvo")]
    [Tooltip("Apertar lock sem inimigo por perto recentraliza a camera atras do player.")]
    [SerializeField] private bool recenterWhenNoTarget = true;
    [SerializeField] private float recenterDuration = 0.35f;
    [SerializeField] private float recenterSharpness = 12f;

    private LockOnSystem lockOnSystem;
    private CinemachineOrbitalFollow orbital;
    private CinemachineRotationComposer composer;
    private CinemachineInputAxisController inputAxis;
    private Transform framingPoint;

    private bool wasLocked;
    private bool recentering;
    private float recenterTimer;
    private float resolveRetryTimer;
    private bool warnedMissingCamera;

    private Transform Focus
    {
        get
        {
            if (gameplayCamera != null && gameplayCamera.Follow != null) return gameplayCamera.Follow;
            return lockOnSystem.PlayerRoot;
        }
    }

    private void Awake()
    {
        lockOnSystem = GetComponent<LockOnSystem>();
    }

    private void OnEnable()
    {
        lockOnSystem.OnLockFailed += HandleLockFailed;
    }

    private void OnDisable()
    {
        lockOnSystem.OnLockFailed -= HandleLockFailed;
    }

    private void Start()
    {
        TryResolveCamera();
    }

    private void OnDestroy()
    {
        if (framingPoint != null) Destroy(framingPoint.gameObject);
    }

    private bool TryResolveCamera()
    {
        if (orbital != null) return true;

        if (gameplayCamera == null)
        {
            CinemachineOrbitalFollow found = FindFirstObjectByType<CinemachineOrbitalFollow>();
            if (found != null) gameplayCamera = found.GetComponent<CinemachineCamera>();
        }

        if (gameplayCamera == null)
        {
            if (!warnedMissingCamera)
            {
                Debug.LogWarning("[LockOnCameraController] Nenhuma CinemachineCamera com CinemachineOrbitalFollow encontrada na cena.");
                warnedMissingCamera = true;
            }
            return false;
        }

        orbital = gameplayCamera.GetComponent<CinemachineOrbitalFollow>();
        composer = gameplayCamera.GetComponent<CinemachineRotationComposer>();
        inputAxis = gameplayCamera.GetComponent<CinemachineInputAxisController>();

        if (orbital == null)
        {
            Debug.LogWarning("[LockOnCameraController] A camera '" + gameplayCamera.name + "' precisa de CinemachineOrbitalFollow.");
            return false;
        }

        if (gameplayCamera.Follow == null) gameplayCamera.Follow = lockOnSystem.PlayerRoot;

        gameplayCamera.Priority = activePriority;
        if (disableLegacyCameras) DisableLegacyCameras();
        ApplyFreeSettings();

        GameObject fp = new GameObject("LockOn Framing Point");
        framingPoint = fp.transform;
        framingPoint.position = Focus.position + playerFocusOffset;

        return true;
    }

    private void DisableLegacyCameras()
    {
        CinemachineCamera[] all = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);
        for (int i = 0; i < all.Length; i++)
        {
            CinemachineCamera cam = all[i];
            if (cam == gameplayCamera) continue;
            if (cam.GetComponent<CinemachineThirdPersonFollow>() == null) continue;

            Debug.Log("[LockOnCameraController] Desativando camera antiga '" + cam.name + "' (ThirdPersonFollow).");
            cam.gameObject.SetActive(false);
        }
    }

    private void ApplyFreeSettings()
    {
        if (!applyFreeSettings) return;

        gameplayCamera.Lens.FieldOfView = fieldOfView;
        orbital.TrackerSettings.PositionDamping = freePositionDamping;
        orbital.Radius = freeRadius;
        if (composer != null) composer.Damping = freeComposerDamping;
    }

    private void Update()
    {
        if (orbital == null)
        {
            resolveRetryTimer -= Time.unscaledDeltaTime;
            if (resolveRetryTimer > 0f) return;
            resolveRetryTimer = 0.5f;
            if (!TryResolveCamera()) return;
        }

        float dt = Time.deltaTime;
        Transform target = lockOnSystem.CurrentTarget;
        bool locked = target != null;

        if (locked && !wasLocked) BeginLock();
        else if (!locked && wasLocked) EndLock();
        wasLocked = locked;

        if (locked) UpdateLocked(target, dt);
        else UpdateFree(dt);
    }

    private void BeginLock()
    {
        recentering = false;

        framingPoint.position = Focus.position + playerFocusOffset;
        gameplayCamera.Target.LookAtTarget = framingPoint;
        gameplayCamera.Target.CustomLookAtTarget = true;

        if (composer != null) composer.Damping = lockComposerDamping;
        if (inputAxis != null) inputAxis.enabled = false;
    }

    private void EndLock()
    {
        gameplayCamera.Target.CustomLookAtTarget = false;
        gameplayCamera.Target.LookAtTarget = null;

        if (composer != null && applyFreeSettings) composer.Damping = freeComposerDamping;
        if (inputAxis != null) inputAxis.enabled = true;
    }

    private void UpdateLocked(Transform target, float dt)
    {
        Vector3 playerPos = Focus.position;
        Vector3 enemyPos = target.position;
        Vector3 flat = enemyPos - playerPos;

        flat.y = 0f;
        float dist = flat.magnitude;

        if (dist > 0.01f)
        {
            float yaw = Mathf.Atan2(flat.x, flat.z) * Mathf.Rad2Deg + shoulderYaw;
            float current = orbital.HorizontalAxis.Value;
            float next = Mathf.LerpAngle(current, yaw, Damp(yawSharpness, dt));
            orbital.HorizontalAxis.Value = Mathf.DeltaAngle(0f, next);
        }

        orbital.VerticalAxis.Value = Mathf.Lerp(orbital.VerticalAxis.Value, lockPitch, Damp(pitchSharpness, dt));

        float wantedRadius = Mathf.Min(lockRadiusBase + dist * lockRadiusPerMeter, lockRadiusMax);
        orbital.Radius = Mathf.Lerp(orbital.Radius, wantedRadius, Damp(radiusSharpness, dt));

        Vector3 wantedFraming = Vector3.Lerp(playerPos + playerFocusOffset, enemyPos + enemyFocusOffset, framingBias);
        framingPoint.position = Vector3.Lerp(framingPoint.position, wantedFraming, Damp(framingSharpness, dt));
    }

    private void UpdateFree(float dt)
    {
        if (applyFreeSettings)orbital.Radius = Mathf.Lerp(orbital.Radius, freeRadius, Damp(radiusSharpness, dt));

        if (!recentering) return;

        recenterTimer += dt;
        Vector3 fwd = Focus.forward;
        fwd.y = 0f;

        if (fwd.sqrMagnitude > 0.0001f)
        {
            float yaw = Mathf.Atan2(fwd.x, fwd.z) * Mathf.Rad2Deg;
            float next = Mathf.LerpAngle(orbital.HorizontalAxis.Value, yaw, Damp(recenterSharpness, dt));
            orbital.HorizontalAxis.Value = Mathf.DeltaAngle(0f, next);
        }

        if (recenterTimer >= recenterDuration)
        {
            recentering = false;
            if (inputAxis != null) inputAxis.enabled = true;
        }
    }

    private void HandleLockFailed()
    {
        if (!recenterWhenNoTarget || orbital == null) return;
        recentering = true;
        recenterTimer = 0f;
        if (inputAxis != null) inputAxis.enabled = false;
    }
    private static float Damp(float sharpness, float dt)
    {
        return 1f - Mathf.Exp(-sharpness * dt);
    }
}