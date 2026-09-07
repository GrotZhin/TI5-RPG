using UnityEngine;

public class Atackinimigo: Istateinimigos
{
    inimigoagente agente;
    float time;
    int chace;
    SkinnedMeshRenderer renderer;
    float rotationSpeed = 10;
    public Atackinimigo(inimigoagente agent, SkinnedMeshRenderer renderer)
    {
        this.agente = agent;
        this.renderer = renderer;
    }

    public void Enter()
    {
        //Debug.Log("Atack entrou");
        renderer.material.color = Color.red;
        Vector3 dir = agente.player.transform.position - agente.transform.position;
        dir.y = agente.transform.position.y;
        Quaternion toRotation = Quaternion.LookRotation(dir, Vector3.up);

        chace = Random.Range(0, 100);
        time = 2;
    }

    public void Execute(float delta)
    {
        //Debug.Log("atack executando");
        
        time -= delta;
        if (time < 0)
        {
            if (chace > 50)
            {
                agente.ChangeState(new Moveinimigo(agente, renderer));
            }
            else if(chace > 40)
            {
                agente.ChangeState(new Atackinimigo(agente, renderer));
            }
            else
            {
                agente.ChangeState(new Fogeinimigo(agente, renderer));
            }
        }
    }

    public void Exite()
    {
        //Debug.Log("IDLE saiu");
    }

}
