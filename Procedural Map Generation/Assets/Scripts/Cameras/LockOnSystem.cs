using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LockOnSystem : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Câmera de calculo")]
    [SerializeField] private Camera referenceCamera;
    [Tooltip("Transform do player ")]
    [SerializeField] private Transform playerRoot;

    [Header("Detecção")]
    [SerializeField] private float lockRadius = 15f;
    [Tooltip("Camada de inimigo")]
    [SerializeField] private LayerMask enemyLayer;
    [Tooltip("Camada de obstaculo")]
    [SerializeField] private LayerMask obstructionLayer;
    [Tooltip("Ângulo máximo ")]
    [SerializeField] private float maxLockAngle = 100f;

    [Header("Troca de alvo ")]
    [SerializeField] private float switchTargetCooldown = 0.25f;
    [SerializeField] private float switchStickThreshold = 0.6f;

    [Header("Input")]
    [Tooltip("Button: R3 MBM")]
    [SerializeField] private InputActionReference lockOnAction;
    [Tooltip("Axis: Mouse scroll Gamepad stick ")]
    [SerializeField] private InputActionReference switchTargetAction;

    public Transform CurrentTarget { get; private set; }
    public bool IsLockedOn => CurrentTarget != null;
    public Transform PlayerRoot => playerRoot != null ? playerRoot : transform;

    private Camera Cam
    {
        get
        {
            if (referenceCamera == null) referenceCamera = Camera.main;
            return referenceCamera;
        }
    }

    public event System.Action<Transform> OnTargetLocked;
    public event System.Action OnTargetUnlocked;
    public event System.Action<Transform> OnTargetChanged;
    public event System.Action OnLockFailed;

    private float switchCooldownTimer;
    private readonly Collider[] overlapBuffer = new Collider[32];

    private void Reset()
    {
        referenceCamera = Camera.main;
        playerRoot = transform;
    }

    private void OnEnable()
    {
        if (lockOnAction != null)
        {
            lockOnAction.action.Enable();
            lockOnAction.action.performed += OnLockOnPressed;
        }
        if (switchTargetAction != null) switchTargetAction.action.Enable();
    }

    private void OnDisable()
    {
        if (lockOnAction != null) lockOnAction.action.performed -= OnLockOnPressed;
    }

    private void Update()
    {
        if (!IsLockedOn) return;

        if (CurrentTarget == null || !CurrentTarget.gameObject.activeInHierarchy || Vector3.Distance(playerRoot.position, CurrentTarget.position) > lockRadius * 1.5f)
        {
            Unlock();
            return;
        }

        HandleTargetSwitch();
    }

    private void OnLockOnPressed(InputAction.CallbackContext ctx)
    {
        if (IsLockedOn)
        {
            Unlock();
        }
        else
        {
            TryLockOnBestTarget();
        }
    }

    public void TryLockOnBestTarget()
    {
        Transform best = FindBestTarget(preferFrontOnly: true);
        if (best != null)
        {
            CurrentTarget = best;
            OnTargetLocked?.Invoke(CurrentTarget);
            OnTargetChanged?.Invoke(CurrentTarget);
        }
        else
        {
            OnLockFailed?.Invoke();
        }
    }

    public void Unlock()
    {
        if (!IsLockedOn) return;
        CurrentTarget = null;
        OnTargetUnlocked?.Invoke();
    }

    private void HandleTargetSwitch()
    {
        if (switchTargetAction == null) return;
        switchCooldownTimer -= Time.deltaTime;
        if (switchCooldownTimer > 0f) return;
        float axis = switchTargetAction.action.ReadValue<float>();
        if (Mathf.Abs(axis) < switchStickThreshold) return;

        Transform next = FindTargetInDirection(axis > 0f ? 1f : -1f);
        if (next != null && next != CurrentTarget)
        {
            CurrentTarget = next;
            switchCooldownTimer = switchTargetCooldown;
            OnTargetChanged?.Invoke(CurrentTarget);
        }
    }
    private Transform FindBestTarget(bool preferFrontOnly)
    {
        int count = Physics.OverlapSphereNonAlloc(playerRoot.position, lockRadius, overlapBuffer, enemyLayer);
        Transform best = null;
        float bestScore = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Transform candidate = overlapBuffer[i].transform;
            if (!IsValidTarget(candidate)) continue;

            if (preferFrontOnly)
            {
                Vector3 toCandidate = (candidate.position - playerRoot.position).normalized;
                Vector3 viewForward = Cam != null ? Cam.transform.forward : playerRoot.forward;
                float angle = Vector3.Angle(viewForward, toCandidate);
                if (angle > maxLockAngle) continue;
            }

            float score = ScreenDistanceScore(candidate);
            if (score < bestScore)
            {
                bestScore = score;
                best = candidate;
            }
        }

        return best;
    }

    private Transform FindTargetInDirection(float dir)
    {
        if (CurrentTarget == null) return FindBestTarget(false);
        if (Cam == null) return null;

        Vector3 currentScreenPos = Cam.WorldToScreenPoint(CurrentTarget.position);
        int count = Physics.OverlapSphereNonAlloc(playerRoot.position, lockRadius, overlapBuffer, enemyLayer);
        Transform best = null;
        float bestScore = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Transform candidate = overlapBuffer[i].transform;
            if (candidate == CurrentTarget || !IsValidTarget(candidate)) continue;

            Vector3 candidateScreenPos = Cam.WorldToScreenPoint(candidate.position);
            float deltaX = candidateScreenPos.x - currentScreenPos.x;

            if (Mathf.Sign(deltaX) != Mathf.Sign(dir)) continue;

            float score = Mathf.Abs(deltaX) + Mathf.Abs(candidateScreenPos.y - currentScreenPos.y) * 0.5f;
            if (score < bestScore)
            {
                bestScore = score;
                best = candidate;
            }
        }

        return best;
    }

    private bool IsValidTarget(Transform candidate)
    {
        if (candidate == null || !candidate.gameObject.activeInHierarchy) return false;
        Vector3 origin = playerRoot.position + Vector3.up * 1f;
        Vector3 targetPos = candidate.position + Vector3.up * 1f;
        if (Physics.Linecast(origin, targetPos, obstructionLayer)) return false;

        return true;
    }

    private float ScreenDistanceScore(Transform candidate)
    {
        float worldDist = Vector3.Distance(playerRoot.position, candidate.position) / lockRadius;

        Camera cam = Cam;
        if (cam == null) return worldDist;

        Vector3 screenPos = cam.WorldToScreenPoint(candidate.position);
        if (screenPos.z < 0f) return float.MaxValue;

        Vector2 screenCenter = new Vector2(cam.pixelWidth * 0.5f, cam.pixelHeight * 0.5f);
        float screenDist = ((Vector2)screenPos - screenCenter).magnitude / cam.pixelHeight;

        return screenDist + worldDist * 0.5f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, lockRadius);
    }
}