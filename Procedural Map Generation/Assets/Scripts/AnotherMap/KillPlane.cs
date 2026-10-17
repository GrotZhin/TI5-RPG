using UnityEngine;

public class KillPlane : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        other.gameObject.SetActive(false);
        MapGM.mapGM.enemies.Remove(other.gameObject);
    }
}
