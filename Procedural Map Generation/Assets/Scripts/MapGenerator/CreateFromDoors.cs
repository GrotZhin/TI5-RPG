using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CreateFromDoors : AbstractMapGenerator
{
    [SerializeField]
    private int height;
    [SerializeField]
    private int witdth;
    [SerializeField]
    private int roomCount;
    public List<RoomStats> island;
    public static CreateFromDoors createFromDoors;
    public List<Door> actualDoor;
    public RoomStats firtsIsle;

    void Awake()
    {
        createFromDoors = this;
    }
    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }
    void Start()
    {
        Invoke("CreateRooms", 3f);
    }
    private void CreateRooms()
    {
        var roomsSorted = ProceduralGeneration.SortRooms(roomCount, island, firtsIsle);
        DefinePositions(roomCount, roomsSorted, actualDoor);

    }
    private void DefinePositions(int roomCount, List<RoomStats> islands, List<Door> doors)
    {
        List<RoomStats> list = new List<RoomStats>();

        roomCount--;
        var i = 0;
        var j = 1;
        int trys = 0;
        while (roomCount > 0)
        {
            var roomNumber = Random.Range(1, islands[i].doors.Count);
            Debug.Log(roomNumber);
            var doorChosed = islands[i].doors[Random.Range(0, islands[i].doors.Count)];
            while (roomNumber > 0 && trys < 100)
            {
                //actualDoor = islands[i].doors;
                Debug.Log("numbers of doors");



                islands[i].doors.Remove(doorChosed);
                var position = doorChosed.transform.position;
                var offset = doorChosed.direction * Random.Range(10, 20);
                var pos = position + offset;

                Instantiate(island[j].gameObject, pos, Quaternion.identity);
                j++;
                roomNumber--;

                trys++;
            }

            i++;
            roomCount--;

        }
    }
}
