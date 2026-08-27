using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CorridorFirstDungeonGenerator : RandomWalkGenerator
{
    [SerializeField]
    private int corridorLength = 14, corridorCount = 5;
    [SerializeField]
    [Range(0.1f,1)]
    private float roomPercent = 0.8f;
    
    protected override void RunProceduralGeneration()
    {
        prefabVisualizer.Clear();
        CorridorFirstGeneration();
        
    }

    private void CorridorFirstGeneration()
    {
        HashSet<Vector3Int> floorPosition = new HashSet<Vector3Int>();
        HashSet<Vector3Int> potentialRoomPositions = new HashSet<Vector3Int>();

        CreateCorridors(floorPosition, potentialRoomPositions);

        HashSet<Vector3Int> roomPosition = CreateRooms(potentialRoomPositions);

        List<Vector3Int> deadEnds = FindAllDeadEnds(floorPosition);

        CreateRoomsAtDeadEnds(deadEnds, roomPosition);

        floorPosition.UnionWith(roomPosition);

        prefabVisualizer.CreateFloorPrefabs(floorPosition);

        WallGenerator.CreateWalls(floorPosition, prefabVisualizer);
    }

    private void CreateRoomsAtDeadEnds(List<Vector3Int> deadEnds, HashSet<Vector3Int> roomFloors)
    {
        foreach (var position in deadEnds)
        {
            if(roomFloors.Contains(position) == false)
            {
                var room = RunRandomWalk(randomWalkData, position);
                roomFloors.UnionWith(room);
            }
        }
    }

    private List<Vector3Int> FindAllDeadEnds(HashSet<Vector3Int> floorPosition)
    {
        List<Vector3Int> deadEnds = new List<Vector3Int>();
        foreach (var position in floorPosition)
        {
            int neighbourCount = 0;
            foreach (var direction in Direction2D.cardinalDirectionList)
            {
                if(floorPosition.Contains(position + direction))
                neighbourCount++;
                
            }
            if(neighbourCount == 1)
                deadEnds.Add(position);
        }
        return deadEnds;
    }

    private HashSet<Vector3Int> CreateRooms(HashSet<Vector3Int> potentialRoomPositions)
    {
        HashSet<Vector3Int> roomPosition = new HashSet<Vector3Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);

        List<Vector3Int> roomToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

        foreach (var room in roomToCreate)
        {
            var roomFloor = RunRandomWalk(randomWalkData, room);

            roomPosition.UnionWith(roomFloor);
        }
        return roomPosition;
    }

    private void CreateCorridors(HashSet<Vector3Int> floorPosition, HashSet<Vector3Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);

        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGeneration.RandomWalkCorridor(currentPosition, corridorLength);
            currentPosition = corridor[corridor.Count-1];
            potentialRoomPositions.Add(currentPosition);
            floorPosition.UnionWith(corridor);
        }
    }
}
