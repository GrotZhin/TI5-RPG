using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class UiAni : MonoBehaviour
{
    [Header("Input & Canvas")]
    public PlayerInput Input;
    [SerializeField] private InputInfo inputInfo;
    [SerializeField] private CanvasGroup UiCG;
    public GameObject UI;

    [Header("Timing Config")]
    public float BRotationime;
    public float BFadeTime;
    public float TweenDur;
    public bool MenuActive;
    public float IntroOverlapTime;
    [Header("Ui Intro & Outro Tweens")]
    [SerializeField] private RectTransform TopPart;
    [SerializeField] private CanvasGroup TopPartCG;
    public float TopPartIntroPos;
    public float TopPartOutroPos;
    public float TopPartTweenDur;
    public Ease TopPartEase;

    [Header("Options Settings & Tweens")]
    public bool options;
    [SerializeField] private RectTransform OptionsGroup;
    public Vector2 OptionsGroupIntroPos;
    public Vector2 OptionsGroupOutroPos;
    public float OptionsGroupTweendur;
    public Ease OptionEaseIntro;
    public Ease OptionEaseOutro;
    public Ease OptionEaseRotIntro;
    public Ease OptionEaseRotOutro;
    [SerializeField] private RectTransform Tape;
    public Vector2 TapeIntroPos;
    public Vector2 TapeOutroPos;
    public float TapeTweendur;
    public Ease TapeEase;
    [SerializeField] private RectTransform OptionsBg;
    [SerializeField] private CanvasGroup Pointer;
    
    [SerializeField] private CanvasGroup OptionsCG;
    public GameObject Settings;
    [SerializeField] private GameObject FirstOptnButton;
    [SerializeField] private GameObject MenuOptnButton;
    
    private UiButtonManager buttonManager;

    [Header("Control Schemes")]
    [SerializeField] private CanvasGroup SchemesGroup;
    public GameObject KBCtrlscheme;
    public GameObject MouseCtrlscheme;
    public GameObject GPCtrlscheme;

    private bool OptionsisAni = false;
     private bool isOutroPlaying = false; 

    void Start()
    {
        if (Settings != null) Settings.SetActive(false);
        buttonManager = GetComponent<UiButtonManager>();
    }

    void OnEnable()
    {
        if (Input != null)
        {
            Input.onControlsChanged += ControlsChanged;
            ControlSchemes(Input.currentControlScheme);
        }
    }

    void OnDisable()
    {
        if (Input != null)
        {
            Input.onControlsChanged -= ControlsChanged;
        }
    }

    void ControlsChanged(PlayerInput input)
    {
        ControlSchemes(input.currentControlScheme);
    }

    void ControlSchemes(string scheme)
    {
        bool isGamepad = scheme == "Gamepad";
        
        if (KBCtrlscheme != null) KBCtrlscheme.SetActive(!isGamepad);
        if (MouseCtrlscheme != null) MouseCtrlscheme.SetActive(!isGamepad);
        if (GPCtrlscheme != null) GPCtrlscheme.SetActive(isGamepad);
    }
    
    public void Update()
    {
        if (options && EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
        {
            if (!EventSystem.current.currentSelectedGameObject.transform.IsChildOf(Settings.transform))
            {
                EventSystem.current.SetSelectedGameObject(FirstOptnButton);
            }
        }
    }

    public void UIIntro()
    {
        if (isOutroPlaying) return; 
        
        UiCG.DOKill();
        if (Input != null) Input.SwitchCurrentActionMap("UI");
        UiCG.alpha = 0;
        
        Sequence intro = DOTween.Sequence().SetUpdate(true);
        intro.Join(UiCG.DOFade(1, 1f));
        intro.Join(TopPart.DOAnchorPosY(TopPartIntroPos,TopPartTweenDur).SetEase(TopPartEase));
        intro.Join(TopPartCG.DOFade(1, 0.4f));
    }

    public void UIOutro()
    {
        if (isOutroPlaying) return; 
        isOutroPlaying = true;

        UiCG.DOKill();
        options = false;
        OptionsAni();

        if (Input != null) 
        {
            Input.currentActionMap?.Disable(); 
        }

        Sequence outro = DOTween.Sequence().SetUpdate(true);
        outro.Join(TopPart.DOAnchorPosY(TopPartOutroPos,TopPartTweenDur).SetEase(TopPartEase));
        outro.Join(TopPartCG.DOFade(0, 0.4f));
        outro.Join(UiCG.DOFade(0, 1f).SetEase(Ease.InFlash));
        
        outro.OnComplete(() => 
        { 
            if (UI != null) UI.SetActive(false);
            if (Input != null) Input.SwitchCurrentActionMap("Player"); 
            
            isOutroPlaying = false;
        });
    }
    
    public void OptionsToggle()
    {
        if (OptionsisAni) return;
        options = !options;
        OptionsAni();
    }

    public void OptionsAni()
    {
        OptionsGroup.DOKill();
        OptionsBg.DOKill();
        OptionsCG.DOKill();
        Tape.DOKill();

        OptionsisAni = true;
        Sequence animSeq = DOTween.Sequence().SetUpdate(true);

        if (options)
        {
            if (Settings != null && !Settings.activeSelf)
            {
                Settings.SetActive(true);
            }

            // --- TURBO SPIN INTRO ---
            OptionsGroup.localScale = Vector3.zero;
            // Começa girado em -180 graus (meia volta completa) para dar o efeito de twist
            OptionsGroup.localRotation = Quaternion.Euler(0, 0, -180f); 
            animSeq.Join(SchemesGroup.DOFade(0f, 0.2f));        
            animSeq.Join(OptionsCG.DOFade(1f, 0.4f));
            animSeq.Join(Pointer.DOFade(0f, 0));
            animSeq.Join(SchemesGroup.DOFade(0f, 0.2f));
            animSeq.Join(OptionsGroup.DOAnchorPos(OptionsGroupIntroPos, OptionsGroupTweendur).SetEase(OptionEaseIntro).SetUpdate(true));
            animSeq.Join(OptionsGroup.DOScale(Vector3.one, OptionsGroupTweendur).SetEase(Ease.OutBack).SetUpdate(true));
            animSeq.Join(OptionsGroup.DOLocalRotate(Vector3.zero, OptionsGroupTweendur, RotateMode.FastBeyond360).SetEase(OptionEaseRotIntro).SetUpdate(true));
            
            animSeq.Join(Tape.DOAnchorPos(TapeIntroPos, TapeTweendur).SetEase(TapeEase).SetUpdate(true));

            animSeq.OnComplete(() => 
            {
                OptionsisAni = false;

                if (buttonManager != null && Settings != null && FirstOptnButton != null)
                {
                    buttonManager.InitializeManager(Settings, FirstOptnButton);
                }
                else if (EventSystem.current != null && FirstOptnButton != null)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(FirstOptnButton);
                }
            });
        }
        else
        {
            animSeq.Join(SchemesGroup.DOFade(1f, 0.2f));
            
           animSeq.Join(OptionsCG.DOFade(0f, 0.4f));
            animSeq.Join(OptionsGroup.DOAnchorPos(OptionsGroupOutroPos, OptionsGroupTweendur).SetEase(OptionEaseOutro).SetUpdate(true));
            animSeq.Join(OptionsGroup.DOScale(Vector3.zero, OptionsGroupTweendur * 0.8f).SetEase(Ease.InBack).SetUpdate(true));
            animSeq.Join(OptionsGroup.DOLocalRotate(new Vector3(0, 0, 180f), OptionsGroupTweendur, RotateMode.FastBeyond360).SetEase(OptionEaseRotOutro).SetUpdate(true));
            
            animSeq.Join(Tape.DOAnchorPos(TapeOutroPos, TapeTweendur).SetEase(Ease.InBack).SetUpdate(true));
            
            animSeq.OnComplete(() =>
            {
                Pointer.DOFade(1f, 0.1f).SetUpdate(true);
                OptionsisAni = false;
                if (Settings != null) Settings.SetActive(false);

                if (buttonManager != null && UI != null && MenuOptnButton != null)
                {
                    buttonManager.InitializeManager(UI, MenuOptnButton);
                }
                else if (EventSystem.current != null && MenuOptnButton != null)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(MenuOptnButton);
                }
            });
        }
    }

    public void Resume()
    {
        if (inputInfo != null) inputInfo.SetGameplay();
        UIOutro();
        Time.timeScale = 1f;
    }

    private IEnumerator FadeOutBG(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null; 
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }

    public void BoolOnSound() { }
    public void BoolOffSound() { }
}