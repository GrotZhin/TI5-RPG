using UnityEngine;

public class Danoinimigo : Istateinimigos
{
    inimigoagente agente;
    int chace;
    float time;
    SkinnedMeshRenderer renderer;
    public Danoinimigo(inimigoagente agent, SkinnedMeshRenderer renderer)
    {
        this.agente = agent;
        this.renderer = renderer;
    }

    public void Enter()
    {
        Debug.Log("Move entrou");
        renderer.material.color = Color.black;
        chace = Random.Range(0, 100);
        time = 2;
    }

    public void Execute(float delta)
    {
        time -= delta;
        Debug.Log("move executando");
        if (time < 0)
        {
            if (chace > 20)
            {
                agente.ChangeState(new Fogeinimigo(agente, renderer));
            }
            else
            {
                agente.ChangeState(new Atackinimigo(agente, renderer));
            }
        }
    }

    public void Exite()
    {
        Debug.Log("IDLE saiu");
    }
    
}
