using UnityEngine;

public class Idleplayer : Istateinimigos
{
    agenteplayer agente;
    public Idleplayer(agenteplayer agente)
    {
        this.agente = agente;
    }
    public void Enter()
    {
        agente.GetComponent<MeshRenderer>().material.color = Color.gray;
    }

    public void Execute(float delta)
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            agente.ChangeState(new Ataque1player(agente));
        }*/
    }

    public void Exite()
    {
       
    }
}
