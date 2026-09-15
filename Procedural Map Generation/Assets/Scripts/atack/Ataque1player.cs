using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ataque1player : IState
{
    PlayerAgent agente;
    float time;
    InputAction.CallbackContext context;
    public Ataque1player(PlayerAgent agente)
    {
        this.agente = agente;
    }
    public void Enter()
    {
        Debug.Log("atack1");
        InputInfo.OnAttackEvent += bater;
        time = 2;
    }

    public void Execute(float delta)
    {
        time -= delta;
        foreach(EnemyAgent agen in agente.agents)
        {
            agen.ChangeState(new EnemyStateDano(agen, agen.renderer));
        }
        if (time < 0)
        {
            InputInfo.OnAttackEvent += agente.bater;
            agente.ChangeState(new PlayerStateIdle(agente));
        }
        }

    public void Exit()
    {
        InputInfo.OnAttackEvent -= bater;
    }
    public void bater()
    {
        agente.ChangeState(new Ataque2player(agente));
    }
}
