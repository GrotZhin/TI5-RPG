
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandManager : MonoBehaviour
{

    public List<Agent> enemies;

    public int islandId = 0;
    public static IslandManager islandManager;

    [SerializeField]
    private DoorsInScene island;
    [SerializeField]
    MeshCollider wallCol;
    [SerializeField]
    bool firstIsle = false;
    public bool combatStarted = false;
    [SerializeField] SpawnEnemies spawnEnemies;
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
            if(AnotherWave())
            spawnEnemies.SpawnEnemy();
            else
            OpenDoor();
        }
    }
    public void CloseDoors()
    {
        Debug.Log("closeDoors");
        foreach (var item in island.doors)
        {
            if (item == null) continue;
            if (!item.isOpen) continue;

            item.isOpen = false;

            item.mesh.SetActive(true);
            item.animator.Play("DoorLock", 0, 0f);
        }
        if (wallCol != null) wallCol.enabled = true;
    }
    [ContextMenu("OpenDoor")]
    private void OpenDoor()
    {

        foreach (var item in island.doors)
        {
            if (item == null) continue;
            if (item.isOpen) continue;
            item.isOpen = true;
            item.pair.isOpen = true;
            item.animator.Play("DoorUnlock", 0, 0f);


            StartCoroutine(DisableMesh(item.animator, item));
        }
        if (wallCol != null) wallCol.enabled = false;
    }
    IEnumerator DisableMesh(Animator animator, DoorPos item)
    {
        yield return new WaitUntil(() =>
        animator.GetCurrentAnimatorStateInfo(0).IsName("DoorUnlock"));

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        item.mesh.SetActive(false);
        item.pair.mesh.SetActive(false);
    }

    public bool AnotherWave()
    {
        bool rd = Random.value >  0.5f;
        return rd;
    }

}
