using System;
using System.Collections.Generic;
using System.IO.Compression;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
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
        List<Vector3Int> doors = new List<Vector3Int>();
        var map = ProceduralGeneration.CreateMatrizRooms(height, witdth, roomCount, roomsSorted, layerMask);
        //HashSet<Vector3Int> corridors = ConnectRooms(doors);
        
        var i = 0;

        foreach (var pos in map)
        {

            var obj = Instantiate(roomsSorted[i].gameObject, pos, Quaternion.identity);

            i++;
        }
       // prefabVisualizer.CreateFloorPrefabs(corridors);
      
    }

    private HashSet<Vector3Int> ConnectRooms(List<Vector3Int> doors)
    {
        HashSet<Vector3Int> corridors = new HashSet<Vector3Int>();
        var currentDoor = doors[Random.Range(0, doors.Count)];

        doors.Remove(currentDoor);

        while (doors.Count > 0)
        {
            Vector3Int closest = FindClosestPointTo(currentDoor, doors);
            doors.Remove(closest);

            HashSet<Vector3Int> newCorridor = CreateCorridor(currentDoor, closest);

            currentDoor = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    // private HashSet<Vector3Int> CreateCorridor(Vector3Int currentDoor, Vector3Int destination)
    // {
    //     HashSet<Vector3Int> corridor = new HashSet<Vector3Int>();
    //     var position = currentDoor;
    //     while (destination.x + destination.z != currentDoor.x + currentDoor.z)
    //     {
    //         if (Random.value > 0.5f)
    //         {
    //             if (currentDoor.z > destination.z)
    //                 position += Vector3Int.back;
    //             else if (currentDoor.z < destination.z)
    //                 position += Vector3Int.forward;
    //         }
    //         else
    //         {
    //             if (currentDoor.x > destination.x)
    //                 position += Vector3Int.left;
    //             else if (currentDoor.x < destination.x)
    //                 position += Vector3Int.right;
    //         }
    //         corridor.Add(position);
    //     }
    //     return corridor;

    // }
    private HashSet<Vector3Int> CreateCorridor(Vector3Int currentRoomCenter, Vector3Int destination)
    {
        HashSet<Vector3Int> corridor = new HashSet<Vector3Int>();

        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.z != destination.z)
        {
            if (destination.z > position.z)
            {
                position += Vector3Int.forward ;
            }
            else if (destination.z < position.z)
            {
                position += Vector3Int.back ;
            }
            corridor.Add(position);
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector3Int.right ;
            }
            else if (destination.x < position.x)
            {
                position += Vector3Int.left ;
            }
            corridor.Add(position);
        }
        return corridor;
    }
    private Vector3Int FindClosestPointTo(Vector3Int currentRoomCenter, List<Vector3Int> roomCenters)
    {
        Vector3Int closest = Vector3Int.zero;
        float distance = float.MaxValue;

        foreach (var position in roomCenters)
        {
            float currentDistance = Vector3Int.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }




}
