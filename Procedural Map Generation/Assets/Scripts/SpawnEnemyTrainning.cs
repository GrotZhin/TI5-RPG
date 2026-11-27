using UnityEngine;

public class SpawnEnemyTrainning : MonoBehaviour
{
    public GameObject prefab;
    public Transform spawnPoint;


    private void OnTriggerEnter(Collider other)
    {

            Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

    }
}