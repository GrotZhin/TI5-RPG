using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    [Tooltip("ID único. Aparece no slot de save e na mensagem de checkpoint.")]
    public string checkpointId = "Checkpoint";
    public bool showMessage = true;

    string Id => string.IsNullOrEmpty(checkpointId) ? name : checkpointId;

    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
        checkpointId = name;
    }

    void OnTriggerEnter(Collider other)
    {
        if (SaveManager.Instance == null) return;
        PlayerMove player = other.GetComponentInParent<PlayerMove>();
        if (player == null) return;
        if (SaveManager.Instance.CurrentCheckpointId == Id) return;

        SaveManager.Instance.SetCheckpoint(Id, player.transform.position, player.transform.eulerAngles.y);

        if (showMessage && FeedbackPopup.Instance != null) FeedbackPopup.Instance.Show($"Checkpoint: {Id}", true);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.8f);
        Collider c = GetComponent<Collider>();
        if (c != null) Gizmos.DrawWireCube(c.bounds.center, c.bounds.size);
    }
}