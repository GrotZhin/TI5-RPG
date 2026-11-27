
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIslandManager : IslandManager
{

    public static BossIslandManager bossIslandManager;
    [SerializeField] Animator anim;
    [SerializeField] GameObject elevator;
   
    int waveCount = 0;
    
    void Awake()
    {
        bossIslandManager = this;
    }
    

    void LateUpdate()
    {
        if (enemies.Count <= 0 && combatStarted)
        {
            if (waveCount < 4)
            {
                SpawnEnemies.SpawnEnemy();

            }
            else
            {
                CallElevator();
                OpenDoor();
            }
        }
    }

    public void CallElevator()
    {
        elevator.SetActive(true);
    }
}




