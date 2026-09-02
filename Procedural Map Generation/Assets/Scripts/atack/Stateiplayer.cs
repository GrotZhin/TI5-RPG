using UnityEngine;

public class Stateiplayer : Istateinimigos
{
    agenteplayer agente;
    public Stateiplayer(agenteplayer agente) 
    {
        this.agente = agente;
    }
    public void Enter()
    {
        Debug.Log("Entrou");
    }
    public void Execute(float delta)
    {
        Debug.Log("Executando");
    }
    public void Exite()
    {
        Debug.Log("Saiu");
    }
}
