using UnityEngine;
using UnityEngine.Events;

public class Fogeinimigo : Istateinimigos
{
    inimigoagente agente;
    int chace;
    UnityAction<float> Move;
    CharacterController cc;
    Animator animator;
    Vector3 target;
    SkinnedMeshRenderer renderer;
    float ySpeed, rotationSpeed = 10;
    public Fogeinimigo(inimigoagente agent, SkinnedMeshRenderer renderer)
    {
        this.agente = agent;
        this.renderer = renderer;
    }

    public void Enter()
    {
        Debug.Log("foge entrou");
        renderer.material.color = Color.purple;
        chace = Random.Range(0, 100);
        target = (Random.insideUnitSphere) + agente.player.transform.position;
        target.y = agente.transform.position.y;
        cc = agente.GetComponent<CharacterController>();
        animator = agente.GetComponent<Animator>();
    }

    public void Execute(float delta)
    {
        Debug.Log("foge executando");
        Vector3 dir = agente.transform.position - target;
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
        if (dir.magnitude > 6)
        {
            if (chace > 70)
            {
                agente.ChangeState(new Moveinimigo(agente, renderer));
            }
            else
            {
                agente.ChangeState(new Idleinimigo(agente, renderer));
            }
        }
    }

    public void Exite()
    {
        Debug.Log("foge saiu");
        animator.SetBool("IsMoving", false);
    }

}
