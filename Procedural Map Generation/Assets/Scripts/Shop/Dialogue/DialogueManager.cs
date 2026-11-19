using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text vendorNameText;
    public TMP_Text dialogueText;
    public Button shopButton;
    public Button closeButton;

    private NPCVendor currentVendor;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        dialoguePanel.SetActive(false);
        shopButton.onClick.AddListener(OpenShop);
        closeButton.onClick.AddListener(CloseDialogue);
    }

    public void StartVendorDialogue(NPCVendor vendor)
    {
        currentVendor = vendor;
        vendorNameText.text = vendor.vendorName;
        dialogueText.text = "Olá, viajante! Deseja ver os itens da loja?";

        dialoguePanel.SetActive(true);
        GameUIState.IsUIOpen = true;
    }

    void OpenShop()
    {
        dialoguePanel.SetActive(false);
        ShopManager.Instance.OpenShop(currentVendor);
    }

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        GameUIState.IsUIOpen = false;
    }
}
