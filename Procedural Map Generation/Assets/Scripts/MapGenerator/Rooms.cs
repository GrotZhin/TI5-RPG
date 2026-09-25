using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rooms : RandomWalkGenerator
{
    [SerializeField]
    private List<RoomStats> rooms;
    
    [SerializeField]
    private int roomCount;

    [SerializeField]
    private Vector3Int offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    protected override void RunProceduralGeneration()
    {
       Create();
    }
    void Create()
    {

        var roomsPos = ProceduralGeneration.CreateRooms(rooms, startPosition, offset, roomCount);
        List<Vector3Int> roomCenters = new List<Vector3Int>();
        foreach (var room in roomsPos)
        {
            roomCenters.Add(Vector3Int.RoundToInt(room.center));
            Instantiate(rooms[0].gameObject, room.position, Quaternion.identity);
        }
       
        HashSet<Vector3Int> corridors = ConnectRooms(roomCenters);
        
        prefabVisualizer.CreateFloorPrefabs(corridors);
        WallGenerator.CreateWalls(corridors, prefabVisualizer);
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
    // private HashSet<Vector3Int> CreateCorridor(Vector3Int currentRoomCenter, Vector3Int destination)
    // {
    //     HashSet<Vector3Int> corridor = new HashSet<Vector3Int>();

    //     var position = currentRoomCenter;
    //     corridor.Add(position);
    //     while (position.z != destination.z)
    //     {
    //         if (destination.z > position.z)
    //         {
    //             position += Vector3Int.forward ;
    //         }
    //         else if (destination.z < position.z)
    //         {
    //             position += Vector3Int.back ;
    //         }
    //         corridor.Add(position);
    //     }
    //     while (position.x != destination.x)
    //     {
    //         if (destination.x > position.x)
    //         {
    //             position += Vector3Int.right ;
    //         }
    //         else if (destination.x < position.x)
    //         {
    //             position += Vector3Int.left ;
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
