using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCheck : MonoBehaviour
{
    public Transform attackPoint;
    public readonly HashSet<PlayerStats> alreadyHit = new HashSet<PlayerStats>();
    public SkillData skill;

    public void CheckCollision()
    {
       
        Collider[] colliders = Physics.OverlapSphere(attackPoint.position, skill.hitBox);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent<PlayerStats>(out PlayerStats stats))
            {
                if (!alreadyHit.Add(stats)) continue;
                stats.TakeDamage(skill.force);
                
            }
        }
    }
     void OnDrawGizmos()
    {
       
            Gizmos.DrawWireSphere(attackPoint.position, skill.hitBox);
           
        



    }
}
