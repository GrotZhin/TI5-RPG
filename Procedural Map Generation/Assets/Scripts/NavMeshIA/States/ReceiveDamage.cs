using System.Collections;
using UnityEngine;


public class ReceiveDamage : IState
{
    Agent agent;
    SkinnedMeshRenderer renderer;
    float damage;

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
        if(!state.IsName("damage"))
            agent.animator.SetTrigger("damage");
        
        GameFeel.gameFeel.FlashEffect(0.2f, renderer);
        bool isDead = agent.stats.Damage(damage);
        if (isDead) agent.ChangeState(new Death(agent));

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
