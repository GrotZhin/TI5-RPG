using UnityEngine;

public class Atackinimigo: Istateinimigos
{
    inimigoagente agente;
    float time;
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
        time = 2;
    }

    public void Execute(float delta)
    {
        Debug.Log("atack executando");
        time -= delta;
        if (time < 0)
        {
            agente.ChangeState(new Moveinimigo(agente, renderer));
        }
    }

    public void Exite()
    {
        Debug.Log("IDLE saiu");
    }

}
