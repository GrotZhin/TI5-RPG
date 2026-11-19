using System.Collections;
using UnityEngine;

public class KillPlane : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {

        if(other.CompareTag("Enemy"))
        IslandManager.islandManager.enemies.Remove(other.gameObject);
        if (other.CompareTag("Player"))
        {
            other.gameObject.SetActive(false);
            PlayerStats.Instance.Respawn();
            StartCoroutine(TurnOn(other.gameObject)); 
        }
       
    }
     IEnumerator TurnOn(GameObject obj)
        {
            yield return new WaitForSeconds(0.1f);
            obj.SetActive(true);
        }
}
