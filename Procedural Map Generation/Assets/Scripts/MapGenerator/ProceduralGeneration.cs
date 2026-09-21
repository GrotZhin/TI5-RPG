using System.Collections.Generic;
using System.Drawing;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
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
    public static HashSet<BoundsInt> CreateRooms(List<RoomStats> rooms, Vector3Int startPosition, Vector3Int offset, int count)
    {
        HashSet<BoundsInt> roomsPosition = new HashSet<BoundsInt>();

        BoundsInt currentPosition = new BoundsInt(startPosition, rooms[0].size.size);
        while (count > 0)
        {
            count--;
            foreach (var position in rooms)
            {
                roomsPosition.Add(currentPosition);
                //if(Random.value < 0.5f)
                currentPosition.position += position.size.max + Direction2D.GetRandomDirection() * offset;
                // else  
                // currentPosition = currentPosition + position.size.min * 2 +  Direction2D.GetRandomDirection() * offset;
            }

        }
        return roomsPosition;
    }
    public static HashSet<Vector3Int> CreateMatrizRooms(int x, int z, int roomCount, List<RoomStats> islands, LayerMask layerMask)
    {
        int[,] map = new int[x, z];
        HashSet<Vector3Int> positions = new HashSet<Vector3Int>();
        List<Bounds> rooms = new List<Bounds>();
        List<GameObject> objects = new List<GameObject>();
        var i = 0;
        var t = 0;

        var distance = Vector3Int.zero;
        int maxTrys = 100;

        while (roomCount > 0)
        {
            Vector3Int pos = Vector3Int.zero;
            bool positionFound = false;
            var trys = 0;
            //GameObject obj = CreateGameObjects(pos, islands[i].size.size, ref t);
            while (!positionFound && trys < maxTrys)
            {
                var positionX = Random.Range(0, map.GetLength(0));
                var positionZ = Random.Range(0, map.GetLength(1));


                distance.x = Random.Range(islands[i].size.min.x, islands[i].size.max.x) * Random.Range(2, 5);
                distance.z = Random.Range(islands[i].size.min.z, islands[i].size.max.z) * Random.Range(2, 5);

                pos = new Vector3Int(positionX, 0, positionZ) + distance;
                var newRoom = new Bounds(pos, islands[i].size.size);
                bool dontCollide = false;

                foreach (var room in rooms)
                {
                    if (newRoom.Intersects(room))
                    {
                        dontCollide = true;
                        break;
                    }
                }
                if (!dontCollide)
                {
                    positionFound = true;
                    rooms.Add(newRoom);

                }
                trys++;
            }
            if (!positionFound)
            {
                Debug.Log("cabou o espaço");
                break;
            }
            // Collider[] hits;
            // bool hit;
            // do
            // {


            //     obj.transform.position = pos;
            //     Physics.SyncTransforms();
            //     hits = Physics.OverlapBox(pos, islands[i].size.size / 2, Quaternion.identity, layerMask);

            // } while (hits.Length != 0);

            //obj.layer = LayerMask.NameToLayer("louco");
            //rooms.Add(newRoom);
            positions.Add(pos);
            roomCount--;
            i++;
        }
        return positions;

    }

    public static GameObject CreateGameObjects(Vector3Int pos, Vector3Int size, ref int i)
    {
        GameObject obj = new GameObject();

        obj.name = i.ToString();
        obj.transform.position = pos;
        obj.layer = LayerMask.NameToLayer("excludeLayer");

        BoxCollider box = obj.AddComponent<BoxCollider>();
        box.size = size;
        i++;
        return obj;
    }


    public static List<RoomStats> SortRooms(int roomCount, List<RoomStats> island)
    {
        List<RoomStats> rooms = new List<RoomStats>();

        while (roomCount > 0)
        {
            var porcent = Random.value * 100;
            switch (porcent)
            {
                case < 10:
                    rooms.Add(island[0]);
                    roomCount--;
                    break;
                case >= 10 and < 50:
                    rooms.Add(island[1]);
                    roomCount--;
                    break;
                case >= 50 and <= 100:
                    rooms.Add(island[2]);
                    roomCount--;
                    break;
            }
        }
        return rooms;
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
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.size.y, room.min.z + zSplit),
        new Vector3Int(room.size.x, room.size.y, room.size.z - zSplit));

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

