using System;
using System.Collections;
using System.Collections.Generic;
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
    public void CloseDoors()
    {
        Debug.Log("closeDoors");
        foreach (var item in island.doors)
        {
            if(item == null) continue;
            if(!item.isOpen) continue;
            
            item.isOpen = false;
            
            item.mesh.SetActive(true);
            item.animator.Play("DoorLock", 0, 0f);
        }
    }

    private void OpenDoor()
    {
        foreach (var item in island.doors)
        {
            if(item == null) continue;
            if(item.isOpen) continue;
            item.isOpen = true;
            item.pair.isOpen = true;
            item.animator.Play("DoorUnlock",0,0f);
            item.pair.animator.Play("DoorUnlock",0,0f);

            StartCoroutine(DisableMesh(item.animator,item));
        }
    }
    IEnumerator DisableMesh(Animator animator, DoorPos item)
    {
        var stage = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitUntil(()=> stage.normalizedTime >= 1); 
          item.mesh.SetActive(false);
            item.pair.mesh.SetActive(false);
    }

    
}
