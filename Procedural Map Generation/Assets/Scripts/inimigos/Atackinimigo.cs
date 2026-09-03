using UnityEngine;

public class Atackinimigo: Istateinimigos
{
    inimigoagente agente;
    float time;
    int chace;
    SkinnedMeshRenderer renderer;
    public Atackinimigo(inimigoagente agent, SkinnedMeshRenderer renderer)
    {
        this.agente = agent;
        this.renderer = renderer;
    }

    public void Enter()
    {
        Debug.Log("Atack entrou");
        renderer.material.color = Color.red;
        chace = Random.Range(0, 100);
        time = 2;
    }

    public void Execute(float delta)
    {
        Debug.Log("atack executando");
        time -= delta;
        if (time < 0)
        {
            if (chace < 70)
            {
                agente.ChangeState(new Moveinimigo(agente, renderer));
            }
            else
            {
                agente.ChangeState(new Fogeinimigo(agente, renderer));
            }
        }
    }

    public void Exite()
    {
        Debug.Log("IDLE saiu");
    }

}
