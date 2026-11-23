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
      
        
        for(int i = 0; i<count; i++)
        {
            var circle = Random.insideUnitSphere * radius;
            circle.y = 0;
            var rd = Random.Range(0, enemy.Length); 
            var pos =  spawnPoints[Random.Range(0,spawnPoints.Length)].position + circle;
           
            islandManager.enemies.Add(Instantiate(enemy[rd], pos,Quaternion.identity, transform));
            
        }
        var rdLeader = Random.Range(0, islandManager.enemies.Count); 
        islandManager.enemies[rdLeader].isLeader = true;
        islandManager.CloseDoors();
        islandManager.combatStarted = true;
    }

 
}
