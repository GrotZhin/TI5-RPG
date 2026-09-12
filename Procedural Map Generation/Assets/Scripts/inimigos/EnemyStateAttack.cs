using UnityEngine;

public class EnemyStateAttack : IState
{
    EnemyAgent agent;
    float time;
    int chace;
    SkinnedMeshRenderer renderer;

    public EnemyStateAttack(EnemyAgent agent, SkinnedMeshRenderer renderer)
    {
        this.agent = agent;
        this.renderer = renderer;
    }

    public void Enter()
    {
        //Debug.Log("Atack entrou");
        renderer.material.color = Color.red;
        Vector3 dir = agent.player.transform.position - agent.transform.position;
        dir.y = agent.transform.position.y;
        Quaternion toRotation = Quaternion.LookRotation(dir, Vector3.up);

        chace = Random.Range(0, 100);
        time = 2;
    }

    public void Execute(float delta)
    {
        //Debug.Log("atack executando");
        
        time -= delta;
        if (time < 0)
        {
            if (chace > 50)
            {
                agent.ChangeState(new EnemyStateMove(agent, renderer));
            }
            else if(chace > 40)
            {
                agent.ChangeState(new EnemyStateMove(agent, renderer));
            }
            else
            {
                agent.ChangeState(new EnemyStateFlee(agent, renderer));
            }
        }
    }

    public void Exit()
    {
        //Debug.Log("IDLE saiu");
    }

}
