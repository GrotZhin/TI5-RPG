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
        // [SAVE-INTEGRATION] dano escala com o bônus de ataque dos itens da loja.
        float mult = PlayerStats.Instance != null ? PlayerStats.Instance.AttackMultiplier : 1f;
        int dmg = Mathf.Max(1, Mathf.RoundToInt(5 * mult));

        foreach (EnemyAgent agen in agente.agents)
        {
            agen.ChangeState(new EnemyStateDano(agen, agen.renderer));
            agen.control.dano(dmg);
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
