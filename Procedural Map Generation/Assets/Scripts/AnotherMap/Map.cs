using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField]
    private int witdh = 20;
    [SerializeField]
    private int height = 20;
    [SerializeField]
    private int numberOfRooms = 10;
    [SerializeField]
    private GameObject roomPrefab;
    [SerializeField]
    private GameObject corridorPrefab;
    [SerializeField]
    private GameObject roomBossPrefab;
    [SerializeField]
    private GameObject firstRoomPrefab;
    [SerializeField]
    private GameObject roomTreasurePrefab;
    [SerializeField]
    private DoorPos doorPrefab;
    [SerializeField] List<DoorPos> doorsPositions = new List<DoorPos>();
    public Dictionary<string, Door> actualDoors = new Dictionary<string, Door>();
    public static Map instance;

    private Room[,] map;
    private List<Room> deadEndRooms = new List<Room>();


    void Awake()
    {
        instance = this;
        CreateMapLayout();
        PlaceRooms();
    }
    private void CreateMapLayout()
    {
        int roomsCount = 0;
        while (roomsCount < numberOfRooms)
        {

            InitializeMap();
            deadEndRooms.Clear();

            Room startingRoom = map[witdh / 2, height / 2];
            startingRoom.IsSelected = true;

            Queue<Room> roomsToVisit = new Queue<Room>();
            roomsToVisit.Enqueue(startingRoom);


            roomsCount = 1;

            while (roomsToVisit.Count > 0)
            {
                Room currentRoom = roomsToVisit.Dequeue();
                List<Room> currentRoomNeighbors = GetNeighbors(currentRoom);
                bool hasAddedNewRoom = false;

                foreach (Room neighbor in currentRoomNeighbors)
                {
                    if (neighbor.IsSelected) continue;
                    if (HasMoreThanOneNeighbor(neighbor)) continue;
                    bool hasEnoughtRoom = roomsCount >= numberOfRooms;
                    if (hasEnoughtRoom) continue;
                    bool shouldGiveUp = Random.value < 0.5f;
                    if (shouldGiveUp) continue;
                    neighbor.IsSelected = true;
                    roomsToVisit.Enqueue(neighbor);
                    roomsCount++;
                    hasAddedNewRoom = true;
                }
                if (!hasAddedNewRoom) deadEndRooms.Add(currentRoom);
            }
        }
    }
    private void InitializeMap()
    {
        map = new Room[witdh, height];
        for (int x = 0; x < witdh; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = new Room(new Vector3Int(x, 0, y));
            }
        }
    }
    private List<Room> GetNeighbors(Room room)
    {
        List<Room> output = new List<Room>();
        int up = room.Position.z + 1;
        if (IsRoomPositionValid(new Vector3Int(room.Position.x, 0, up))) output.Add(map[room.Position.x, up]);

        int down = room.Position.z - 1;
        if (IsRoomPositionValid(new Vector3Int(room.Position.x, 0, down))) output.Add(map[room.Position.x, down]);

        int right = room.Position.x + 1;
        if (IsRoomPositionValid(new Vector3Int(right, 0, room.Position.z))) output.Add(map[right, room.Position.z]);

        int left = room.Position.x - 1;
        if (IsRoomPositionValid(new Vector3Int(left, 0, room.Position.z))) output.Add(map[left, room.Position.z]);
        return output;
    }

    private bool IsRoomPositionValid(Vector3Int position)
    {
        if (position.x < 0 || position.x >= witdh) return false;
        if (position.z < 0 || position.z >= height) return false;
        return true;
    }

    private bool HasMoreThanOneNeighbor(Room room)
    {
        List<Room> neighbors = GetNeighbors(room);
        int selectedNeighborsCount = 0;
        foreach (var neighbor in neighbors)
        {
            if (neighbor.IsSelected)
                selectedNeighborsCount++;
        }
        return selectedNeighborsCount > 1;
    }
    private void PlaceRooms()
    {
        PlaceNormalRooms();
        PlaceFirstRoom();
        PlaceBossRoom();
        PlaceTreasureRooms();
        CreateCorridors();
    }
    private void PlaceFirstRoom()
    {
        Room start = map[witdh / 2, height / 2];
        InstantiateRoom(firstRoomPrefab, start);
    }
     private void PlaceNormalRooms()
    {
        foreach (var room in map)
        {
            if (room.IsSelected && !deadEndRooms.Contains(room))
                InstantiateRoom(roomPrefab, room);
        }
    }
    private GameObject InstantiateRoom(GameObject roomPrefab, Room room)
    {
        Vector3 roomPosition = new Vector3(room.Position.x * 20, 0, room.Position.z * 20);
        
        GameObject obj = Instantiate(roomPrefab, roomPosition, Quaternion.identity, transform);

        AddDoorsToRoom( room);
        return obj;
    }
    private void PlaceBossRoom()
    {
        Room room = deadEndRooms.Last();
        InstantiateRoom(roomBossPrefab, room);
    }
    private void PlaceTreasureRooms()
    {
        List<Room> rooms = deadEndRooms.Where(room => room != deadEndRooms.Last()).ToList();
        foreach (Room room in rooms)
        {
            float treasureChance = Random.value;
            bool hasTreasure = treasureChance > 0.7f;

            if (hasTreasure) InstantiateRoom(roomTreasurePrefab, room);
            else InstantiateRoom(roomPrefab, room);
        }
    }
    int islandId = 0;
    private Dictionary<(Vector3Int, Vector3Int), DoorPos> doorsByEdge = new();
    private void GetDoorPosition( string doorDirection, DoorPos doorPrefab, Vector3Int from, Vector3Int to)
    {
        Vector3 pos = actualDoors[doorDirection].transform.position;
      
        
        var door = Instantiate(doorPrefab, pos, Quaternion.identity);
        door.id = islandId;
        doorsPositions.Add(door);

        bool fromFirst = from.x < to.x || (from.x == to.x && from.z < to.z);
        var key = fromFirst ? (from,to): (to,from);

        if(doorsByEdge.TryGetValue(key, out var other))
        {
            door.pair = other;
            other.pair = door;
        }
        else doorsByEdge[key] = door;

    }

    private void CreateCorridors()
    {
        //var currentDoor = doorsPositions[0];
        
        HashSet<Vector3Int> corridors = new HashSet<Vector3Int>();
        HashSet<DoorPos> done = new HashSet<DoorPos>();

        foreach (var door in doorsPositions)
        {
            if(door.pair == null || done.Contains(door)) continue;
            foreach (var pos in PositionCorridor(door, door.pair))
            {
                corridors.Add(pos);
            }
            done.Add(door);
            done.Add(door.pair);
        }
        InstantiateCorridor(corridors);

       

    }

    private void InstantiateCorridor(HashSet<Vector3Int> positions)
    {
        
        foreach (var pos in positions)
        {
            Instantiate(corridorPrefab, pos, Quaternion.identity);
        }
    }

    private HashSet<Vector3Int> PositionCorridor(DoorPos currentDoor, DoorPos closest)
    {
        HashSet<Vector3Int> corridor = new HashSet<Vector3Int>();
        Vector3Int doorPosition = Vector3Int.RoundToInt(currentDoor.transform.position);
        Vector3Int destination = Vector3Int.RoundToInt(closest.transform.position);
       
        
            while(doorPosition.x != destination.x)
            {
                if (destination.x > doorPosition.x)
            {
                doorPosition += Vector3Int.right;
            }
            else if (destination.x < doorPosition.x)
            {
                doorPosition += Vector3Int.left;
            }
            corridor.Add(doorPosition);
            }
      
              while(doorPosition.z != destination.z)
            {
                if (destination.z > doorPosition.z)
            {
                doorPosition += Vector3Int.forward;
            }
            else if (destination.z < doorPosition.z)
            {
                doorPosition += Vector3Int.back;
            }
            corridor.Add(doorPosition);
            }
       
        return corridor;
    }

   
    private void AddDoorsToRoom( Room room)
    {
        var cur = new Vector3Int(room.Position.x, 0, room.Position.z);
        int up = room.Position.z + 1;
        if (IsRoomPositionValid(new Vector3Int(room.Position.x, 0, up)))
        {
          
            if (map[room.Position.x, up].IsSelected)
                GetDoorPosition( "Up", doorPrefab, cur, new Vector3Int(room.Position.x,0,up));
        }
        int down = room.Position.z - 1;
        if (IsRoomPositionValid(new Vector3Int(room.Position.x, 0, down)))
        {
             
            if (map[room.Position.x, down].IsSelected)
                GetDoorPosition( "Down", doorPrefab,cur, new Vector3Int(room.Position.x,0,down));
        }

        int right = room.Position.x + 1;
        if (IsRoomPositionValid(new Vector3Int(right, 0, room.Position.z)))
        {
            if (map[right, room.Position.z].IsSelected)
                GetDoorPosition( "Right", doorPrefab,cur, new Vector3Int(right,0,room.Position.z));
        }

        int left = room.Position.x - 1;
        if (IsRoomPositionValid(new Vector3Int(left, 0, room.Position.z)))
        {
            if (map[left, room.Position.z].IsSelected)
                GetDoorPosition( "Left", doorPrefab,cur, new Vector3Int(left,0,room.Position.z));

                
        }
        actualDoors.Clear();
        islandId++;

    }
   
}

