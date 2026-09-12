using UnityEngine;
using UnityEngine.Events;

public class EnemyStateFlee : IState
{
    EnemyAgent agent;
    Animator animator;
    Vector3 target;
    SkinnedMeshRenderer renderer;
    float rotationSpeed = 10;
    public EnemyStateFlee(EnemyAgent agent, SkinnedMeshRenderer renderer)
    {
        this.agent = agent;
        this.renderer = renderer;
    }

    public void Enter()
    {
        //Debug.Log("foge entrou");
        renderer.material.color = Color.purple;
        target = (Random.insideUnitSphere * 5) + agent.transform.position;
        target.z = -Mathf.Abs(target.z);
        target.y = agent.transform.position.y;
        animator = agent.GetComponent<Animator>();

    }

    public void Execute(float delta)
    {
        //Debug.Log("foge executando");
        Vector3 dir = agent.control.Move();
        target = dir;
        animator.SetFloat("Input Magnitude", target.magnitude, 0.05f, delta);
        Quaternion toRotation = Quaternion.LookRotation(target, Vector3.up);
        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, toRotation, rotationSpeed);
        animator.SetBool("IsMoving", true);
        if (dir.magnitude > 5f)
        {
            agent.ChangeState(new EnemyStateIdle(agent, renderer));
        }
    }

    public void Exit()
    {
        //Debug.Log("foge saiu");
        animator.SetBool("IsMoving", false);
        agent.GetNeighbours().Clear();
    }

}
