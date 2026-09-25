using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(LockOnSystem))]
public class LockOnCameraController : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineCamera freeCamera;
    [SerializeField] private CinemachineCamera lockOnCamera;

    [Header("Prioridades")]
    [SerializeField] private int activePriority = 20;
    [SerializeField] private int inactivePriority = 10;

    [Header("Ajuste de composição em lock-on")]
    [SerializeField] private Vector2 lockOnScreenOffset = new Vector2(0f, 0.15f);

    [Header("Orbita durante o lock")]
    [Tooltip("Velocidade com que a câmera reposiciona atrás do player enquanto ele se move")]
    [SerializeField] private float orbitFollowSpeed = 10f;

    private LockOnSystem lockOnSystem;
    private CinemachineRotationComposer rotationComposer;
    private CinemachineInputAxisController lockOnInputAxisController;
    private CinemachineOrbitalFollow orbitalFollow;

    private void Awake()
    {
        lockOnSystem = GetComponent<LockOnSystem>();

        if (lockOnCamera != null)
        {
            rotationComposer = lockOnCamera.GetComponent<CinemachineRotationComposer>();
            lockOnInputAxisController = lockOnCamera.GetComponent<CinemachineInputAxisController>();
            orbitalFollow = lockOnCamera.GetComponent<CinemachineOrbitalFollow>();
        }
    }

    private void OnEnable()
    {
        lockOnSystem.OnTargetLocked += HandleTargetLocked;
        lockOnSystem.OnTargetChanged += HandleTargetChanged;
        lockOnSystem.OnTargetUnlocked += HandleTargetUnlocked;
        SetPriorities(locked: false);
    }

    private void OnDisable()
    {
        lockOnSystem.OnTargetLocked -= HandleTargetLocked;
        lockOnSystem.OnTargetChanged -= HandleTargetChanged;
        lockOnSystem.OnTargetUnlocked -= HandleTargetUnlocked;
    }

    private void Update()
    {
        if (!lockOnSystem.IsLockedOn || orbitalFollow == null || lockOnSystem.CurrentTarget == null) return;

        Vector3 toTarget = lockOnSystem.CurrentTarget.position - lockOnSystem.transform.position;
        toTarget.y = 0f;
        if (toTarget.sqrMagnitude < 0.0001f) return;

        float targetAngle = Mathf.Atan2(toTarget.x, toTarget.z) * Mathf.Rad2Deg;


        orbitalFollow.HorizontalAxis.Value = Mathf.LerpAngle(orbitalFollow.HorizontalAxis.Value,targetAngle,orbitFollowSpeed * Time.deltaTime);
    }

    private void HandleTargetLocked(Transform target)
    {
        ApplyTarget(target);
        SetPriorities(locked: true);

        if (lockOnInputAxisController != null) lockOnInputAxisController.enabled = false;
    }

    private void HandleTargetChanged(Transform target)
    {
        ApplyTarget(target);
    }

    private void HandleTargetUnlocked()
    {
        SetPriorities(locked: false);

        if (lockOnInputAxisController != null) lockOnInputAxisController.enabled = true;
    }

    private void ApplyTarget(Transform target)
    {
        if (lockOnCamera == null) return;

        lockOnCamera.Target.TrackingTarget = lockOnSystem.transform;
        lockOnCamera.Target.LookAtTarget = target;

        if (rotationComposer != null)
        {
            rotationComposer.Composition.ScreenPosition = lockOnScreenOffset;
        }
    }

    private void SetPriorities(bool locked)
    {
        if (freeCamera != null) freeCamera.Priority = locked ? inactivePriority : activePriority;

        if (lockOnCamera != null) lockOnCamera.Priority = locked ? activePriority : inactivePriority;
    }
}