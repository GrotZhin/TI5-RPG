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
    Vector3 target, dirtmp;
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
        
        renderer.material.color = Color.blue;
        chace = Random.Range(0, 100);
        time = Random.Range(5, 10);
        dirtmp = (Random.insideUnitSphere * 2);
        animator = agent.GetComponent<Animator>();
        animator.SetBool("IsMoving", true);
    }

    public void Execute(float delta)
    {
        //Debug.Log("move executando");
        
        Vector3 dir = agent.control.Move();
        //Debug.Log((agent.player.transform.position - agent.transform.position).magnitude);
        if (agent.player)
        {
            if((agent.player.transform.position - agent.transform.position).magnitude < 1.5)
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
        }
        else
        {
            if(Random.Range(0, 100) < 10 && time<0)
            {
                agent.ChangeState(new EnemyStateIdle(agent, renderer));
            }
            time -= delta;
        }
        animator.SetFloat("Input Magnitude", dir.magnitude, 0.05f, delta);
        Quaternion toRotation = Quaternion.LookRotation(dir, Vector3.up);
        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, toRotation, rotationSpeed);
        
        agent.cc.Move(animator.deltaPosition);
    }

    public void Exit()
    {
        //Debug.Log("move saiu");
        animator.SetBool("IsMoving", false);
        animator.SetFloat("Input Magnitude", 0, 0f, 0);
        agent.GetNeighbours().Clear();
    }
    
}
