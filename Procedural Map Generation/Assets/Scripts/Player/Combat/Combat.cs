
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class Combat : MonoBehaviour
{
    public Animator anim;
    public Transform scissorLeft, scissorRight, bowHandle;
    public SkillData skill;
    public LayerMask layer;
    public readonly HashSet<Agent> alreadyHit = new HashSet<Agent>();

    private GameObject arrow;
    private void Start()
    {
        DefineSkill.ShootArrow = ShootArrow;
    }

    void FixedUpdate()
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        bool inAttack = state.IsTag("Input Lock");


        if (!inAttack || skill == null || skill.Arrow) return;
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

    public void ShootArrow()
    { 
        GameObject prefab = Resources.Load<GameObject>("Needle");
        arrow =  Instantiate(prefab, bowHandle.transform.position, transform.rotation);
        arrow.GetComponent<Arrow>().Init(skill);
    }

    void CheckCollision(Transform point, SkillData data)
    {
        //Collider[] colliders = Physics.OverlapSphere(point.position, data.HitBox, layer);
        Collider[] colliders = Physics.OverlapBox(point.position, new Vector3(data.HitBox,data.HitBox,data.HitBox),Quaternion.identity, layer);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent<Agent>(out Agent enemy))
            {
                if (!alreadyHit.Add(enemy)) continue;
                if (enemy.stats.Life > 0)
                {
                    GameFeel.gameFeel.Stop(data.HitStop);
                    GameFeel.gameFeel.CamaraShake();
                    if (StyleSystem.Instance != null) StyleSystem.Instance.RegisterHit(data);
                    enemy.ChangeState(new ReceiveDamage(enemy, data.Force, enemy.renderer));
                }//enemy.ChangeState(new EnemyStateDano(enemy, enemy.renderer, data.Force));
            }
        }
    }

    void OnDrawGizmos()
    {
        if (skill == null) return;
        if (skill.Both)
        {
            // Gizmos.DrawWireSphere(scissorLeft.position, skill.HitBox);
            // Gizmos.DrawWireSphere(scissorRight.position, skill.HitBox);
            Gizmos.DrawWireCube(scissorLeft.position, new Vector3(skill.HitBox,skill.HitBox,skill.HitBox));
            Gizmos.DrawWireCube(scissorRight.position, new Vector3(skill.HitBox,skill.HitBox,skill.HitBox));
        }
        else
        {
            if (skill.Left)
                //Gizmos.DrawWireSphere(scissorLeft.position, skill.HitBox);
                    Gizmos.DrawWireCube(scissorLeft.position, new Vector3(skill.HitBox,skill.HitBox,skill.HitBox));
            else
                  Gizmos.DrawWireCube(scissorRight.position, new Vector3(skill.HitBox,skill.HitBox,skill.HitBox));
        }
    }
}
