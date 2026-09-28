using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public static class ProceduralGeneration
{
    public static HashSet<Vector3Int> CreateMatrizRooms(int x, int z, int roomCount, List<RoomStats> islands, ref List<Vector3Int> centers)
    {
        int[,] map = new int[x, z];
        HashSet<Vector3Int> positions = new HashSet<Vector3Int>();
        List<Bounds> rooms = new List<Bounds>();
        var i = 0;

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
                    
                    centers.Add(Vector3Int.RoundToInt(newRoom.center));
                }
                trys++;
            }
            if (!positionFound)
            {
                Debug.Log("cabou o espaço");
                break;
            }
           
            positions.Add(pos);
            roomCount--;
            i++;
        }
       
        return positions;

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
    public static HashSet<Vector3Int> ConnectRooms(List<Vector3Int> roomCenters)
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

   private static HashSet<Vector3Int> CreateCorridor(Vector3Int currentRoomCenter, Vector3Int destination)
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

    private static Vector3Int FindClosestPointTo(Vector3Int currentRoomCenter, List<Vector3Int> roomCenters)
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

}