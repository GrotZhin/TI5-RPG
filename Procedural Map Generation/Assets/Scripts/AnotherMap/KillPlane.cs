using UnityEngine;

public class KillPlane : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        other.gameObject.SetActive(false);
        IslandManager.islandManager.enemies.Remove(other.gameObject);
    }
}
