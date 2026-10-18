using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsHUD : MonoBehaviour
{
    [Header("Objeto raiz da HUD (some ao abrir diálogo/loja/inventário)")]
    public GameObject hudRoot;

    [Header("Barras")]
    public Image healthBar;
    public Image staminaBar;
    public Image shieldBar;
    public Image attackBar;

    [Header("Textos (opcional)")]
    public TMP_Text healthText;
    public TMP_Text staminaText;
    public TMP_Text shieldText;
    public TMP_Text attackText;
    public TMP_Text coinsText;
    public float attackBarReferenceMax = 3f;

    [Header("Cores da barra de estamina por faixa")]
    public Color staminaNormalColor = new Color(0.25f, 0.65f, 1f);
    public Color staminaWarningColor = Color.yellow;
    public Color staminaCriticalColor = Color.red;
    [Range(0f, 1f)] public float staminaWarningThreshold = 0.5f;
    [Range(0f, 1f)] public float staminaCriticalThreshold = 0.15f;

    void OnDisable()
    {
        if (PlayerStats.Instance != null) PlayerStats.Instance.OnStatsChanged -= Refresh;
        if (PlayerWallet.Instance != null) PlayerWallet.Instance.OnCoinsChanged -= UpdateCoinsText;
    }

    void Start()
    {
        if (PlayerStats.Instance != null) PlayerStats.Instance.OnStatsChanged += Refresh;
        if (PlayerWallet.Instance != null) PlayerWallet.Instance.OnCoinsChanged += UpdateCoinsText;

        Refresh();
        if (PlayerWallet.Instance != null) UpdateCoinsText(PlayerWallet.Instance.coins);
    }

    void UpdateCoinsText(int amount)
    {
        if (coinsText != null) coinsText.text = $"{amount} moedas";
    }

    void Update()
    {
        if (hudRoot != null && hudRoot.activeSelf == GameUIState.IsUIOpen) hudRoot.SetActive(!GameUIState.IsUIOpen);
    }

    void Refresh()
    {
        var stats = PlayerStats.Instance;
        if (stats == null) return;

        if (healthBar != null) healthBar.fillAmount = stats.MaxHealth > 0f ? stats.currentHealth / stats.MaxHealth : 0f;
        if (healthText != null) healthText.text = $"{Mathf.RoundToInt(stats.currentHealth)}/{Mathf.RoundToInt(stats.MaxHealth)}";

        if (staminaBar != null)
        {
            float staminaPct = stats.MaxStamina > 0f ? stats.currentStamina / stats.MaxStamina : 0f;
            staminaBar.fillAmount = staminaPct;

            if (staminaPct <= staminaCriticalThreshold) staminaBar.color = staminaCriticalColor;
            else if (staminaPct <= staminaWarningThreshold) staminaBar.color = staminaWarningColor;
            else staminaBar.color = staminaNormalColor;
        }
        if (staminaText != null) staminaText.text = $"{Mathf.RoundToInt(stats.currentStamina)}/{Mathf.RoundToInt(stats.MaxStamina)}";

        if (shieldBar != null) shieldBar.fillAmount = stats.MaxShield > 0f ? stats.currentShield / stats.MaxShield : 0f;
        if (shieldText != null) shieldText.text = $"{Mathf.RoundToInt(stats.currentShield)}/{Mathf.RoundToInt(stats.MaxShield)}";

        if (attackBar != null) attackBar.fillAmount = Mathf.Clamp01(stats.AttackMultiplier / attackBarReferenceMax);
        if (attackText != null) attackText.text = $"x{stats.AttackMultiplier:0.0}";
    }
}