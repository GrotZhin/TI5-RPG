using UnityEngine;
using UnityEngine.UIElements;

public class EnemyStateTransformar :IState
{
    EnemyAgent agent;
    int chace;
    float time;
    Animator animator;
    Vector3 target, dirtmp;
    SkinnedMeshRenderer renderer;
    float rotationSpeed = 5f;
    public EnemyStateTransformar(EnemyAgent agent, SkinnedMeshRenderer renderer)
    {
        this.agent = agent;
        this.renderer = renderer;
    }

    public void Enter()
    {
        Debug.Log("transformar");
        animator = agent.GetComponent<Animator>();
        if (animator.GetBool("transformar"))
        {
            animator.SetBool("transformar", true);
        }
        else
        {
            animator.SetBool("transformar", false);
        }
    }

    public void Execute(float delta)
    {
        if (animator.GetBool("transformar"))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime == 0f && !animator.IsInTransition(0))
            {
                agent.ChangeState(new EnemyStateAttack(agent, renderer));
            }
        }
        else
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime == 0f && !animator.IsInTransition(0))
            {
                agent.ChangeState(new EnemyStateIdle(agent, renderer));
            }
        }
    }

    public void Exit()
    {
        
    }
}
