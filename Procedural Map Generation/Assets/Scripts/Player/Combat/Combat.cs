using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class Combat : MonoBehaviour
{
    public Animator anim;
    public Transform scissorLeft, scissorRight;
    public SkillTree skill;

    void FixedUpdate()
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(1);
        
        if(state.IsTag("X"))
        {
            if(skill.firstLightHit.left)
            CheckCollision(scissorLeft, skill.firstLightHit);
            else
            CheckCollision(scissorRight, skill.firstLightHit);
        }else if(state.IsTag("HeavyAttack"))
        {
            CheckCollision(scissorLeft, skill.HeavyCut);
        }

    }
    void CheckCollision(Transform point, SkillData data)
    {
        Collider[] colliders = Physics.OverlapSphere(point.position, data.hitBox);
        for (int i = 0; i < colliders.Length; i++)
        {
            if(colliders[i].TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddForceAtPosition(transform.forward * data.force, point.position);
            }
        }
    }
    void OnDrawGizmos()
    {
        if (skill.firstLightHit.left)
        Gizmos.DrawWireSphere(scissorLeft.position, skill.firstLightHit.hitBox);
        else
        Gizmos.DrawWireSphere(scissorRight.position, skill.firstLightHit.hitBox);
        
            
        
    }
}
