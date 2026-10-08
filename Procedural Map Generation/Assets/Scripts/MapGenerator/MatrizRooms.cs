using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Reflection;
using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class MatrizRooms : AbstractMapGenerator
{
    public static MatrizRooms matriz;
    [SerializeField]
    private int height;
    [SerializeField]
    private int witdth;
    [SerializeField]
    private int roomCount;
    public List<RoomStats> island;
    [SerializeField]
    LayerMask layerMask;
    public GameObject floor;
    [SerializeField]
    private PathFind path;
    [SerializeField]
    private GridManager grid;
    public List<Door> doors = new List<Door>();
    public List<Vector3> positions = new List<Vector3>();

    public void Awake()
    {
        matriz = this;
    }
    protected override void RunProceduralGeneration()
    {
        //CreateRooms();
    }
    void Start()
    {

        //CreateRooms();
    }

    // private void CreateRooms()
    // {
    //     //var roomsSorted = ProceduralGeneration.SortRooms(roomCount, island);

    //     var map = ProceduralGeneration.CreateMatrizRooms(height, witdth, roomCount, roomsSorted);
    //     // var centers = ProceduralGeneration.ConnectRooms(list);
    //     var i = 0;
    //     foreach (var pos in map)
    //     {

    //         var obj = Instantiate(roomsSorted[i].gameObject, pos, Quaternion.identity);
    //         i++;
    //     }
    //     grid.InstantiateGrid();
    //     Invoke("CreateCorridors", 1f);




    //     //var doors = GameObject.FindGameObjectsWithTag("Door");
    //     //Metodo com portas
    //     // foreach (var item in doors)
    //     // {
    //     //     list.Add(Vector3Int.RoundToInt(item.transform.position));
    //     // }

    //     // var centers = ProceduralGeneration.ConnectRooms(list);
    //     // foreach (var pos in centers)
    //     // {
    //     //     Instantiate(floor, pos, Quaternion.identity);
    //     // }
    //     // prefabVisualizer.CreateFloorPrefabs(corridors);

    // }
    private void CreateCorridors()
    {
        HashSet<Vector3> paths = new HashSet<Vector3>();
       
        while (doors.Count > 0)
        {
            
            var currentDoor = doors[Random.Range(0, doors.Count)];
            Door closest = FindClosestPointTo(currentDoor, doors);

            path.FindPath(currentDoor.transform.position, closest.transform.position);
            Debug.Log(grid.path);   
            foreach(var pos in grid.path)
            {
                paths.Add(grid.GetVector3FromNodePosition(pos));
                
            }
        }
        foreach (var item in paths)
        {
            Instantiate(floor,item,Quaternion.identity);
        }
       
    }


    private Door FindClosestPointTo(Door door, List<Door> doors)
    {
        Door closest = door;
        float distance = float.MaxValue;
        doors.Remove(closest);
        foreach (var position in doors)
        {
            float currentDistance = Vector3.Distance(position.transform.position, door.transform.position);
            if (currentDistance < distance && position.id != door.id)
            {
                distance = currentDistance;

                closest = position;

            }
        }
        doors.Remove(closest);
        return closest;
    }




}
