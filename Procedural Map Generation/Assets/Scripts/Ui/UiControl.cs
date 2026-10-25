using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
public class UiControl : MonoBehaviour
{
    PlayerInput Input;
    Inputs inputs;
    [SerializeField] private InputInfo inputInfo;

    [SerializeField] InputActionReference Menu;
    public bool MenuUi;
    public GameObject MenuCanvas;
    [SerializeField] GameObject CurrentButton;
    public UiAni MenuAni;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   

    void Start()
    {
        MenuCanvas.SetActive(false);

        InputInfo.OnMenuEvent += OpenMenu;
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    public void OpenMenu()
    {
        Debug.Log("pause");
        //if(PlayerStats.IsDead) return;
        MenuUi = !MenuCanvas.activeSelf;

        if (MenuUi)
        {
            
            inputInfo.SetUi();
            MenuCanvas.SetActive(true);
            MenuAni.UIIntro();
          //  Time.timeScale = 0;
      
        
            EventSystem.current.SetSelectedGameObject(CurrentButton);
        }
        else
        {
            inputInfo.SetGameplay();
            MenuAni.UIOutro();
          //  Time.timeScale = 1;
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

public void ActiveMove(bool tf)
    {
     //   PlayerStats.cutScene = tf;
    }
    
}
