using UnityEngine;

public class EnemyStateDano : IState
{
    EnemyAgent agent;
    int chace;
    float time;
    SkinnedMeshRenderer renderer;
    public EnemyStateDano(EnemyAgent agent, SkinnedMeshRenderer renderer)
    {
        this.agent = agent;
        this.renderer = renderer;
    }

    public void Enter()
    {
        Debug.Log("Move entrou");
        renderer.material.color = Color.black;
        chace = Random.Range(0, 100);
        time = 2;
    }

    public void Execute(float delta)
    {
        time -= delta;
        Debug.Log("move executando");
        if (time < 0)
        {
            if (chace > 20 || agent.player == null)
            {
                agent.ChangeState(new EnemyStateFlee(agent, renderer));
            }
            else
            {
                agent.ChangeState(new EnemyStateAttack(agent, renderer));
            }
        }
    }

    public void Exit()
    {
        Debug.Log("IDLE saiu");
    }
    
}
