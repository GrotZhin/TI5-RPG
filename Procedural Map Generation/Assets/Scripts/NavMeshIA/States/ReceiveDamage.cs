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

        AnimatorStateInfo state = agent.animator.GetCurrentAnimatorStateInfo(0);
        if (!state.IsTag("damage"))
        {
            agent.animator.SetBool("damage",true);
        }
        time = 1;
        GameFeel.gameFeel.FlashEffect(0.2f, renderer);
        bool isDead = agent.stats.Damage(damage);
        if (isDead) agent.ChangeState(new Death(agent));

    }

    public void Execute(float delta)
    {
        time -= Time.deltaTime;
        //if (agent.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !agent.animator.IsInTransition(0) && time <= 0)
        AnimatorStateInfo state = agent.animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsTag("Other"))
        {
            agent.ChangeState(new Walk(agent));
            Debug.Log("timer");

        }

    }

    public void Exit()
    {
        Debug.Log("Sai do state");
        agent.animator.SetBool("damage",false);
    }
}
