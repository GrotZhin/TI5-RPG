using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class UIInputBridge : MonoBehaviour
{
    [Tooltip("O MESMO asset InputInfo que o PlayerMove usa.")]
    [SerializeField] InputInfo input;

    [Tooltip("Se vazio, procura todos os CinemachineInputAxisController da cena.")]
    [SerializeField] CinemachineInputAxisController[] cameraInputs;

    [SerializeField] bool controlCursor = true;

    readonly List<CinemachineInputAxisController> disabledByUs = new List<CinemachineInputAxisController>();

    void Awake()
    {
        if (cameraInputs == null || cameraInputs.Length == 0)cameraInputs = FindObjectsByType<CinemachineInputAxisController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }

    void OnEnable() => GameUIState.OnChanged += Apply;
    void OnDisable() => GameUIState.OnChanged -= Apply;
    void Start() => Apply(GameUIState.IsUIOpen);

    void Apply(bool uiOpen)
    {
        if (input != null)
        {
            if (uiOpen) input.SetUi();
            else input.SetGameplay();
        }

        if (uiOpen)
        {
            disabledByUs.Clear();
            foreach (var c in cameraInputs)
            {
                if (c != null && c.enabled)
                {
                    c.enabled = false;
                    disabledByUs.Add(c);
                }
            }
        }
        else
        {
            foreach (var c in disabledByUs)
                if (c != null) c.enabled = true;
            disabledByUs.Clear();
        }

        if (controlCursor)
        {
            Cursor.lockState = uiOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = uiOpen;
        }
    }
}