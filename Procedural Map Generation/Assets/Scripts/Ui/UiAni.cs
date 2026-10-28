using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
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

    [Header("Options Settings & Tweens")]
    public bool options;
    [SerializeField] private RectTransform OptionsGroup;
    [SerializeField] private RectTransform OptionsBg;
  
    [SerializeField] private CanvasGroup OptionsiconCG;
    public GameObject Settings;
  
    [Header("Control Schemes")]
    [SerializeField] private CanvasGroup SchemesGroup;
    public GameObject KBCtrlscheme;
    public GameObject MouseCtrlscheme;
    public GameObject GPCtrlscheme;

    private bool OptionsisAni = false;

    void Start()
    {
        if (Settings != null) Settings.SetActive(false);
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

    }

    public void UIIntro()
    {
        // UiSfXManager.soundManager.PlaySound(UiSfXManager.SoundType.OpenMenu);
       
        UiCG.DOKill();
        
        if (Input != null) Input.SwitchCurrentActionMap("UI");
        
        UiCG.alpha = 0;
        
        // Sequência de Entrada
        Sequence intro = DOTween.Sequence().SetUpdate(true);
        intro.Join(UiCG.DOFade(1, 1f));
        //.OnComplete(() => inputInfo.SetUi());
        
    }

    public void UIOutro()
    {
        // UiSfXManager.soundManager.PlaySound(UiSfXManager.SoundType.CloseMenu);
       
       
        UiCG.DOKill();
        options = false;
        OptionsAni();

        Sequence outro = DOTween.Sequence().SetUpdate(true);
        outro.Join(UiCG.DOFade(0, 1f).SetEase(Ease.InFlash));
        outro.OnComplete(() => 
        { 
            if (UI != null) UI.SetActive(false);
            //inputInfo.SetGameplay();
            if (Input != null) Input.SwitchCurrentActionMap("Player");
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
       
        OptionsiconCG.DOKill();

        OptionsisAni = true;
        Sequence animSeq = DOTween.Sequence().SetUpdate(true);

        if (options)
        {

            if (Settings != null && !Settings.activeSelf)
            {
                // UiSfXManager.soundManager.PlaySound(UiSfXManager.SoundType.OpenSettings);
                Settings.SetActive(true);
            }



            animSeq.Join(SchemesGroup.DOFade(0f, 0.5f));
            animSeq.OnComplete(() => OptionsisAni = false);
        }
        else
        {
            if (Settings != null && Settings.activeSelf)
            {
                // UiSfXManager.soundManager.PlaySound(UiSfXManager.SoundType.CloseSettings);
            }


            animSeq.OnComplete(() =>
            {
                OptionsisAni = false;
                if (Settings != null) Settings.SetActive(false);
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

    public void BoolOnSound()
    {
        // UiSfXManager.soundManager.PlaySound(UiSfXManager.SoundType.BoolOn);
    }

    public void BoolOffSound()
    {
        // UiSfXManager.soundManager.PlaySound(UiSfXManager.SoundType.BoolOff);
    }
}