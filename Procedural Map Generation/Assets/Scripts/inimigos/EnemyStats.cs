using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Scriptable Objects/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    [SerializeField] private float life;
    [SerializeField] private float stance;

    public float Stance => stance;
    public float Life => life;
    

}
