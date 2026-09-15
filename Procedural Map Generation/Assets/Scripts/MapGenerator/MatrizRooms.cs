using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class MatrizRooms : RandomWalkGenerator
{
    [SerializeField]
    private int height;
    [SerializeField]
    private int witdth;
    [SerializeField]
    private int roomCount;
    public List<RoomStats> island;
    [SerializeField]
    LayerMask layerMask;
    [SerializeField]
    Vector3Int socorro1, socorro2;

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        var roomsSorted = ProceduralGeneration.SortRooms(roomCount, island);
        var map = ProceduralGeneration.CreateMatrizRooms(height, witdth, roomCount, roomsSorted,layerMask);

        var i = 0;

        foreach (var pos in map)
        {

            var obj = Instantiate(roomsSorted[i].gameObject, pos, Quaternion.identity);
          

            i++;
        }
    }
    



}
