using UnityEngine;

public class EnemyStateDanoCa : IState
{
    EnemyAgentCa agent;
    int chace;
    float time;
    SkinnedMeshRenderer renderer;
    Animator animator;
    public EnemyStateDanoCa(EnemyAgentCa agent, SkinnedMeshRenderer renderer)
    {
        this.agent = agent;
        this.renderer = renderer;
    }

    public void Enter()
    {
        //Debug.Log("Move entrou");
        renderer.material.color = Color.black;
        agent.control.dano(5);
        chace = Random.Range(0, 100);
        animator = agent.GetComponent<Animator>();
        animator.SetBool("Dano", true);
        time = 1;
    }

    public void Execute(float delta)
    {
        time -= delta;
        //Debug.Log("move executando");
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !animator.IsInTransition(0) && time <= 0)
        {
            if (chace > 20 || agent.player == null)
            {
                agent.ChangeState(new EnemyStateFleeCa(agent, renderer));
            }
            else
            {
                agent.ChangeState(new EnemyStateAttackCa(agent, renderer));
            }
        }
    }

    public void Exit()
    {
        Debug.Log("dano saiu  " + agent.control.vida);
        animator.SetBool("Dano", false);
    }
    
}
