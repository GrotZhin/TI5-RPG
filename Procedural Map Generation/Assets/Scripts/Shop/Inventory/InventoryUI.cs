using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Transform itemsContainer;
    public UIItemSlot slotPrefab;
    [Tooltip("Tecla para abrir/fechar o inventário.")]
    public Key toggleKey = Key.I;

    void Start()
    {
        inventoryPanel.SetActive(false);
        InventoryManager.Instance.OnInventoryChanged += RefreshUI;
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance != null) InventoryManager.Instance.OnInventoryChanged -= RefreshUI;
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null || !kb[toggleKey].wasPressedThisFrame) return;

        bool isOpen = inventoryPanel.activeSelf;
        if (!isOpen && GameUIState.IsUIOpen) return;
        ToggleInventory(!isOpen);
    }

    void ToggleInventory(bool open)
    {
        inventoryPanel.SetActive(open);
        GameUIState.IsUIOpen = open;
        if (open) RefreshUI();
    }

    void RefreshUI()
    {
        foreach (Transform child in itemsContainer) Destroy(child.gameObject);
        foreach (var slot in InventoryManager.Instance.slots)
        {
            UIItemSlot ui = Instantiate(slotPrefab, itemsContainer);
            ui.SetupInventory(slot);
        }
    }
}