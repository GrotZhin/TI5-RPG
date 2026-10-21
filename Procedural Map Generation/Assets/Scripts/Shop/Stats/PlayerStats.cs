using System;

using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Valores base")]
    public float baseMaxHealth = 100f;
    public float baseMaxStamina = 100f;
    public float baseMaxShield = 0f;
    public float baseAttackMultiplier = 1f;

    [Header("Estamina - regenera��o")]
    public float staminaRegenPerSecond = 15f;
    public float exhaustedStaminaRegenPerSecond = 5f;
    [Range(0f, 1f)]
    public float exhaustedRecoverThreshold = 0.3f;

    [Header("Sistema de estamina")]
    [Tooltip("Desligado: pulo, dash e corrida não gastam estamina.")]
    public bool staminaEnabled = false;

    [Header("Valores atuais")]
    public float currentHealth;
    public float currentStamina;
    public float currentShield;


    public float MaxHealth { get; private set; }
    public float MaxStamina { get; private set; }
    public float MaxShield { get; private set; }
    public float AttackMultiplier { get; private set; }
    public bool IsExhausted { get; private set; }
    public bool IsDead { get; private set; }

    public event Action OnStatsChanged;
    public event Action OnDied;
    public Transform islandPosition;

    private bool staminaSpentThisFrame = false;
    private bool subscribedToInventory = false;

    [SerializeField]
    private CharacterController cc;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(this); return; }

        MaxHealth = baseMaxHealth;
        MaxStamina = baseMaxStamina;
        MaxShield = baseMaxShield;
        AttackMultiplier = baseAttackMultiplier;

        currentHealth = MaxHealth;
        currentStamina = MaxStamina;
        currentShield = MaxShield;
    }

    void OnEnable()
    {
        TrySubscribeInventory();
    }

    void OnDisable()
    {
        if (subscribedToInventory && InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= RecalculateStats;
        subscribedToInventory = false;
    }

    void Start()
    {


        TrySubscribeInventory();
        RecalculateStats();
    }

    void TrySubscribeInventory()
    {
        if (subscribedToInventory || InventoryManager.Instance == null) return;
        InventoryManager.Instance.OnInventoryChanged += RecalculateStats;
        subscribedToInventory = true;
    }

    void LateUpdate()
    {
        HandleStaminaRegen();
        staminaSpentThisFrame = false;
    }

    void HandleStaminaRegen()
    {
        if (staminaSpentThisFrame) return;
        if (currentStamina >= MaxStamina)
        {
            IsExhausted = false;
            return;
        }

        float regenRate = IsExhausted ? exhaustedStaminaRegenPerSecond : staminaRegenPerSecond;
        float before = currentStamina;
        currentStamina = Mathf.Min(MaxStamina, currentStamina + regenRate * Time.deltaTime);

        if (IsExhausted && currentStamina >= MaxStamina * exhaustedRecoverThreshold) IsExhausted = false;

        if (!Mathf.Approximately(before, currentStamina)) OnStatsChanged?.Invoke();
    }

    public void RecalculateStats()
    {
        if (InventoryManager.Instance == null) return;

        float bonusHealth = 0f, bonusStamina = 0f, bonusShield = 0f, bonusAttack = 0f;

        foreach (var slot in InventoryManager.Instance.slots)
        {
            if (slot.item == null) continue;

            switch (slot.item.type)
            {
                case ItemType.Vida: bonusHealth += slot.item.statValue * slot.quantity; break;
                case ItemType.Estamina: bonusStamina += slot.item.statValue * slot.quantity; break;
                case ItemType.Escudo: bonusShield += slot.item.statValue * slot.quantity; break;
                case ItemType.Ataque: bonusAttack += slot.item.statValue * slot.quantity; break;
            }
        }

        float oldMaxHealth = MaxHealth;
        float oldMaxStamina = MaxStamina;
        float oldMaxShield = MaxShield;

        MaxHealth = baseMaxHealth + bonusHealth;
        MaxStamina = baseMaxStamina + bonusStamina;
        MaxShield = baseMaxShield + bonusShield;
        AttackMultiplier = baseAttackMultiplier + bonusAttack;

        currentHealth = Mathf.Clamp(currentHealth + (MaxHealth - oldMaxHealth), 0, MaxHealth);
        currentStamina = Mathf.Clamp(currentStamina + (MaxStamina - oldMaxStamina), 0, MaxStamina);
        currentShield = Mathf.Clamp(currentShield + (MaxShield - oldMaxShield), 0, MaxShield);

        OnStatsChanged?.Invoke();
    }

    public void ApplySavedValues(float health, float stamina, float shield)
    {
        RecalculateStats();

        currentHealth = Mathf.Clamp(health, 0f, MaxHealth);
        currentStamina = Mathf.Clamp(stamina, 0f, MaxStamina);
        currentShield = Mathf.Clamp(shield, 0f, MaxShield);
        IsExhausted = false;
        IsDead = currentHealth <= 0f;

        OnStatsChanged?.Invoke();
    }

    public void FullRestore()
    {
        currentHealth = MaxHealth;
        currentStamina = MaxStamina;
        currentShield = MaxShield;
        IsExhausted = false;
        IsDead = false;
        OnStatsChanged?.Invoke();
    }

    public void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f) return;

        float remaining = amount;

        if (currentShield > 0f)
        {
            float absorbed = Mathf.Min(currentShield, remaining);
            currentShield -= absorbed;
            remaining -= absorbed;
        }

        currentHealth = Mathf.Clamp(currentHealth - remaining, 0, MaxHealth);
        OnStatsChanged?.Invoke();

        if (currentHealth <= 0f)
        {
            IsDead = true;
            OnDied?.Invoke();
        }
    }

    public bool TrySpendStamina(float amount)
    {
        if (!staminaEnabled) return true;

        if (currentStamina < amount) return false;
        currentStamina -= amount;
        staminaSpentThisFrame = true;

        if (currentStamina <= 0f)
        {
            currentStamina = 0f;
            IsExhausted = true;
        }

        OnStatsChanged?.Invoke();
        return true;
    }
    public void Respawn()
    {

        transform.position = islandPosition.position;
        Physics.SyncTransforms();
        Debug.Log("Rodei aq");
    }
}