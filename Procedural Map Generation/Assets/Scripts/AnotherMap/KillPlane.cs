using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class KillPlane : MonoBehaviour
{
  
    void OnTriggerEnter(Collider other)
    {

        if(other.CompareTag("Enemy"))
        IslandManager.islandManager.enemies.Remove(other.GetComponent<Agent>());
        if (other.CompareTag("Player"))
        {
            PlayerStats.Instance.TakeDamage(10);
            PlayerStats.Instance.Respawn();
            
        }
       
    }

}
