using UnityEngine;
using UnityEngine.Events;

public class Fogeinimigo : Istateinimigos
{
    inimigoagente agente;
    Animator animator;
    Vector3 target;
    SkinnedMeshRenderer renderer;
    float rotationSpeed = 10;
    public Fogeinimigo(inimigoagente agent, SkinnedMeshRenderer renderer)
    {
        this.agente = agent;
        this.renderer = renderer;
    }

    public void Enter()
    {
        Debug.Log("foge entrou");
        renderer.material.color = Color.purple;
        target = (Random.insideUnitSphere * 5) + agente.transform.position;
        target.z = -Mathf.Abs(target.z);
        target -= agente.transform.position;
        target.y = agente.transform.position.y;
        animator = agente.GetComponent<Animator>();

    }

    public void Execute(float delta)
    {
        Debug.Log("foge executando");
        Vector3 dir = agente.transform.position - agente.player.transform.position;
        animator.SetFloat("Input Magnitude", 1, 0.05f, delta);
        Quaternion toRotation = Quaternion.LookRotation(target, Vector3.up);
        agente.transform.rotation = Quaternion.RotateTowards(agente.transform.rotation, toRotation, rotationSpeed);
        animator.SetBool("IsMoving", true);
        if(dir.magnitude > 5f)
        {
            agente.ChangeState(new Idleinimigo(agente, renderer));
        }
    }

    public void Exite()
    {
        Debug.Log("foge saiu");
        animator.SetBool("IsMoving", false);
    }

}
