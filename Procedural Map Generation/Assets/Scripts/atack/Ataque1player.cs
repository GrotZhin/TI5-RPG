using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ataque1player : IState
{
    PlayerAgent agente;
    float time;
    InputAction.CallbackContext context;
    bool mudar = true;
    public Ataque1player(PlayerAgent agente)
    {
        this.agente = agente;
    }
    public void Enter()
    {
        
        InputInfo.OnAttackEvent -= agente.bater;
        time = 10;
        mudar = true;
        foreach (EnemyAgent agen in agente.agents)
        {
            agen.ChangeState(new EnemyStateDano(agen, agen.renderer));
            if (agen.control.vida <= 0)
            {
                agen.gameObject.SetActive(false);
            }
        }
    }

    public void Execute(float delta)
    {
        time -= delta;
        
        if (time <= 0)
        {
            InputInfo.OnAttackEvent += agente.bater;
            agente.ChangeState(new PlayerStateIdle(agente));
        }
        else if (time < 8 && mudar)
        {
            mudar = false;
            InputInfo.OnAttackEvent += bater;
        }
    }
    public void Exit()
    {
        InputInfo.OnAttackEvent -= bater;
    }
    public void bater()
    {
        agente.ChangeState(new Ataque2player(agente,this.bater));
    }
}
