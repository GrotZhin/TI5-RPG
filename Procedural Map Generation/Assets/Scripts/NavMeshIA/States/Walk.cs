
using System.Collections;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;


public class Walk : IState
{
    Agent agent;
    private Vector2 velocity;
    private Vector2 smoothDeltaPosition;
    private Coroutine coroutine;

    public Walk(Agent agent)
    {
        this.agent = agent;

    }
    public void Enter()
    {
        Debug.Log("Entrei no walk stae");
        agent.animator.applyRootMotion = true;
        agent.agent.updatePosition = false;
        agent.agent.updateRotation = true;

        // agent.Invoke("Move", 1);
        coroutine = agent.StartCoroutine(Move());


        agent.StartCoroutine(SortState(4));
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

        if (deltaMagnitude > agent.agent.radius)
        {
            agent.transform.position = Vector3.Lerp(agent.animator.rootPosition, agent.agent.nextPosition, smooth);
        }
    }
    IEnumerator Move()
    {


        agent.agent.SetDestination(agent.group.formation.GetPosition(agent));


        yield return new WaitUntil(() => !agent.agent.pathPending && agent.agent.remainingDistance <= 0.7f);



        agent.StartCoroutine(Move());

    }

    IEnumerator SortState(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (Random.value > 0.6f) agent.ChangeState(new GiveDamage(agent));
        else
        {

            agent.StartCoroutine(SortState(4));
        }

    }
    public void Execute(float delta)
    {

        SyncronizeAnimator();
    }

    public void Exit()
    {
        if (coroutine != null) agent.StopAllCoroutines();
    }
}
