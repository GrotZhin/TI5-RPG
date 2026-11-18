using System.Collections;
using UnityEngine;


public class ReceiveDamage : IState
{
    Agent agent;
    SkinnedMeshRenderer renderer;
    float damage;
    float time;

    public ReceiveDamage(Agent agent, float damage, SkinnedMeshRenderer renderer)
    {
        this.agent = agent;
        this.damage = damage;
        this.renderer = renderer;

    }
    public void Enter()
    {
        Debug.Log("Entrei no state dnv");
        bool isDead = agent.stats.Damage(damage);
        if (isDead)
        {
        agent.ChangeState(new Death(agent));
            
        } 


        agent.animator.Play("damage", 0, 0f);

        GameFeel.gameFeel.FlashEffect(0.2f, renderer);


    }

    public void Execute(float delta)
    {

        AnimatorStateInfo state = agent.animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsTag("damage") && agent.animator.IsInTransition(0))
        {
            agent.ChangeState(new Walk(agent));
        }

    }

    public void Exit()
    {

        Debug.Log("Sai do state");
    }
}
