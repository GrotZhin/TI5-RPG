using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class MainMenuUiAni : MonoBehaviour
{
    public string Name;
    [Header("Input & Canvas")]
    [SerializeField] private InputInfo inputInfo;
    public GameObject UI;

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

    void Start()
    {
        if (Settings != null) Settings.SetActive(false);
        buttonManager = GetComponent<UiButtonManager>();
        Time.timeScale = 1f;

        if (inputInfo != null) inputInfo.SetUi();

        if (buttonManager != null && UI != null && MenuOptnButton != null)
        {
            buttonManager.InitializeManager(UI, MenuOptnButton);
        }
        else if (EventSystem.current != null && MenuOptnButton != null)
        {
            EventSystem.current.SetSelectedGameObject(MenuOptnButton);
        }
    }

    void OnEnable()
    {
        // if (Input != null)
        // {
        //     Input.onControlsChanged += ControlsChanged;
        //     ControlSchemes(Input.currentControlScheme);
        // }
    }

    void OnDisable()
    {
        // if (Input != null)
        // {
        //     Input.onControlsChanged -= ControlsChanged;
        // }
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
        Sequence animSeq = DOTween.Sequence();

        if (options)
        {
            if (Settings != null && !Settings.activeSelf)
            {
                Settings.SetActive(true);
            }

            OptionsGroup.localScale = Vector3.zero;
            OptionsGroup.localRotation = Quaternion.Euler(0, 0, -180f); 
            
            animSeq.Join(SchemesGroup.DOFade(0f, 0.2f));        
            animSeq.Join(OptionsCG.DOFade(1f, 0.4f));
            animSeq.Join(Pointer.DOFade(0f, 0));
            animSeq.Join(OptionsGroup.DOAnchorPos(OptionsGroupIntroPos, OptionsGroupTweendur).SetEase(OptionEaseIntro));
            animSeq.Join(OptionsGroup.DOScale(Vector3.one, OptionsGroupTweendur).SetEase(Ease.OutBack));
            animSeq.Join(OptionsGroup.DOLocalRotate(Vector3.zero, OptionsGroupTweendur, RotateMode.FastBeyond360).SetEase(OptionEaseRotIntro));
            animSeq.Join(Tape.DOAnchorPos(TapeIntroPos, TapeTweendur).SetEase(TapeEase));

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
            animSeq.Join(OptionsGroup.DOAnchorPos(OptionsGroupOutroPos, OptionsGroupTweendur).SetEase(OptionEaseOutro));
            animSeq.Join(OptionsGroup.DOScale(Vector3.zero, OptionsGroupTweendur * 0.8f).SetEase(Ease.InBack));
            animSeq.Join(OptionsGroup.DOLocalRotate(new Vector3(0, 0, 180f), OptionsGroupTweendur, RotateMode.FastBeyond360).SetEase(OptionEaseRotOutro));
            animSeq.Join(Tape.DOAnchorPos(TapeOutroPos, TapeTweendur).SetEase(Ease.InBack));
            
            animSeq.OnComplete(() =>
            {
                Pointer.DOFade(1f, 0.1f);
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
    public void Exit()
    {
        Application.Quit();
    }
    public void StartToLobby()
    {
        SceneManager.LoadScene(Name);
    }
}
