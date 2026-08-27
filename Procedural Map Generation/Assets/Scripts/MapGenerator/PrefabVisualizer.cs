using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

using UnityEngine.UIElements;

public class PrefabVisualizer : MonoBehaviour
{
    [SerializeField]
    private GameObject floorPrefab, wallTop;

    private List<GameObject> path = new List<GameObject>();

    public void CreateFloorPrefabs(IEnumerable<Vector3Int> floorPositions)
    {
        CreateFloor(floorPositions, floorPrefab);
    }

    private void CreateFloor(IEnumerable<Vector3Int> floorPositions, GameObject floorPrefab)
    {
        foreach (var position in floorPositions)
        {
            path.Add(Instantiate(floorPrefab, position, Quaternion.identity));
        }
    }
    private void CreateSingleFloor(GameObject prefab, Vector3Int position)
    {
        path.Add(Instantiate(prefab, position, Quaternion.identity));
    }
    public void Clear()
    {
        for (int i = 0; i < path.Count; i++)
        {

            Destroy(path[i]);

        }
        path.Clear();

    }

    internal void PaintSingleBasicWall(Vector3Int position)
    {
        CreateSingleFloor(wallTop, position);
    }
}
