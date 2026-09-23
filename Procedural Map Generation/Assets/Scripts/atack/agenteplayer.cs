using System.Collections.Generic;
using UnityEngine;

public class PlayerAgent : MonoBehaviour
{
    IState state;
    public float lifePoints = 100;
    public List<EnemyAgent> agents = new List<EnemyAgent>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputInfo.OnAttackEvent += bater;
        state = new PlayerStateIdle(this);
        state?.Enter();
    }

    // Update is called once per frame
    void Update()
    {
        state?.Execute(Time.deltaTime);
    }

    public void ChangeState(IState state)
    {
        this.state?.Exit();
        this.state = state;
        state.Enter();
    }
    public void bater()
    {
        InputInfo.OnAttackEvent -= bater;
        ChangeState(new Ataque1player(this));
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            agents.Add(other.GetComponent<EnemyAgent>());

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            agents.Remove(other.GetComponent<EnemyAgent>());

        }
    }

    public void Damage(float value)
    {
        lifePoints -= value;
    }
}
