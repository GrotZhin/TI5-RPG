using System.Collections;
using UnityEngine;
using TMPro;

public class FeedbackPopup : MonoBehaviour
{
    public static FeedbackPopup Instance { get; private set; }

    public GameObject popupPanel;
    public TMP_Text popupText;
    public float displayTime = 2f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        if (popupPanel != null) popupPanel.SetActive(false);
    }

    public void Show(string message, bool success)
    {
        StopAllCoroutines();
        StartCoroutine(ShowRoutine(message, success));
    }

    IEnumerator ShowRoutine(string message, bool success)
    {
        popupText.text = message;
        popupText.color = success ? Color.green : Color.red;
        popupPanel.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        popupPanel.SetActive(false);
    }
}
