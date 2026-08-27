using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomWalkGenerator : AbstractMapGenerator
{
   
    [SerializeField]
    protected RandomWalkData randomWalkData;
   
    
    protected override void RunProceduralGeneration()
    {
        HashSet<Vector3Int> floorPosition = RunRandomWalk(randomWalkData,startPosition);
       prefabVisualizer.Clear();
        prefabVisualizer.CreateFloorPrefabs(floorPosition );
        WallGenerator.CreateWalls(floorPosition, prefabVisualizer);
    }

    protected HashSet<Vector3Int> RunRandomWalk(RandomWalkData parameters, Vector3Int position)
    {
        var currentPosition = position;
        HashSet<Vector3Int> floorPositions = new HashSet<Vector3Int>();

        for (int i = 0; i < parameters.iterations; i++)
        {
            var path = ProceduralGeneration.SimpleRandomWalk(currentPosition, parameters.walkLenght);
            floorPositions.UnionWith(path);

            if(parameters.startRandomlyEachIterations)
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));

        }
        return floorPositions;
    }

   
}