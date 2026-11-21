using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class EnemyStateMoveCa: IState
{
    EnemyAgentCa agent;
    int chace;
    float time;
    Animator animator;
    Vector3 dir;
    SkinnedMeshRenderer renderer;
    float rotationSpeed = 5f;
    public EnemyStateMoveCa(EnemyAgentCa agent, SkinnedMeshRenderer renderer)
    {
        this.agent = agent;
        this.renderer = renderer;
    }

    public void Enter()
    {
        //Debug.Log("Move entrou");
        agent.control.seek = 0.9f;
        agent.control.separate = 0.65f;
        agent.control.align = 0.08f;
        agent.control.cohesion = 0.12f;
        agent.control.avoid = 0.8f;

        renderer.material.color = Color.blue;
        chace = Random.Range(0, 100);
        time = Random.Range(5, 10);
        animator = agent.GetComponent<Animator>();
        //animator.SetBool("IsMoving", true);
        animator.SetBool("andar", true);

        
        agent.nav.SetDestination(agent.transform.position + (Random.insideUnitSphere * 5));
        agent.nav.velocity = Vector3.one;
    }

    public void Execute(float delta)
    {
        //Debug.Log("move executando");
        
        if (agent.player)
        {
            if((agent.player.transform.position - agent.transform.position).magnitude < 1.5f)
            {
                if (chace > 80)
                {
                    agent.ChangeState(new EnemyStateMoveCa(agent, renderer));
                }
                else
                {
                    agent.ChangeState(new EnemyStateAttackCa(agent, renderer));
                }
                return;
            }
            agent.nav.SetDestination(agent.player.transform.position + Random.insideUnitSphere);
        }
        else
        {
            if (agent.nav.remainingDistance <= agent.nav.stoppingDistance)
            {
                
                if (Random.Range(0, 100) < 10)
                {
                    agent.ChangeState(new EnemyStateIdleCa(agent, renderer));
                }
                else
                {
                    agent.nav.SetDestination(agent.transform.position + (Random.insideUnitSphere * 15));
                }
            }
            else
            {
                //dir = this.agent.control.Move();
                //dir.y = 0;
                //agent.nav.velocity = this.agent.control.Move(1, agent.nav.desiredVelocity);
                //Debug.Log(agent.nav.velocity + "  " + this.agent.control.Move(1, agent.nav.desiredVelocity));
            }
            time -= delta;
        }
        
        
    }

    public void Exit()
    {
        agent.GetNeighbours().Clear();
        agent.nav.ResetPath();
    }
    
}
