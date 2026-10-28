using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UiControl : MonoBehaviour
{
    [Header("Input System")]
    [SerializeField] private PlayerInput playerInput;

    [Header("UI Elements")]
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject currentButton;
    [SerializeField] private UiAni menuAni;

    private bool isMenuOpen;

    void Start()
    {
        if (menuCanvas != null) menuCanvas.SetActive(false);
    }
    public void OnMenu(InputValue value)
    {

        if (value.isPressed)
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;

        if (isMenuOpen)
        {
            if (playerInput != null) playerInput.SwitchCurrentActionMap("UI");
            if (menuCanvas != null) menuCanvas.SetActive(true); 
            if (menuAni != null) menuAni.UIIntro();
            
            if (currentButton != null && EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(currentButton);
            }
        }
        else
        {
            if (menuAni != null) menuAni.UIOutro();
            if (playerInput != null) playerInput.SwitchCurrentActionMap("Player");
            
        }
    }

    public void Lobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}