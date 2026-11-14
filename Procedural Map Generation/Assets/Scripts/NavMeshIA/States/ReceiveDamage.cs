using System.Collections;
using UnityEngine;


public class ReceiveDamage : IState
{
    Agent agent;

    public ReceiveDamage(Agent agent)
    {
        this.agent = agent;

    }
    public void Enter()
    {
        Debug.Log("Entrei no state dnv");
        agent.animator.SetBool("andar", true);

        agent.StartCoroutine(SortState(2));
    }

    IEnumerator SortState(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (Random.value > 0.9f) agent.ChangeState(new Walk(agent));
        else
        {
        Debug.Log("Nao entrei no state");    
        agent.StartCoroutine(SortState(2));
        } 

    }
    public void Execute(float delta)
    {


    }

    public void Exit()
    {
        
    }
}
