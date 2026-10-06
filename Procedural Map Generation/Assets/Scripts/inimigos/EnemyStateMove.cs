using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class EnemyStateMove: IState
{
    EnemyAgent agent;
    int chace;
    float time;
    Animator animator;
    Vector3 dir;
    SkinnedMeshRenderer renderer;
    float rotationSpeed = 5f;
    public EnemyStateMove(EnemyAgent agent, SkinnedMeshRenderer renderer)
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
    }

    public void Execute(float delta)
    {
        //Debug.Log("move executando");
        dir = this.agent.control.Move();
        dir.y = 0;
        if (agent.player)
        {
            if((agent.player.transform.position - agent.transform.position).magnitude < 1.5f)
            {
                if (chace > 80)
                {
                    agent.ChangeState(new EnemyStateMove(agent, renderer));
                }
                else
                {
                    agent.ChangeState(new EnemyStateAttack(agent, renderer));
                }
                return;
            }
            time = Random.Range(5, 10);
            Vector3 arc = agent.player.transform.position;
            arc.x = agent.player.transform.position.x + Mathf.Cos(Time.time) * 10f;
            arc.z = agent.player.transform.position.z + Mathf.Sin(Time.time) * 10f;
            dir += (arc - agent.transform.position).normalized;
        }
        else
        {
            if(Random.Range(0, 100) < 10 && time<0)
            {
                agent.ChangeState(new EnemyStateIdle(agent, renderer));
            }
            time -= delta;
            
        }
        Quaternion toRotation = Quaternion.LookRotation(dir, Vector3.up);
        agent.transform.rotation = Quaternion.Slerp(
            agent.transform.rotation,
            toRotation,
            rotationSpeed * Time.deltaTime
        );
        agent.cc.SimpleMove(dir * 0.5f * Time.timeScale);
    }

    public void Exit()
    {
        agent.GetNeighbours().Clear();
    }
    
}
