using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIItemSlot : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text extraText; 
    public Button button;

    public void Setup(ItemData item, Action onClick)
    {
        icon.sprite = item.icon;
        nameText.text = item.itemName;
        extraText.text = $"{item.price} moedas";

        if (button != null)
        {
            button.interactable = true;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick());
        }
    }

    public void SetupInventory(InventorySlot slot)
    {
        icon.sprite = slot.item.icon;
        nameText.text = slot.item.itemName;
        extraText.text = $"x{slot.quantity}";
        if (button != null) button.interactable = false;
    }
}
