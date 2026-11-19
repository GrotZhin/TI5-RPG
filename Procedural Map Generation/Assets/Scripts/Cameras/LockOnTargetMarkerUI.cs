using UnityEngine;
using DG.Tweening; // Importação necessária para o DOTween

[RequireComponent(typeof(LockOnSystem))]
public class LockOnTargetMarkerUI : MonoBehaviour
{

    [SerializeField] private Camera referenceCamera;
    [SerializeField] private RectTransform markerRect;
    [SerializeField] private RectTransform LettersRect;
    [SerializeField] private CanvasGroup markerCanvasGroup;

    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.6f, 0f);

    [SerializeField] private float popInDuration;
    [SerializeField] private float FadeDur;
    [SerializeField] private float ROtDur;
    [SerializeField] private float popSizeStart ;
    [SerializeField] private float popLettersStart ;
    [SerializeField] private Ease popEase = Ease.InOutSine;

    private LockOnSystem lockOnSystem;
    private Transform trackedTarget;
    private Sequence ActiveSizeTween; 
    private Tween activeAlphaTween;

    private void Awake()
    {
        lockOnSystem = GetComponent<LockOnSystem>();
        if (referenceCamera == null) referenceCamera = Camera.main;
        if (markerCanvasGroup != null)
        {
            markerCanvasGroup.blocksRaycasts = false;
            markerCanvasGroup.interactable = false;
        }
        
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
        KillActiveTweens();
    }

    private void HandleTargetLocked(Transform target)
    {
        trackedTarget = target;
        TriggerPopInAnimation();
        SetVisible(true, instant: false);
    }

    private void HandleTargetChanged(Transform target)
    {
        trackedTarget = target;
        TriggerPopInAnimation();
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
            if (markerCanvasGroup != null) markerCanvasGroup.alpha = 0f;
            return;
        }

        markerRect.position = screenPos;
    }

     private void TriggerPopInAnimation()
    {

        ActiveSizeTween?.Kill();


        if (markerRect != null) markerRect.localScale = Vector3.one * popSizeStart;
        if (LettersRect != null) LettersRect.localScale = Vector3.one * popLettersStart;


        ActiveSizeTween = DOTween.Sequence().SetUpdate(UpdateType.Late);


        ActiveSizeTween.Join(markerRect.DOScale(Vector3.one, popInDuration).SetEase(popEase));
        ActiveSizeTween.Join(LettersRect.DOScale(Vector3.one, popInDuration).SetEase(popEase));
        ActiveSizeTween.Join(LettersRect.DORotate(new Vector3(0,0,-360), ROtDur,RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental));
    }

    

    private void SetVisible(bool visible, bool instant)
    {
        if (markerCanvasGroup == null) return;

        activeAlphaTween?.Kill();

        float targetAlpha = visible ? 1f : 0f;

        if (instant)
        {
            markerCanvasGroup.alpha = targetAlpha;
        }
        else
        {
            activeAlphaTween = markerCanvasGroup.DOFade(targetAlpha, FadeDur)
                .SetUpdate(UpdateType.Late);
        }
    }

    private void KillActiveTweens()
    {
        ActiveSizeTween?.Kill();
        activeAlphaTween?.Kill();
    }
}