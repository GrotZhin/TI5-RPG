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
    [Tooltip("Offset de centro da tela")]
    [SerializeField] private Vector2 lockOnScreenOffset = new Vector2(0f, 0.15f);

    private LockOnSystem lockOnSystem;
    private CinemachineRotationComposer rotationComposer;

    private void Awake()
    {
        lockOnSystem = GetComponent<LockOnSystem>();

        if (lockOnCamera != null) rotationComposer = lockOnCamera.GetComponent<CinemachineRotationComposer>();
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

    private void HandleTargetLocked(Transform target)
    {
        ApplyTarget(target);
        SetPriorities(locked: true);
    }

    private void HandleTargetChanged(Transform target)
    {
        ApplyTarget(target);
    }

    private void HandleTargetUnlocked()
    {
        SetPriorities(locked: false);
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
        if (freeCamera != null)
            freeCamera.Priority = locked ? inactivePriority : activePriority;

        if (lockOnCamera != null)
            lockOnCamera.Priority = locked ? activePriority : inactivePriority;
    }
}