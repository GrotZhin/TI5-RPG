using System.Collections.Generic;
using UnityEngine;

public class DoorsInScene : MonoBehaviour
{
   public List<DoorPos> doors;

   public static DoorsInScene doorsInScene;
    void Awake()
    {
        doorsInScene = this;
    }
}
