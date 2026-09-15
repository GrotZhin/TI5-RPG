using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

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
        agent.control.separate = 1f;
        agent.control.seek = 0f;
        agent.control.align = 0f;
        agent.control.avoid = 1f;
        agent.control.cohesion = 0f;
        /*target = (Random.insideUnitSphere * 5) + agent.transform.position;
        target.z = -Mathf.Abs(target.z);
        target.y = agent.transform.position.y;*/
        animator = agent.GetComponent<Animator>();
        animator.SetBool("IsMoving", true);
    }

    public void Execute(float delta)
    {
        //Debug.Log("foge executando");
        Vector3 dir = agent.control.Move(3);
        target = dir;
        animator.SetFloat("Input Magnitude", target.magnitude, 0.05f, delta);
        Quaternion toRotation = Quaternion.LookRotation(target, Vector3.up);
        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, toRotation, rotationSpeed);
        if (agent.player)
        {
            if ((agent.transform.position-agent.player.transform.position).magnitude > 5)
            {
                agent.ChangeState(new EnemyStateIdle(agent, renderer));
            }
        }
    }

    public void Exit()
    {
        Debug.Log("foge saiu");
        animator.SetBool("IsMoving", false);
        animator.SetFloat("Input Magnitude", 0, 0f, 0);
        agent.GetNeighbours().Clear();
    }

}
