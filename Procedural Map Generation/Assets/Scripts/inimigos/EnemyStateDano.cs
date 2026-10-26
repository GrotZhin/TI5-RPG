using UnityEngine;

public class EnemyStateDano : IState
{
    EnemyAgent agent;
    int chace;
    float time;
    SkinnedMeshRenderer renderer;
    Animator animator;
    float damage;
    public EnemyStateDano(EnemyAgent agent, SkinnedMeshRenderer renderer, float damage)
    {
        this.agent = agent;
        this.renderer = renderer;
        this.damage = damage;
    }

    public void Enter()
    {

        //Debug.Log("Move entrou");
        renderer.material.color = Color.black;


        chace = Random.Range(0, 100);

        agent.animator.SetBool("Dano", true);
        bool isDead = agent.control.dano(damage);
        if(isDead) agent.ChangeState(new EnemyStateDeath(agent, renderer)) ;
        time = 1;
    }

    public void Execute(float delta)
    {
        time -= delta;
        //Debug.Log("move executando");
        if (agent.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !agent.animator.IsInTransition(0) && time <= 0)
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

        agent.animator.SetBool("Dano", false);
    }


}
