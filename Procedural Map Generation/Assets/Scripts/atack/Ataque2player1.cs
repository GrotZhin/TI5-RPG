using UnityEngine;

public class Ataque2player : Istateinimigos
{
    agenteplayer agente;
    float time;
    public Ataque2player(agenteplayer agente)
    {
        this.agente = agente;
    }
    public void Enter()
    {
        //agente.GetComponent<MeshRenderer>().material.color = Color.white;
        time = 1;
    }

    public void Execute(float delta)
    {
        if (time < 0)
        {
            agente.ChangeState(new Idleplayer(agente));
        }
    }

    public void Exite()
    {
       
    }
}
