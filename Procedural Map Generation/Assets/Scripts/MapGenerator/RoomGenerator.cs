using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomGenerator : RandomWalkGenerator
{
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField]
    [Range(0, 10)]
    private int offset = 1;
    [SerializeField]
    private bool randomWalkRooms = false;


    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }
    private void CreateRooms()
    {

        var roomsList = ProceduralGeneration.BinarySpacePartitioning(new BoundsInt(startPosition,
        new Vector3Int(dungeonWidth, 0, dungeonHeight)), minRoomWidth, minRoomHeight);

        HashSet<Vector3Int> floor = new HashSet<Vector3Int>();
        floor = CreateSimpleRooms(roomsList);

        //floor = 

        List<Vector3Int> roomCenters = new List<Vector3Int>();
        foreach (var room in roomsList)
        {
            roomCenters.Add(Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector3Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);
        prefabVisualizer.Clear();
        prefabVisualizer.CreateFloorPrefabs(floor);
        WallGenerator.CreateWalls(floor, prefabVisualizer);
    }

    private HashSet<Vector3Int> ConnectRooms(List<Vector3Int> roomCenters)
    {
        HashSet<Vector3Int> corridors = new HashSet<Vector3Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];

        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector3Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);

            HashSet<Vector3Int> newCorridor = CreateCorridor(currentRoomCenter, closest);

            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    private HashSet<Vector3Int> CreateCorridor(Vector3Int currentRoomCenter, Vector3Int destination)
    {
        HashSet<Vector3Int> corridor = new HashSet<Vector3Int>();

        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.z != destination.z)
        {
            if (destination.z > position.z)
            {
                position += Vector3Int.forward;
            }
            else if (destination.z < position.z)
            {
                position += Vector3Int.back;
            }
            corridor.Add(position);
        }
        while (position.x != destination.x)
        {
            if(destination.x > position.x)
            {
                position += Vector3Int.right;
            }
            else if( destination.x < position.x)
            {
                position += Vector3Int.left;
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

    private HashSet<Vector3Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector3Int> floor = new HashSet<Vector3Int>();

        foreach (var room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.z - offset; row++)
                {
                    Vector3Int position = room.min + new Vector3Int(col, 0, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }
}
