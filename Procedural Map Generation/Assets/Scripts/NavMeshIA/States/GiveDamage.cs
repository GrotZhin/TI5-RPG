using System.Collections;
using UnityEngine;


public class GiveDamage : IState
{
    Agent agent;
   

    public GiveDamage(Agent agent)
    {
        this.agent = agent;
      

    }
    public void Enter()
    {
        Debug.Log("Vou bater");
        agent.agent.SetDestination(PlayerStats.Instance.transform.position);

    }

    public void Execute(float delta)
    {


        AnimatorStateInfo state = agent.animator.GetCurrentAnimatorStateInfo(0);
        if (agent.agent.remainingDistance >= 0.1f)
            agent.animator.SetTrigger("attack");
        if (state.IsTag("Atk"))
        {
            agent.attackCheck.CheckCollision();
        }
        if (state.IsTag("Atk") && agent.animator.IsInTransition(0))
        {
            agent.ChangeState(new Walk(agent));
        }


    }

    public void Exit()
    {


    }
}
