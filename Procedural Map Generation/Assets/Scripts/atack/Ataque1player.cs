using UnityEngine;

public class Ataque1player : Istateinimigos
{
    agenteplayer agente;
    float time;
    public Ataque1player(agenteplayer agente)
    {
        this.agente = agente;
    }
    public void Enter()
    {
        //agente.GetComponent<MeshRenderer>().material.color = Color.magenta;
        time = 2;
    }

    public void Execute(float delta)
    {
        time -= delta;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            agente.ChangeState(new Ataque2player(agente));
        }
        else if (time < 0)
        {
            agente.ChangeState(new Idleplayer(agente));
        }
        }

    public void Exite()
    {
       
    }
}
