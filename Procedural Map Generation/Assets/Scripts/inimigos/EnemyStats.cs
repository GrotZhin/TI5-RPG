using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Scriptable Objects/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    [SerializeField] private float life;
    [SerializeField] private float stance;
    [SerializeField] private int coinDrop = 10;

    public int CoinDrop => coinDrop;
    public float Stance => stance;
    public float Life => life;
    
 
}
