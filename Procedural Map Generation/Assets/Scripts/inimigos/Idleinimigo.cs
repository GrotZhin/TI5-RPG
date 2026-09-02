using UnityEngine;

public class Idleinimigo : Istateinimigos
{
    inimigoagente agente;
    float time;
    SkinnedMeshRenderer renderer;
    public Idleinimigo(inimigoagente agent, SkinnedMeshRenderer renderer)
    {
        this.agente = agent;
        this.renderer = renderer;
    }

    public void Enter()
    {
        Debug.Log("IDLE entrou");
        time = 2;
        renderer.material.color = Color.gray;
    }

    public void Execute(float delta)
    {
        Debug.Log("IDLE executando");
        time -= delta;
        if (time < 0)
        {
            agente.ChangeState(new Atackinimigo(agente, renderer));
        }
    }

    public void Exite()
    {
        Debug.Log("IDLE saiu");
    }
}
