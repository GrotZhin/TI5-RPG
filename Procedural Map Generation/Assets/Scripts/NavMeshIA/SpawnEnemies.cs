using Unity.VisualScripting;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [SerializeField] private GameObject[] enemy;
    [SerializeField] private int min;
    [SerializeField] private int max;
    [SerializeField] private float radius;
    [SerializeField] private Transform[] spawnPoints;
    void Start()
    {
        SpawnEnemy();
    }
    void SpawnEnemy()
    {
        var count = Random.Range(min , max);
        for(int i = 0; i<count; i++)
        {
            int R = Random.Range(0, enemy.Length);
            var circle = Random.insideUnitSphere * radius;
            circle.y = 0;

            var pos =  spawnPoints[Random.Range(0,spawnPoints.Length)].position + circle;
            Instantiate(enemy[R], pos,Quaternion.identity, transform);
        }
    }

 
}
