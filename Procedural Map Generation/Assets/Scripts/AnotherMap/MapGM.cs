using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapGM : MonoBehaviour
{
    [SerializeField]

    private GameObject enemySpawn;
    [SerializeField]
    BoxCollider col;
    public List<GameObject> enemies;
    public List<DoorPos> doors;
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
        if (firstIsle) OpenDoor();
    }

    void FixedUpdate()
    {
        if (enemies.Count <= 0 && playeyOnIsle)
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        foreach (var item in doors)
        {
            if(item == null) continue;
            item.gameObject.SetActive(false);
            item.pair.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemySpawn.SetActive(true);
            playeyOnIsle = true;
            col.enabled = false;
        }
    }
}
