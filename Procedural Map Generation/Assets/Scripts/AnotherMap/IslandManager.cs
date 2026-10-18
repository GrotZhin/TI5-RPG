using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IslandManager : MonoBehaviour
{
   
    public List<GameObject> enemies;
   
    public int islandId = 0;
    public static IslandManager islandManager;
   
    [SerializeField]
    private DoorsInScene island;
    [SerializeField]
    bool firstIsle = false;
    public bool combatStarted = false;
    void Awake()
    {
        islandManager = this;
    }
    void Start()
    {
          if (firstIsle) OpenDoor();
    }

    void LateUpdate()
    {
        if (enemies.Count <= 0 && combatStarted)
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        foreach (var item in island.doors)
        {
            if(item == null) continue;
            item.gameObject.SetActive(false);
            item.pair.gameObject.SetActive(false);
        }
    }

    
}
