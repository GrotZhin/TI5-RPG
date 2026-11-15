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
    }


    public void Execute(float delta)
    {
        time -= Time.deltaTime;
        if (agent.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !agent.animator.IsInTransition(0) && time <= 0)
        {
            agent.Remove();
        }
    

    }

    public void Exit()
    {

    }
}
