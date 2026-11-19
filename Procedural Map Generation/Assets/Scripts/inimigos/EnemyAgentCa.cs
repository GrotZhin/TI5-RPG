using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAgentCa : MonoBehaviour
{
    IState state;
    public PlayerAgent player = null;
    public SkinnedMeshRenderer renderer;
    public EnemyAgentControlCa control;
    public List<EnemyAgentCa> neighbours = new List<EnemyAgentCa>();
    public SphereCollider collider;
    public NavMeshAgent nav;
    void OnEnable()
    {
        //if (Geralageteinimigo.Geralinimigo != null)
        //{
        //    Geralageteinimigo.Geralinimigo.Addageteinimigo(this);
        //}
        control = new EnemyAgentControlCa(this);
        state = new EnemyStateIdleCa(this, renderer);
        collider = GetComponent<SphereCollider>();
        state?.Enter();
        nav.avoidancePriority = Random.Range(30, 100);
        
    }

    void Update()
    {
        state?.Execute(Time.deltaTime);
        //Debug.Log(state.ToString());
        if(collider.radius != control.radius)
            collider.radius = control.radius;


        if (neighbours.Contains(null))
        {
            neighbours.Remove(null);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("aaaaaaa");
        if(other.CompareTag("Enemy") && (this.state is EnemyStateMoveCa or EnemyStateFleeCa))
        {
            if(!neighbours.Contains(other.GetComponent<EnemyAgentCa>()))
                neighbours.Add(other.GetComponent<EnemyAgentCa>());
        }
        if (other.CompareTag("Player") &&  player == null)
        {
            player = other.GetComponent<PlayerAgent>();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && (this.state is EnemyStateMoveCa or EnemyStateFleeCa))
        {
            neighbours.Remove(other.GetComponent<EnemyAgentCa>());
        }
        
    }
    private void OnTriggerStay(Collider other)
    {
        
        if (other != null)
        {
            if (player == null)
            {
                if (other.CompareTag("Player"))
                {
                    player = other.GetComponent<PlayerAgent>();
                }
            }
            if (other.CompareTag("Enemy") && (this.state is EnemyStateMoveCa or EnemyStateFleeCa))
            {
                if (!neighbours.Contains(other.GetComponent<EnemyAgentCa>()))
                {
                    neighbours.Add(other.GetComponent<EnemyAgentCa>());
                }
                
            }
        }
        
    }
    
    public void ChangeState(IState state)
    {
        this.state?.Exit();
        this.state = state;
        state?.Enter();
    }

    public List<EnemyAgentCa> GetNeighbours()
    {
        return neighbours;
    }
}
