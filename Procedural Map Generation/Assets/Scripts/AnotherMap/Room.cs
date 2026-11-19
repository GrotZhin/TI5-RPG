using UnityEngine;

public class Room
{
    
    public Room(Vector3Int position)
    {
        Position = position;
    }
    public Vector3Int Position { get; set; }
    public bool IsSelected { get; set; }
}
