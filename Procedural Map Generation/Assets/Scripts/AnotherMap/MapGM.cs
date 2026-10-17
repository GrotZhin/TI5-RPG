using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapGM : MonoBehaviour
{
    [SerializeField]
    private GameObject enemySpawn;
  
    List<EnemyAgent> enemies;
    public List<GameObject> doors;
    public int islandId = 0;
    public static MapGM mapGM;
    bool playeyOnIsle = false;
    [SerializeField]
    bool firstIsle = false;
    void Awake()
    {
        mapGM = this;
    }
    void Start()
    {
        if(firstIsle) OpenDoor();
    }

    void Update()
    {
        // if(enemies.Count <= 0 && playeyOnIsle)
        // {
        //     OpenDoor();
        // }
    }

    private void OpenDoor()
    {
        foreach (var item in doors)
        {
            item.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            enemySpawn.SetActive(true);
            playeyOnIsle = true;
        }
    }
}
