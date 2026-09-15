using UnityEngine;

public class EnemyAgentManager : MonoBehaviour
{
        public GameObject enemyPrefab;
        public int quantidade = 5;

        public Transform[] spawnPoints;

        public bool Seek = true;
        public bool Separate = true;
        public bool Align = true;
        public bool Cohesion = true;
        public bool Avoid = true;

        void Start()
        {
            SpawnEnemy();
        }

        public void SpawnEnemy()
        {
            if (enemyPrefab == null)
            {
                Debug.LogError("Nenhum prefab definido!");
                return;
            }

            if (spawnPoints.Length == 0)
            {
                Debug.LogError("Nenhum Spawnpoint definido!");
                return;
            }

            for (int i = 0; i < quantidade; i++)
            {
                Transform spawnPoint = spawnPoints[i % spawnPoints.Length];

                GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

                EnemyAgent agent = enemy.GetComponent<EnemyAgent>();

                if (agent != null)
                {
                    Config(agent);
                }
                else
                {
                    Debug.LogError("O prefab não possui EnemyAgent!");
                }
            }
        }

        public void Config(EnemyAgent enemy)
        {
            enemy.control.seek = Seek ? 0.8f : 0f;
            enemy.control.separate = Separate ? 0.85f : 0f;
            enemy.control.align = Align ? 0.01f : 0f;
            enemy.control.cohesion = Cohesion ? 0.03f : 0f;
            enemy.control.avoid = Avoid ? 1f : 0f;
            Vector3 dirI = Random.insideUnitSphere;
            dirI.y = 0;
            enemy.transform.forward = dirI;
        }
}
