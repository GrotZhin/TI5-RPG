using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UiControl : MonoBehaviour
{
    [Header("Input System")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputActionReference menuActionReference;

    [Header("UI Elements")]
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject currentButton;
    [SerializeField] private UiAni menuAni;

    private bool isMenuOpen;

    void Start()
    {
        if (menuCanvas != null) menuCanvas.SetActive(false);
    }

    void OnEnable()
    {
        if (menuActionReference != null && menuActionReference.action != null)
        {
            menuActionReference.action.performed += OnMenuPerformed;
        }
    }

    void OnDisable()
    {
        if (menuActionReference != null && menuActionReference.action != null)
        {
            menuActionReference.action.performed -= OnMenuPerformed;
        }
    }

    private void OnMenuPerformed(InputAction.CallbackContext context)
    {
        ToggleMenu();
    }

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;

        if (isMenuOpen)
        {
           
            playerInput.SwitchCurrentActionMap("UI");
            menuCanvas.SetActive(true); 
            menuAni.UIIntro();
        }
        else
        {
  
            menuAni.UIOutro();
            playerInput.SwitchCurrentActionMap("Player");
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}