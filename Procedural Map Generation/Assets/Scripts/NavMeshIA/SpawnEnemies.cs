using Unity.VisualScripting;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [SerializeField] private Agent[] enemy;
    [SerializeField] private int min;
    [SerializeField] private int max;
    [SerializeField] private float radius;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField]IslandManager islandManager;
    void Start()
    {
        SpawnEnemy();
    }
    void SpawnEnemy()
    {
        
        var count = Random.Range(min , max);
        var rd = Random.Range(0, enemy.Length);
        enemy[rd].isLeader = true;
        for(int i = 0; i<count; i++)
        {
            var circle = Random.insideUnitSphere * radius;
            circle.y = 0;

            var pos =  spawnPoints[Random.Range(0,spawnPoints.Length)].position + circle;
            if(i == 1) enemy[rd].isLeader = false;
            islandManager.enemies.Add(Instantiate(enemy[rd].gameObject, pos,Quaternion.identity, transform));
            
        }
        islandManager.CloseDoors();
        islandManager.combatStarted = true;
    }

 
}
