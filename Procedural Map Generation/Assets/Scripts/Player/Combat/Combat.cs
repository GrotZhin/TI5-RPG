using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class Combat : MonoBehaviour
{
    public Animator anim;
    public Transform scissorLeft, scissorRight;
    public SkillData skill;
    public LayerMask layer;
    public readonly HashSet<EnemyAgent> alreadyHit = new HashSet<EnemyAgent>();
   

    void FixedUpdate()
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(1);
        bool inAttack = state.IsTag("Input Lock");


        if (!inAttack || skill == null) return;
        {

            if (skill.Both)
            {
                CheckCollision(scissorLeft, skill);
                CheckCollision(scissorRight, skill);
            }
            else
            {
                if (skill.Left)
                    CheckCollision(scissorLeft, skill);
                else
                    CheckCollision(scissorRight, skill);

            }

        }

    }
    void CheckCollision(Transform point, SkillData data)
    {
        Collider[] colliders = Physics.OverlapSphere(point.position, data.HitBox,layer);
      
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent<EnemyAgent>(out EnemyAgent enemy))
            {
                if(!alreadyHit.Add(enemy)) continue;
                //GameFeel.gameFeel.Stop(data.hitStop);
                CameraShakeManager.Shaker.ShakePulse(5,1,.1f);
                enemy.ChangeState(new EnemyStateDano(enemy, enemy.renderer, data.Force));
            }
        }
    }
  
    void OnDrawGizmos()
    {
        if (skill == null) return;
        if (skill.Both)
        {
            Gizmos.DrawWireSphere(scissorLeft.position, skill.HitBox);
            Gizmos.DrawWireSphere(scissorRight.position, skill.HitBox);
        }
        else
        {
            if (skill.Left)
                Gizmos.DrawWireSphere(scissorLeft.position, skill.HitBox);
            else
                Gizmos.DrawWireSphere(scissorRight.position, skill.HitBox);
        }



    }
}
