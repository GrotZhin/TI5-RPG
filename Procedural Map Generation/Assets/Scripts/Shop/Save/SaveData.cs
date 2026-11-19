using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int version = 1;

    [Header("Metadados do slot")]
    public int slotIndex;
    public string saveName;
    public long savedAtUtcTicks;
    public float playTimeSeconds;

    [Header("Checkpoint")]
    public string checkpointId;
    public Vector3 checkpointPosition;
    public float checkpointYaw;

    [Header("Status do player")]
    public float health;
    public float stamina;
    public float shield;
    public int coins;

    [Header("Inventário e loja")]
    public List<InventoryEntry> inventory = new List<InventoryEntry>();
    public List<VendorStockEntry> vendors = new List<VendorStockEntry>();

    public DateTime SavedAtLocal => new DateTime(savedAtUtcTicks, DateTimeKind.Utc).ToLocalTime();
}

[Serializable]
public class InventoryEntry
{
    public string itemId;
    public int quantity;
}

[Serializable]
public class VendorStockEntry
{
    public string vendorId;
    public List<ItemStockEntry> items = new List<ItemStockEntry>();
}

[Serializable]
public class ItemStockEntry
{
    public string itemId;
    public int stock;
}
