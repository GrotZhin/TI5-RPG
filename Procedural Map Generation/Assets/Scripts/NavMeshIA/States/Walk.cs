
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;


public class Walk : IState
{
    Agent agent;
    private Vector2 velocity;
    private Vector2 smoothDeltaPosition;

    public Walk(Agent agent)
    {
        this.agent = agent;

    }
    public void Enter()
    {
        Debug.Log("Entrei no state dnv");
        agent.animator.applyRootMotion = true;
        agent.agent.updatePosition = false;
        agent.agent.updateRotation = true;

       // agent.Invoke("Move", 1);
        agent.StartCoroutine(Move());


        //agent.StartCoroutine(SortState(2));
    }

    private void SyncronizeAnimator()
    {
        Vector3 worldDeltaPosition = agent.agent.nextPosition - agent.transform.position;
        worldDeltaPosition.y = 0;

        float dx = Vector3.Dot(agent.transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(agent.transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        velocity = smoothDeltaPosition / Time.deltaTime;
        if (agent.agent.remainingDistance <= agent.agent.stoppingDistance)
        {
            velocity = Vector2.Lerp(Vector2.zero, velocity, agent.agent.remainingDistance / agent.agent.stoppingDistance);
        }
        bool shouldMove = velocity.magnitude > 0.5f
            && agent.agent.remainingDistance > agent.agent.stoppingDistance;

        agent.animator.SetBool("andar", shouldMove);
        agent.animator.SetFloat("locomotion", velocity.magnitude);

        float deltaMagnitude = worldDeltaPosition.magnitude;

        if (deltaMagnitude > agent.agent.radius / 2f)
        {
            agent.transform.position = Vector3.Lerp(agent.animator.rootPosition, agent.agent.nextPosition, smooth);
        }
    }
    IEnumerator Move()
    {

        agent.agent.SetDestination(agent.group.formation.GetPosition(agent));
        SyncronizeAnimator();
        float delay = agent.agent.remainingDistance / agent.agent.speed;
        yield return new WaitForSeconds(delay);
        agent.StartCoroutine(Move());

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
