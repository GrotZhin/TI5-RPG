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
       
        if (!agent.animator.GetBool("transformar"))
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
        Debug.Log(animator.GetBool("transformar"));
        if (agent.animator.GetBool("transformar"))
        {
            Debug.Log((agent.animator.GetCurrentAnimatorStateInfo(0).normalizedTime == 0f) + "  " + !agent.animator.IsInTransition(0));
            if (agent.animator.GetCurrentAnimatorStateInfo(0).normalizedTime == 0f && !agent.animator.IsInTransition(0))
            {
                agent.ChangeState(new EnemyStateAttack(agent, renderer));
            }
        }
        else
        {
            if (agent.animator.GetCurrentAnimatorStateInfo(0).normalizedTime == 0f && !agent.animator.IsInTransition(0))
            {
                agent.ChangeState(new EnemyStateIdle(agent, renderer));
            }
        }
    }

    public void Exit()
    {
        
    }
}
