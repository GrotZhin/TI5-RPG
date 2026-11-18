using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UiControlMainMenu : MonoBehaviour
{

    [SerializeField] private InputInfo inputInfo;
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject currentButton;
    [SerializeField] private UiButtonManager buttonManager; 


    void Start()
    {
        buttonManager.InitializeManager(menuCanvas, currentButton);
    }

    
}
