using UnityEngine;
using UnityEngine.Events;

public class Moveinimigo: Istateinimigos
{
    inimigoagente agente;
    int chace;
    UnityAction<float> Move;
    CharacterController cc;
    Animator animator;
    Vector3 target;
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

        cc = agente.GetComponent<CharacterController>();
        animator = agente.GetComponent<Animator>();
    }

    public void Execute(float delta)
    {
        Debug.Log("move executando");
        Vector3 dir = target - agente.transform.position;
        float inputMagnitude = Mathf.Clamp01(dir.magnitude);

        animator.SetFloat("Input Magnitude", inputMagnitude, 0.05f, delta);
        ySpeed += Physics.gravity.y * delta;

        if (dir != Vector3.zero)
        {
            animator.SetBool("IsMoving", true);

            Quaternion toRotation = Quaternion.LookRotation(dir, Vector3.up);
            
            float angle = Vector3.SignedAngle(agente.transform.forward, dir, Vector3.up);

            agente.transform.rotation = Quaternion.RotateTowards(agente.transform.rotation, toRotation, rotationSpeed);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
        Debug.Log((agente.player.transform.position - agente.transform.position).magnitude);
        if (dir.magnitude < 0.3)
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
    }

    public void Exite()
    {
        Debug.Log("move saiu");
        animator.SetBool("IsMoving", false);
    }
    
}
