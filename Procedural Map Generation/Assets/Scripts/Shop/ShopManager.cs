using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("UI - Painel geral")]
    public GameObject shopPanel;
    public Transform itemsContainer;
    public UIItemSlot itemSlotPrefab;
    public TMP_Text coinsText;

    [Header("UI - Detalhe do item selecionado")]
    public Image selectedItemIcon;
    public TMP_Text selectedItemName;
    public TMP_Text selectedItemDescription;
    public TMP_Text selectedItemPrice;
    public Button buyButton;
    public Button closeButton;

    private NPCVendor currentVendor;
    private ItemData selectedItem;
    private readonly List<UIItemSlot> spawnedSlots = new List<UIItemSlot>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        shopPanel.SetActive(false);
        buyButton.onClick.AddListener(ConfirmPurchase);
        closeButton.onClick.AddListener(CloseShop);
    }

    void OnEnable()
    {
        if (PlayerWallet.Instance != null) PlayerWallet.Instance.OnCoinsChanged += UpdateCoinsUI;
    }

    void OnDisable()
    {
        if (PlayerWallet.Instance != null) PlayerWallet.Instance.OnCoinsChanged -= UpdateCoinsUI;
    }

    public void OpenShop(NPCVendor vendor)
    {
        currentVendor = vendor;
        selectedItem = null;
        ClearSelection();
        RebuildItemList(vendor.itemsForSale);
        UpdateCoinsUI(PlayerWallet.Instance.coins);
        shopPanel.SetActive(true);
        GameUIState.IsUIOpen = true;
    }

    void RebuildItemList(ItemData[] items)
    {
        foreach (var slot in spawnedSlots) Destroy(slot.gameObject);
        spawnedSlots.Clear();

        foreach (var item in items)
        {
            ItemData capturedItem = item;
            UIItemSlot slot = Instantiate(itemSlotPrefab, itemsContainer);
            slot.Setup(capturedItem, () => SelectItem(capturedItem));

            if (currentVendor != null && currentVendor.limitedStock)
            {
                int left = currentVendor.GetStock(capturedItem);
                slot.extraText.text = left > 0 ? $"{capturedItem.price} moedas (x{left})" : "ESGOTADO";
            }
            spawnedSlots.Add(slot);
        }
    }

    void SelectItem(ItemData item)
    {
        selectedItem = item;
        selectedItemIcon.sprite = item.icon;
        selectedItemName.text = item.itemName;
        selectedItemDescription.text = item.description;
        selectedItemPrice.text = $"{item.price} moedas";
        buyButton.interactable = true;
    }

    void ClearSelection()
    {
        selectedItem = null;
        selectedItemIcon.sprite = null;
        selectedItemName.text = "";
        selectedItemDescription.text = "";
        selectedItemPrice.text = "";
        buyButton.interactable = false;
    }

    void ConfirmPurchase()
    {
        if (selectedItem == null) return;

        if (currentVendor != null && !currentVendor.IsInStock(selectedItem))
        {
            FeedbackPopup.Instance.Show("Item esgotado!", false);
            return;
        }

        if (PlayerWallet.Instance.TrySpend(selectedItem.price))
        {
            InventoryManager.Instance.AddItem(selectedItem, 1);
            if (currentVendor != null)
            {
                currentVendor.ConsumeStock(selectedItem, 1);
                RebuildItemList(currentVendor.itemsForSale);
            }
            FeedbackPopup.Instance.Show($"{selectedItem.itemName} adquirido!", true);
            UpdateCoinsUI(PlayerWallet.Instance.coins);
        }
        else
        {
            FeedbackPopup.Instance.Show("Saldo insuficiente!", false);
        }
    }

    void UpdateCoinsUI(int amount)
    {
        coinsText.text = $"Moedas: {amount}";
    }
    public void CloseShop()
    {
        shopPanel.SetActive(false);
        DialogueManager.Instance.StartVendorDialogue(currentVendor);
    }
}