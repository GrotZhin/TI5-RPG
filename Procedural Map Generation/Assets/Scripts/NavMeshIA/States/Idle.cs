using UnityEngine;
using System.Collections;

public class Idle : IState
{
    Agent agent;
    
    public Idle(Agent agent)
    {
        this.agent = agent;
        
    }
    public void Enter()
    {
       agent.StartCoroutine(SortState(3));
    }

    IEnumerator SortState(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    agent.ChangeState(new Walk(agent));
        
    }

    public void Execute(float delta)
    {
        
    }

    public void Exit()
    {
        
    }
}
