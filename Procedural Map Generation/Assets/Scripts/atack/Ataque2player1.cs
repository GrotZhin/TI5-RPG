using UnityEngine;

public class Ataque2player : IState
{
    PlayerAgent agente;
    float time;
    public Ataque2player(PlayerAgent agente)
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
            agente.ChangeState(new PlayerStateIdle(agente));
        }
    }

    public void Exit()
    {
       
    }
}
