using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IslandActive : MonoBehaviour
{
    [SerializeField]
    private GameObject enemySpawn, islandManager;
   
    public static IslandActive islandActive;
    [SerializeField]
    private BoxCollider col;

    void Awake()
    {
        islandActive = this;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemySpawn.SetActive(true);
            islandManager.SetActive(true);
            col.enabled = false;
        }
    }
}
