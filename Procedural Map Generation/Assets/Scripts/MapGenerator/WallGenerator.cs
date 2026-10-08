using System;
using System.Collections.Generic;

using UnityEngine;
using static ProceduralGeneration;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector3Int> floorPosition, PrefabVisualizer prefabVisualizer)
    {
        var basicWallPositions = FindWallDirections(floorPosition, Direction2D.cardinalDirectionList);
        foreach (var position in basicWallPositions)
        {
            prefabVisualizer.PaintSingleBasicWall(position);
        }
    }

    private static HashSet<Vector3Int> FindWallDirections(HashSet<Vector3Int> floorPosition, List<Vector3Int> directionsList)
    {
        HashSet<Vector3Int> wallPositions = new HashSet<Vector3Int>();

        foreach (var position in floorPosition)
        {
            foreach (var direction in directionsList)
            {
                var neighbourPosition = position + direction;
                if(floorPosition.Contains(neighbourPosition) == false)
                    wallPositions.Add(neighbourPosition);
            }
        }
        return wallPositions;
    }
}
