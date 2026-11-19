using System.Collections.Generic;
using UnityEngine;

public class NPCVendor : MonoBehaviour, IInteractable
{
    public string vendorName = "Mercador";
    public ItemData[] itemsForSale;

    [Header("Save / Estoque")]
    [Tooltip("ID único deste vendedor no arquivo de save. Se vazio, usa o nome do GameObject.")]
    public string vendorId = "";
    [Tooltip("Se desligado, o estoque é infinito (comportamento antigo).")]
    public bool limitedStock = true;
    public int stockPerItem = 3;

    private readonly Dictionary<ItemData, int> stock = new Dictionary<ItemData, int>();

    public string Id => string.IsNullOrEmpty(vendorId) ? name : vendorId;

    void Awake()
    {
        ResetStock();
    }

    public void Interact(GameObject player)
    {
        DialogueManager.Instance.StartVendorDialogue(this);
    }

    public string GetInteractionPrompt()
    {
        return $"Pressione E para falar com {vendorName}";
    }

    public void ResetStock()
    {
        stock.Clear();
        if (itemsForSale == null) return;
        foreach (var item in itemsForSale)
        {
            if (item != null) stock[item] = stockPerItem;
        }
    }

    public int GetStock(ItemData item)
    {
        return stock.TryGetValue(item, out int amount) ? amount : 0;
    }

    public bool IsInStock(ItemData item)
    {
        return !limitedStock || GetStock(item) > 0;
    }

    public void ConsumeStock(ItemData item, int amount = 1)
    {
        if (!limitedStock) return;
        if (stock.ContainsKey(item)) stock[item] = Mathf.Max(0, stock[item] - amount);
    }

    public VendorStockEntry CaptureStock()
    {
        VendorStockEntry entry = new VendorStockEntry { vendorId = Id };
        foreach (var pair in stock)
        {
            entry.items.Add(new ItemStockEntry { itemId = ItemDatabase.GetId(pair.Key), stock = pair.Value });
        }
        return entry;
    }

    public void RestoreStock(VendorStockEntry entry, ItemDatabase database)
    {
        ResetStock();
        if (entry == null || database == null) return;

        foreach (ItemStockEntry saved in entry.items)
        {
            ItemData item = database.Get(saved.itemId);
            if (item != null && stock.ContainsKey(item)) stock[item] = saved.stock;
        }
    }
}

