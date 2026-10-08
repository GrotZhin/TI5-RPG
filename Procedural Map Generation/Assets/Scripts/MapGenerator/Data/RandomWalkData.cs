using UnityEngine;

[CreateAssetMenu(fileName = "RandomWalkData", menuName = "Scriptable Objects/RandomWalkData")]
public class RandomWalkData : ScriptableObject
{
    public int iterations = 10, walkLenght = 10;
    public bool startRandomlyEachIterations = true;
    public GameObject prefab;
    
    
}
