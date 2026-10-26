using UnityEngine;

public class EnemyStateIdle : IState
{
    EnemyAgent agent;
    float time;
    SkinnedMeshRenderer renderer;
    Animator animator;
    public EnemyStateIdle(EnemyAgent agent, SkinnedMeshRenderer renderer)
    {
        this.agent = agent;
        this.renderer = renderer;
    }

    public void Enter()
    {
        //Debug.Log("IDLE entrou");
        time = 3;
        renderer.material.color = Color.gray;
       
        agent.animator.SetBool("andar", false);
        Vector3 dirI = Random.insideUnitSphere;
        dirI.y = 0;
        agent.transform.forward = dirI;
    }

    public void Execute(float delta)
    {
        //Debug.Log("IDLE executando");
        time -= delta;
        //agent.cc.Move(this.animator.deltaPosition);
        if (agent.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >=1f && !agent.animator.IsInTransition(0))
        {
            agent.ChangeState(new EnemyStateMove(agent, renderer));
        }
    }

    public void Exit()
    {
        //Debug.Log("IDLE saiu");
        //animator.SetBool("IsMoving", false);
        //animator.SetFloat("Input Magnitude", 0, 0f, 0);
        
        agent.GetNeighbours().Clear();
    }
}
