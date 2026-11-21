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

        if (slot.item is AdornoData adorno)
        {
            nameText.text = $"<color=#{adorno.tier.ToHex()}>{adorno.itemName}</color> <size=70%>({adorno.tier.Label()})</size>";
            string good = "7CFC7C", bad = "FF6B6B";
            extraText.text = $"<color=#{adorno.category.ToHex()}>{adorno.category.Label()}</color>  x{slot.quantity}\n" + $"<color=#{(adorno.primary.IsBeneficial ? good : bad)}>{adorno.primary.Describe(slot.quantity)}</color>\n" + $"<color=#{(adorno.secondary.IsBeneficial ? good : bad)}>{adorno.secondary.Describe(slot.quantity)}</color>";
            if (adorno.icon == null) icon.color = adorno.category.ToColor();
        }
    }
}
