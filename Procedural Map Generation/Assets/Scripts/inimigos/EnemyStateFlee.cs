using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class EnemyStateFlee : IState
{
    EnemyAgent agent;
    int chace;
    float time;
    Animator animator;
    Vector3 dir;
    SkinnedMeshRenderer renderer;
    float rotationSpeed = 5f;
    public EnemyStateFlee(EnemyAgent agent, SkinnedMeshRenderer renderer)
    {
        this.agent = agent;
        this.renderer = renderer;
    }

    public void Enter()
    {
        //Debug.Log("Move entrou");
        agent.control.seek = 1f;
        agent.control.separate = 0.85f;
        agent.control.align = 0f;
        agent.control.cohesion = 0f;
        agent.control.avoid = 1f;

        renderer.material.color = Color.purple;
        chace = Random.Range(0, 100);
        time = Random.Range(5, 10);
        animator = agent.GetComponent<Animator>();
        //animator.SetBool("IsMoving", true);
        animator.SetBool("andar", true);
    }

    public void Execute(float delta)
    {
        //Debug.Log("move executando");

        if (agent.player)
        {
            if ((agent.player.transform.position - agent.transform.position).magnitude > 5f)
            {
                if (chace < 20)
                {
                    agent.ChangeState(new EnemyStateIdle(agent, renderer));
                }
                else if(chace < 80)
                {
                    agent.ChangeState(new EnemyStateTransformar(agent, renderer));
                }
                else
                {
                    agent.ChangeState(new EnemyStateMove(agent, renderer));
                }
                return;
            }
            dir = this.agent.control.Move(-1);
            dir.y = 0;
            Quaternion toRotation = Quaternion.LookRotation(dir, Vector3.up);
            agent.transform.rotation = Quaternion.Slerp(
                agent.transform.rotation,
                toRotation,
                rotationSpeed * Time.deltaTime
            );
            agent.cc.SimpleMove(dir * 1.5f * Time.timeScale);
            animator.SetFloat("velocidadeanimaçao", 3);
        }
    }

    public void Exit()
    {
        agent.GetNeighbours().Clear();
        animator.SetFloat("velocidadeanimaçao", 1);
    }

}
