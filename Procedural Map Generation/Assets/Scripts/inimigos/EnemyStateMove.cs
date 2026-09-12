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
        dirtmp = (Random.insideUnitSphere * 2);
        animator = agent.GetComponent<Animator>();
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
        }
        animator.SetFloat("Input Magnitude", dir.magnitude, 0.05f, delta);
        Quaternion toRotation = Quaternion.LookRotation(dir, Vector3.up);
        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, toRotation, rotationSpeed);
        animator.SetBool("IsMoving", true);
        agent.cc.Move(animator.deltaPosition);
    }

    public void Exit()
    {
        //Debug.Log("move saiu");
        animator.SetBool("IsMoving", false);
        agent.GetNeighbours().Clear();
    }
    
}
