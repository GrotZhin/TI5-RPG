using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("Referências")]
    public ItemDatabase itemDatabase;
    [Tooltip("Se vazio, procura o objeto com PlayerMove.")]
    public Transform player;
    [Header("Slots")]
    [Min(3)] public int slotCount = 3;
    [Header("Checkpoint inicial (antes de tocar em qualquer checkpoint)")]
    public string startCheckpointId = "Início";
    [Header("Morte")]
    [Tooltip("Ao morrer, volta ao último checkpoint com vida/estamina cheias.")]
    public bool respawnOnDeath = true;
    public float respawnDelay = 1.5f;

    public string CurrentCheckpointId { get; private set; }
    public Vector3 CurrentCheckpointPosition { get; private set; }
    public float CurrentCheckpointYaw { get; private set; }
    public float PlayTimeSeconds { get; private set; }

    public event Action OnSlotsChanged;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        if (player == null)
        {
            PlayerMove pm = FindFirstObjectByType<PlayerMove>();
            if (pm != null) player = pm.transform;
        }

        if (player != null)
        {
            CurrentCheckpointId = startCheckpointId;
            CurrentCheckpointPosition = player.position;
            CurrentCheckpointYaw = player.eulerAngles.y;
        }
        else
        {
            Debug.LogError("[SaveManager] Player não encontrado. Arraste-o no campo 'Player'.");
        }

        if (itemDatabase == null) Debug.LogError("[SaveManager] Falta atribuir o ItemDatabase.");

        if (PlayerStats.Instance != null) PlayerStats.Instance.OnDied += HandlePlayerDied;
    }

    void OnDestroy()
    {
        if (Instance != this) return;
        if (PlayerStats.Instance != null) PlayerStats.Instance.OnDied -= HandlePlayerDied;
    }

    void Update()
    {
        PlayTimeSeconds += Time.unscaledDeltaTime;
    }

    public void SetCheckpoint(string id, Vector3 position, float yaw)
    {
        CurrentCheckpointId = id;
        CurrentCheckpointPosition = position;
        CurrentCheckpointYaw = yaw;
    }

    void HandlePlayerDied()
    {
        if (respawnOnDeath) StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSecondsRealtime(respawnDelay);
        RespawnAtCheckpoint();
    }

    public void RespawnAtCheckpoint()
    {
        TeleportPlayer(CurrentCheckpointPosition, CurrentCheckpointYaw);
        if (PlayerStats.Instance != null) PlayerStats.Instance.FullRestore();
        if (FeedbackPopup.Instance != null) FeedbackPopup.Instance.Show($"Voltou ao checkpoint: {CurrentCheckpointId}", false);
    }

    public SaveData PeekSlot(int slot) => SaveSystem.Read(slot);

    public bool SlotFileExists(int slot) => SaveSystem.Exists(slot);

    public bool SaveToSlot(int slot)
    {
        if (!IsValidSlot(slot) || player == null) return false;

        SaveData data = CaptureState(slot);
        bool ok = SaveSystem.Write(data, slot);
        if (ok) OnSlotsChanged?.Invoke();
        return ok;
    }

    public bool LoadFromSlot(int slot)
    {
        if (!IsValidSlot(slot)) return false;

        SaveData data = SaveSystem.Read(slot);
        if (data == null) return false;

        ApplyState(data);
        OnSlotsChanged?.Invoke();
        return true;
    }

    public void DeleteSlot(int slot)
    {
        if (!IsValidSlot(slot)) return;
        SaveSystem.Delete(slot);
        OnSlotsChanged?.Invoke();
    }

    bool IsValidSlot(int slot) => slot >= 0 && slot < slotCount;

    SaveData CaptureState(int slot)
    {
        SaveData d = new SaveData
        {
            slotIndex = slot,
            saveName = $"Partida {slot + 1}",
            savedAtUtcTicks = DateTime.UtcNow.Ticks,
            playTimeSeconds = PlayTimeSeconds,
            checkpointId = CurrentCheckpointId,
            checkpointPosition = CurrentCheckpointPosition,
            checkpointYaw = CurrentCheckpointYaw
        };

        PlayerStats stats = PlayerStats.Instance;
        if (stats != null)
        {
            d.health = stats.currentHealth;
            d.stamina = stats.currentStamina;
            d.shield = stats.currentShield;
        }

        if (PlayerWallet.Instance != null) d.coins = PlayerWallet.Instance.coins;

        if (InventoryManager.Instance != null)
        {
            foreach (InventorySlot s in InventoryManager.Instance.slots)
            {
                if (s == null || s.item == null) continue;
                d.inventory.Add(new InventoryEntry { itemId = ItemDatabase.GetId(s.item), quantity = s.quantity });
            }
        }

        foreach (NPCVendor vendor in FindObjectsByType<NPCVendor>(FindObjectsSortMode.None)) d.vendors.Add(vendor.CaptureStock());
        return d;
    }

    void ApplyState(SaveData d)
    {
        if (InventoryManager.Instance != null)
        {
            List<InventorySlot> restored = new List<InventorySlot>();
            foreach (InventoryEntry e in d.inventory)
            {
                ItemData item = itemDatabase != null ? itemDatabase.Get(e.itemId) : null;
                if (item == null)
                {
                    Debug.LogWarning($"[SaveManager] Item '{e.itemId}' não existe no ItemDatabase. Ignorado.");
                    continue;
                }
                restored.Add(new InventorySlot(item, e.quantity));
            }
            InventoryManager.Instance.ReplaceAll(restored);
        }

        if (PlayerStats.Instance != null)
            PlayerStats.Instance.ApplySavedValues(d.health, d.stamina, d.shield);

        if (PlayerWallet.Instance != null)
            PlayerWallet.Instance.SetCoins(d.coins);

        foreach (NPCVendor vendor in FindObjectsByType<NPCVendor>(FindObjectsSortMode.None))
        {
            VendorStockEntry entry = d.vendors.Find(v => v.vendorId == vendor.Id);
            if (entry != null) vendor.RestoreStock(entry, itemDatabase);
            else vendor.ResetStock();
        }

        SetCheckpoint(d.checkpointId, d.checkpointPosition, d.checkpointYaw);
        TeleportPlayer(d.checkpointPosition, d.checkpointYaw);

        PlayTimeSeconds = d.playTimeSeconds;
    }
    void TeleportPlayer(Vector3 position, float yaw)
    {
        if (player == null) return;

        Vector3 delta = position - player.position;

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.SetPositionAndRotation(position, Quaternion.Euler(0f, yaw, 0f));

        if (cc != null) cc.enabled = true;

        PlayerMove pm = player.GetComponent<PlayerMove>();
        if (pm != null)
        {
            pm.ySpeed = 0f;
            pm.SetTargetVelocity(Vector3.zero);
        }

        WarpCameras(delta);
    }

    void WarpCameras(Vector3 delta)
    {
        for (int i = 0; i < CinemachineCore.VirtualCameraCount; i++)
        {
            var cam = CinemachineCore.GetVirtualCamera(i);
            Transform follow = cam != null ? cam.Follow : null;
            if (follow == null) continue;

            if (follow == player || follow.IsChildOf(player))
                cam.OnTargetObjectWarped(follow, delta);
        }
    }
}