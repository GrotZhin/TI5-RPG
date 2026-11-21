using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class EnemyStateFleeCa : IState
{
    EnemyAgentCa agent;
    int chace;
    float time;
    Animator animator;
    Vector3 dir;
    SkinnedMeshRenderer renderer;
    float rotationSpeed = 5f;
    public EnemyStateFleeCa(EnemyAgentCa agent, SkinnedMeshRenderer renderer)
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
                    agent.ChangeState(new EnemyStateIdleCa(agent, renderer));
                }
                else if(chace < 80)
                {
                    agent.ChangeState(new EnemyStateTransformarCa(agent, renderer));
                }
                else
                {
                    agent.ChangeState(new EnemyStateMoveCa(agent, renderer));
                }
                return;
            }
            agent.nav.speed = 3f;
            animator.SetFloat("velocidadeanimaçao", 3);
            agent.nav.SetDestination(agent.transform.position + (agent.player.transform.position - agent.transform.position).normalized * -1);
            
        }
    }

    public void Exit()
    {
        agent.GetNeighbours().Clear();
        animator.SetFloat("velocidadeanimaçao", 1);
        agent.nav.speed = 0.5f;
    }

}
