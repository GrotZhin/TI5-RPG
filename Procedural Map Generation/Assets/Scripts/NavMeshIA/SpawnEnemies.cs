using Unity.VisualScripting;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [SerializeField] private Agent[] enemy;
    [SerializeField] private int min;
    [SerializeField] private int max;
    [SerializeField] private float radius;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] IslandManager islandManager;
    void Start()
    {
        SpawnEnemy();
    }
    public void SpawnEnemy()
    {

        var count = Random.Range(min, max);


        for (int i = 0; i < count; i++)
        {
            var circle = Random.insideUnitSphere * radius;
            circle.y = 0;
            var rd = Random.Range(0, enemy.Length);
            var pos = spawnPoints[Random.Range(0, spawnPoints.Length)];

            islandManager.enemies.Add(Instantiate(enemy[rd], pos.position + circle, Quaternion.identity, pos));

        }
        foreach (var enemy in spawnPoints )
        {
            var group = enemy.GetComponent<GroupFormation>();
           
           group.SetLeader();
        }
        
        islandManager.CloseDoors();
        islandManager.combatStarted = true;
    }


}
