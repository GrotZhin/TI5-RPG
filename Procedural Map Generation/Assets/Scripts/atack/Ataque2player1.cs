using System;
using UnityEngine;

public class Ataque2player : IState
{
    PlayerAgent agente;
    float time;
    Action aterior;
    bool mudar = true;
    public Ataque2player(PlayerAgent agente, Action aterior)
    {
        this.agente = agente;
        this.aterior = aterior;
    }
    public void Enter()
    {
        //agente.GetComponent<MeshRenderer>().material.color = Color.white;
        time = 1;
        InputInfo.OnAttackEvent -= aterior;
        mudar = true;
        foreach (EnemyAgent agen in agente.agents)
        {
            agen.ChangeState(new EnemyStateDano(agen, agen.renderer));
            agen.control.dano(5);
            if (agen.control.vida <= 0)
            {
                agen.gameObject.SetActive(false);
            }
        }
        
    }

    public void Execute(float delta)
    {
        time -= delta;
        if (time < 0)
        {
            agente.ChangeState(new PlayerStateIdle(agente));
        }
    }

    public void Exit()
    {
        InputInfo.OnAttackEvent += agente.bater;
    }
}
