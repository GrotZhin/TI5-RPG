using UnityEngine;

public class Door : MonoBehaviour
{
    public string id;
    
    public Vector3 direction;
    public bool isWay;
    
    void Awake()
    {
     
     
     Map.instance.actualDoors[id] = this;

    }
    
}
