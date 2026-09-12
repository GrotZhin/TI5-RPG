using UnityEngine;

public class EnemyStateIdle : IState
{
    EnemyAgent agent;
    float time;
    SkinnedMeshRenderer renderer;
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
    }

    public void Execute(float delta)
    {
        //Debug.Log("IDLE executando");
        time -= delta;
        if (time < 0)
        {
            agent.ChangeState(new EnemyStateMove(agent, renderer));
        }
    }

    public void Exit()
    {
        //Debug.Log("IDLE saiu");
    }
}
