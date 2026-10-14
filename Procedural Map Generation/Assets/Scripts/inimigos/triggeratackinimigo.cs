using UnityEngine;

public class triggeratackinimigo : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerAgent>().dano(5);
            Debug.Log("atack saiu  " + other.GetComponent<PlayerAgent>().vida);
        }
    }
}
