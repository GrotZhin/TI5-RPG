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
        Debug.Log("foge entrou");
        renderer.material.color = Color.purple;
        agent.control.seek = 0.01f;
        agent.control.separate = 0.9f;
        agent.control.align = 0.08f;
        agent.control.cohesion = 0.12f;
        agent.control.avoid = 0.8f;


        /*target = (Random.insideUnitSphere * 5) + agent.transform.position;
        target.z = -Mathf.Abs(target.z);
        target.y = agent.transform.position.y;*/
        animator = agent.GetComponent<Animator>();
        //animator.SetBool("IsMoving", true);
    }

    public void Execute(float delta)
    {
        
        if (agent.player)
        {
            //Debug.Log("foge executando");
            Vector3 dir = agent.control.Move() + (2 * (agent.transform.position - agent.player.transform.position).normalized);
            target = dir.normalized;
            //animator.SetFloat("Input Magnitude", target.magnitude, 0.05f, delta);
            Quaternion toRotation = Quaternion.LookRotation(target, Vector3.up);
            agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, toRotation, rotationSpeed);
            agent.cc.SimpleMove(target * 1f * Time.timeScale);
            animator.SetFloat("velocidadeanimaçao", 2);
            //Debug.Log(agent.player + "   " + agent);
            
        }
        else
        {
            agent.ChangeState(new EnemyStateIdle(agent, renderer));
        }
    }

    public void Exit()
    {
        //Debug.Log("foge saiu");
        //animator.SetBool("IsMoving", false);
        //animator.SetFloat("Input Magnitude", 0, 0f, 0);
        animator.SetFloat("velocidadeanimaçao", 1);
        agent.GetNeighbours().Clear();
    }

}
