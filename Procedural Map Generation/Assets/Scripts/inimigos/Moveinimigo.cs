using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Moveinimigo: Istateinimigos
{
    inimigoagente agente;
    int chace;
    float time;
    Animator animator;
    Vector3 target, dirtmp;
    SkinnedMeshRenderer renderer;
    float rotationSpeed = 5f;
    public Moveinimigo(inimigoagente agent, SkinnedMeshRenderer renderer)
    {
        this.agente = agent;
        this.renderer = renderer;
    }

    public void Enter()
    {
        //Debug.Log("Move entrou");
        agente.passavizinho().Clear();
        renderer.material.color = Color.blue;
        chace = Random.Range(0, 100);
        dirtmp = (Random.insideUnitSphere * 2);
        animator = agente.GetComponent<Animator>();
        time = -1f;
    }

    public void Execute(float delta)
    {
        //Debug.Log("move executando");
        
        Vector3 dir = Geralageteinimigo.Geralinimigo.PesoMover(Geralageteinimigo.Geralinimigo.Mover(this.dirtmp, this.agente),1f, Geralageteinimigo.Geralinimigo.Separar(this.agente), 1f);
        //Debug.Log((agente.player.transform.position - agente.transform.position).magnitude);
        if((agente.player.transform.position - agente.transform.position).magnitude < 1.5)
        {
            if (chace > 80)
            {
                agente.ChangeState(new Moveinimigo(agente, renderer));
            }
            else
            {
                agente.ChangeState(new Atackinimigo(agente, renderer));
            }
        }
        else
        {
            animator.SetFloat("Input Magnitude", dir.magnitude, 0.05f, delta);
            Quaternion toRotation = Quaternion.LookRotation(dir, Vector3.up);
            agente.transform.rotation = Quaternion.RotateTowards(agente.transform.rotation, toRotation, rotationSpeed);
            animator.SetBool("IsMoving", true);
        }
    }

    public void Exite()
    {
        //Debug.Log("move saiu");
        animator.SetBool("IsMoving", false);
    }
    
}
