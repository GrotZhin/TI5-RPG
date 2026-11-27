using System.Collections;
using UnityEngine;


public class Death : IState
{
    Agent agent;
    SkinnedMeshRenderer renderer;
    float damage;
    float time;

    public Death(Agent agent)
    {
        this.agent = agent;
       

    }
    public void Enter()
    {
        agent.animator.SetTrigger("isDead");
        time = 1;
        if (!agent.droppedLoot)
        {
            agent.droppedLoot = true;
            if (MoneyOrbManager.Instance != null && agent.stats != null && agent.stats.Stats != null) MoneyOrbManager.Instance.Spawn(agent.transform.position + Vector3.up, agent.stats.Stats.CoinDrop);
        }
    }


    public void Execute(float delta)
    {
    
        if (agent.animator.GetCurrentAnimatorStateInfo(0).IsTag("Death")&&agent.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !agent.animator.IsInTransition(0))
        {
            agent.Remove();
        }
    

    }

    public void Exit()
    {

    }
}
