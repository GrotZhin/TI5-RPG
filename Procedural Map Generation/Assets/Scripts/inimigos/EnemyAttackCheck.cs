using Mono.Cecil;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCheck : MonoBehaviour
{
    public Transform attackPoint;
    public readonly HashSet<PlayerStats> alreadyHit = new HashSet<PlayerStats>();
    public SkillData skill;

    public void CheckCollision()
    {
       
        Collider[] colliders = Physics.OverlapSphere(attackPoint.position, skill.HitBox);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent<PlayerStats>(out PlayerStats stats))
            {
                if (!alreadyHit.Add(stats)) continue;
                stats.TakeDamage(skill.Force);
                Debug.Log("acertei o player");
                
            }
        }
        if (StyleSystem.Instance != null && StyleSystem.Instance.RecentlyDashed)
        {
            foreach (var c in Physics.OverlapSphere(attackPoint.position, skill.HitBox + 1.5f))
                if (c.TryGetComponent<PlayerStats>(out var s) && !alreadyHit.Contains(s))
                { 
                    StyleSystem.Instance.RegisterPerfectDodge(); break; 
                }
        }
    }
     void OnDrawGizmos()
    {
            Gizmos.DrawWireSphere(attackPoint.position, skill.HitBox);
    }
}
