using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ProceduralGeneration
{
    public static HashSet<Vector3Int> SimpleRandomWalk(Vector3Int startPosition, int walkLenght)
    {
        HashSet<Vector3Int> path = new HashSet<Vector3Int>();
        path.Add(startPosition);

        var previousPosition = startPosition;

        for (int i = 0; i < walkLenght; i++)
        {
            var newPosition = previousPosition + Direction2D.GetRandomDirection();
            path.Add(newPosition);
            previousPosition = newPosition;
        }
        return path;
    }

    public static List<Vector3Int> RandomWalkCorridor(Vector3Int startPosition, int corridorLength)
    {
        List<Vector3Int> corridor = new List<Vector3Int>();
        var direction = Direction2D.GetRandomDirection();
        var currentPosition = startPosition;
        corridor.Add(currentPosition);
        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }
        return corridor;
    }
    public static HashSet<Vector3Int> CreateRooms( List<BoundsInt> rooms, Vector3Int startPosition,Vector3Int offset )
    {
        HashSet<Vector3Int> roomsPosition = new HashSet<Vector3Int>();
        var currentPosition = startPosition;
        while (rooms.Count > 0)
        {
            foreach (var position in rooms )
            {
               roomsPosition.Add(currentPosition);
               if(Random.value < 0.5f)
               currentPosition = currentPosition + position.min + offset;
               else  
               currentPosition = currentPosition + position.max + offset;

            }
            
        }
        return roomsPosition;
    }
    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();

        roomsQueue.Enqueue(spaceToSplit);
        while (roomsQueue.Count > 0)
        {
            //lembrar de trocar esses Ys para Z quando for mudar para 3D
            var room = roomsQueue.Dequeue();
            if (room.size.z >= minHeight && room.size.x >= minWidth)
            {
                if (Random.value < 0.5f)
                {
                    if (room.size.z >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth && room.size.z >= minHeight)
                    {
                        roomsList.Add(room);
                    }
                }
                else
                {

                    if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    else if (room.size.z >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth && room.size.z >= minHeight)
                    {

                        roomsList.Add(room);
                    }
                }
            }
        }
        return roomsList;
    }

    private static void SplitVertically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        //var xSplit = Random.Range(minWidth, room.size.x - minWidth);
        var xSplit = Random.Range(1, room.size.x);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.min.y, room.min.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
        new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));

        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        //var zSplit = Random.Range(minHeight, room.size.z - minHeight);
        var zSplit = Random.Range(1, room.size.z);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, room.size.y, zSplit));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.size.y ,room.min.z + zSplit),
        new Vector3Int(room.size.x, room.size.y , room.size.z - zSplit));

        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);

    }
}

public static class Direction2D
{
    public static List<Vector3Int> cardinalDirectionList = new List<Vector3Int>
        {
            new Vector3Int(0,0,1), //UP
            new Vector3Int(1,0,0), //RIGHT
            new Vector3Int(0,0,-1), //DOWN
            new Vector3Int(-1,0,0) //LEFT
        };

    public static Vector3Int GetRandomDirection()
    {
        return cardinalDirectionList[Random.Range(0, cardinalDirectionList.Count)];
    }
}

