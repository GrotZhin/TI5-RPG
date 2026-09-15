using UnityEngine;

public class PlayerStateIdle : IState
{
    PlayerAgent agente;
    public PlayerStateIdle(PlayerAgent agente)
    {
        this.agente = agente;
    }
    public void Enter()
    {
        //agente.GetComponent<MeshRenderer>().material.color = Color.gray;
    }

    public void Execute(float delta)
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            agente.ChangeState(new Ataque1player(agente));
        }*/
    }

    public void Exit()
    {
       
    }
}
