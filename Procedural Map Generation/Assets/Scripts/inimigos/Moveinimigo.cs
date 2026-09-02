using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Moveinimigo: Istateinimigos
{
    inimigoagente agente;
    int chace;
    CharacterController cc;
    Animator animator;
    Vector3 target, dirtmp;
    SkinnedMeshRenderer renderer;
    float ySpeed, rotationSpeed = 10;
    public Moveinimigo(inimigoagente agent, SkinnedMeshRenderer renderer)
    {
        this.agente = agent;
        this.renderer = renderer;
    }

    public void Enter()
    {
        Debug.Log("Move entrou");
        renderer.material.color = Color.blue;
        chace = Random.Range(0, 100);
        target = (Random.insideUnitSphere * 2)+ agente.player.transform.position;
        target.y = agente.transform.position.y;
        dirtmp = target - agente.transform.position;
        cc = agente.GetComponent<CharacterController>();
        animator = agente.GetComponent<Animator>();
    }

    public void Execute(float delta)
    {
        Debug.Log("move executando");
        Vector3 dir = target - agente.transform.position;
        //Debug.Log((agente.player.transform.position - agente.transform.position).magnitude);
        if (dir.magnitude < 0.2f)
        {
            if (chace > 5)
            {
                agente.ChangeState(new Moveinimigo(agente, renderer));
            }
            else
            {
                agente.ChangeState(new Atackinimigo(agente, renderer));
            }
        }
        else if((agente.player.transform.position - agente.transform.position).magnitude < 1)
        {
            agente.ChangeState(new Fogeinimigo(agente, renderer));
        }
        else
        {
            animator.SetFloat("Input Magnitude", 1, 0.05f, delta);
            Quaternion toRotation = Quaternion.LookRotation(dir, Vector3.up);
            agente.transform.rotation = Quaternion.RotateTowards(agente.transform.rotation, toRotation, rotationSpeed);
            animator.SetBool("IsMoving", true);
        }
    }

    public void Exite()
    {
        Debug.Log("move saiu");
        animator.SetBool("IsMoving", false);
    }
    
}
