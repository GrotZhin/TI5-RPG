using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class Combat : MonoBehaviour
{
    public Animator anim;
    public Transform scissorLeft, scissorRight;
    public SkillData skill;
    public readonly HashSet<EnemyAgent> alreadyHit = new HashSet<EnemyAgent>();
   

    void FixedUpdate()
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(1);
        bool inAttack = state.IsTag("Input Lock");


        if (!inAttack || skill == null) return;
        {

            if (skill.both)
            {
                CheckCollision(scissorLeft, skill);
                CheckCollision(scissorRight, skill);
            }
            else
            {
                if (skill.left)
                    CheckCollision(scissorLeft, skill);
                else
                    CheckCollision(scissorRight, skill);

            }

        }

    }
    void CheckCollision(Transform point, SkillData data)
    {
        Collider[] colliders = Physics.OverlapSphere(point.position, data.hitBox);
      
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent<EnemyAgent>(out EnemyAgent enemy))
            {
                if(!alreadyHit.Add(enemy)) continue;
                enemy.ChangeState(new EnemyStateDano(enemy, enemy.renderer, data.force));
            }
        }
    }
    void OnDrawGizmos()
    {
        if (skill == null) return;
        if (skill.both)
        {
            Gizmos.DrawWireSphere(scissorLeft.position, skill.hitBox);
            Gizmos.DrawWireSphere(scissorRight.position, skill.hitBox);
        }
        else
        {
            if (skill.left)
                Gizmos.DrawWireSphere(scissorLeft.position, skill.hitBox);
            else
                Gizmos.DrawWireSphere(scissorRight.position, skill.hitBox);
        }



    }
}
