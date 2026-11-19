using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public List<InventorySlot> slots = new List<InventorySlot>();

    public event Action OnInventoryChanged;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddItem(ItemData item, int amount = 1)
    {
        InventorySlot existing = slots.Find(s => s.item == item);
        if (existing != null) existing.quantity += amount;
        else slots.Add(new InventorySlot(item, amount));
        OnInventoryChanged?.Invoke();
    }

    public bool HasItem(ItemData item)
    {
        return slots.Exists(s => s.item == item && s.quantity > 0);
    }
    public void ReplaceAll(List<InventorySlot> newSlots)
    {
        slots.Clear();
        slots.AddRange(newSlots);
        OnInventoryChanged?.Invoke();
    }
}
