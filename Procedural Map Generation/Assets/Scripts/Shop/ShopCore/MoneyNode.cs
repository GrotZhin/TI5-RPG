using UnityEngine;
public class MoneyNode : MonoBehaviour, IInteractable
{
    [Header("Recompensa")]
    public int baseCoins = 10;
    public float cooldown = 2f;

    [Header("Texto")]
    public string actionLabel = "coletar";

    private float lastHitTime = 0;

    public void Interact(GameObject player)
    {
        if (Time.time - lastHitTime < cooldown)
        {
            if (FeedbackPopup.Instance != null) FeedbackPopup.Instance.Show("Aguarde um pouco...", false);
            return;
        }
        lastHitTime = Time.time;

        float multiplier = PlayerStats.Instance != null ? PlayerStats.Instance.AttackMultiplier : 1f;
        int amount = Mathf.Max(1, Mathf.RoundToInt(baseCoins * multiplier));

        if (PlayerWallet.Instance != null) PlayerWallet.Instance.AddCoins(amount);

        if (FeedbackPopup.Instance != null) FeedbackPopup.Instance.Show($"+{amount} moedas!", true);
    }

    public string GetInteractionPrompt()
    {
        return $"Pressione E para {actionLabel}";
    }
}