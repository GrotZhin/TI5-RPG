using UnityEngine;

[RequireComponent(typeof(LockOnSystem))]
public class LockOnTargetMarkerUI : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Camera referenceCamera;
    [SerializeField] private RectTransform markerRect;
    [SerializeField] private CanvasGroup markerCanvasGroup;

    [Header("Posicionamento")]
    [Tooltip("Deslocamento do pivô do inimigo")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.6f, 0f);

    [Header("Animação")]
    [SerializeField] private float popInDuration = 0.12f;
    [SerializeField] private float popScaleStart = 1.6f;
    [SerializeField] private AnimationCurve popCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private LockOnSystem lockOnSystem;
    private Transform trackedTarget;
    private float popTimer;
    private bool isVisible;

    private void Awake()
    {
        lockOnSystem = GetComponent<LockOnSystem>();
        if (referenceCamera == null) referenceCamera = Camera.main;
        SetVisible(false, instant: true);
    }

    private void OnEnable()
    {
        lockOnSystem.OnTargetLocked += HandleTargetLocked;
        lockOnSystem.OnTargetChanged += HandleTargetChanged;
        lockOnSystem.OnTargetUnlocked += HandleTargetUnlocked;
    }

    private void OnDisable()
    {
        lockOnSystem.OnTargetLocked -= HandleTargetLocked;
        lockOnSystem.OnTargetChanged -= HandleTargetChanged;
        lockOnSystem.OnTargetUnlocked -= HandleTargetUnlocked;
    }

    private void HandleTargetLocked(Transform target)
    {
        trackedTarget = target;
        popTimer = 0f;
        SetVisible(true, instant: false);
    }

    private void HandleTargetChanged(Transform target)
    {
        trackedTarget = target;
        popTimer = 0f;
    }

    private void HandleTargetUnlocked()
    {
        trackedTarget = null;
        SetVisible(false, instant: false);
    }

    private void LateUpdate()
    {
        if (trackedTarget == null || markerRect == null) return;

      
        Vector3 worldPos = trackedTarget.position + worldOffset;
        Vector3 screenPos = referenceCamera.WorldToScreenPoint(worldPos);

        bool behindCamera = screenPos.z < 0f;
        if (behindCamera)
        {
            markerCanvasGroup.alpha = 0f;
            return;
        }

        markerRect.position = screenPos;

        
        if (popTimer < popInDuration)
        {
            popTimer += Time.deltaTime;
            float t = popCurve.Evaluate(Mathf.Clamp01(popTimer / popInDuration));
            float scale = Mathf.Lerp(popScaleStart, 1f, t);
            markerRect.localScale = Vector3.one * scale;
        }

        markerCanvasGroup.alpha = isVisible ? 1f : 0f;
    }

    private void SetVisible(bool visible, bool instant)
    {
        isVisible = visible;
        if (markerCanvasGroup == null) return;

        if (instant || !visible)
        {
            markerCanvasGroup.alpha = visible ? 1f : 0f;
        }
        markerCanvasGroup.blocksRaycasts = false;
        markerCanvasGroup.interactable = false;
    }
}