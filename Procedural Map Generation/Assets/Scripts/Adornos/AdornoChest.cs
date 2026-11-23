using UnityEngine;

public class AdornoChest : MonoBehaviour, IInteractable
{
    [Header("Loot")]
    public AdornoLootTable lootTable;
    public AdornoPickup pickupPrefab;
    public Transform dropPoint;

    [Header("Texto")]
    public string actionLabel = "abrir";

    [Header("Estado")]
    public bool opened;
    [Tooltip("Opcional: animator com trigger 'Open'.")]
    public Animator animator;

    public void Interact(GameObject player)
    {
        if (opened || lootTable == null) return;

        AdornoData adorno = lootTable.Roll();
        if (adorno == null)
        {
            Debug.LogWarning("[AdornoChest] LootTable vazia ou sem adornos válidos.", this);
            return;
        }

        opened = true;
        if (animator != null) animator.SetTrigger("Open");

        if (pickupPrefab != null)
        {
            Vector3 pos;
            if (dropPoint != null) pos = dropPoint.position;
            else
            {
                Vector3 toPlayer = player != null ? player.transform.position - transform.position : Vector3.zero;
                toPlayer.y = 0f;
                pos = transform.position + Vector3.up * 1.1f + (toPlayer.sqrMagnitude > 0.01f ? toPlayer.normalized * 0.8f : Vector3.zero);
            }
            AdornoPickup p = Instantiate(pickupPrefab, pos, Quaternion.identity);
            p.Setup(adorno);
        }
        else
        {
            AdornoPickup.Give(adorno);
        }
    }

    public string GetInteractionPrompt() => opened ? "" : $"Pressione E para {actionLabel}";
}
