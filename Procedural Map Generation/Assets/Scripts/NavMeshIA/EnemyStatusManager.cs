using Unity.VisualScripting;
using UnityEngine;

public class EnemyStatusManager : MonoBehaviour
{
    [SerializeField] private float life;
    [SerializeField] private float stance;
    [SerializeField] EnemyStats stats;
    public EnemyStats Stats => stats;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        life = stats.Life;
        stance = stats.Stance;
    }
    public bool Damage(float damage)
    {
        life -= damage;

        if (life <= 0)

            return true;

        else return false;
    }
}
