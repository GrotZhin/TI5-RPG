
using UnityEngine;

public class EnemyStateDeath : IState
{
    EnemyAgent agent;
    int chace;
    float time;
    SkinnedMeshRenderer renderer;
    Animator animator;
    public EnemyStateDeath(EnemyAgent agent, SkinnedMeshRenderer renderer)
    {
        this.agent = agent;
        this.renderer = renderer;

    }

    public void Enter()
    {

        //Debug.Log("Move entrou");
        renderer.material.color = Color.black;


        time = 1;
        agent.animator.SetBool("IsDead", true);
    }

    public void Execute(float delta)
    {
        time -= Time.deltaTime;
        if (agent.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !agent.animator.IsInTransition(0) && time <= 0)
        {
            agent.Destroy();
        }
    }

    public void Exit()
    {


    }


}
