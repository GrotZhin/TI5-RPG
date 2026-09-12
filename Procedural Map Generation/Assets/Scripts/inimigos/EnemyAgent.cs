using System.Collections.Generic;
using UnityEngine;

public class EnemyAgent : MonoBehaviour
{
    IState state;
    public PlayerAgent player = null;
    public SkinnedMeshRenderer renderer;
    public EnemyAgentControl control;
    public CharacterController cc;
    public List<EnemyAgent> neighbours = new List<EnemyAgent>();
    public SphereCollider collider;
    
    void OnEnable()
    {
        //if (Geralageteinimigo.Geralinimigo != null)
        //{
        //    Geralageteinimigo.Geralinimigo.Addageteinimigo(this);
        //}
        control = new EnemyAgentControl(this);
        state = new EnemyStateIdle(this, renderer);
        cc = GetComponent<CharacterController>();
        collider = GetComponent<SphereCollider>();
        state?.Enter();
    }

    void Update()
    {
        state?.Execute(Time.deltaTime);
        if(collider.radius != control.radius)
            collider.radius = control.radius;
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            this.ChangeState(new Danoinimigo(this));
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy") && (this.state is EnemyStateMove or EnemyStateFlee) && other.gameObject != gameObject)
        {
            if(!neighbours.Contains(other.GetComponent<EnemyAgent>()))
                neighbours.Add(other.GetComponent<EnemyAgent>());
        }
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerAgent>();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && (this.state is EnemyStateMove or EnemyStateFlee))
        {
            neighbours.Remove(other.GetComponent<EnemyAgent>());
        }
        if (other.CompareTag("Player"))
        {
            player = null;
        }
    }

    public void ChangeState(IState state)
    {
        this.state?.Exit();
        this.state = state;
        state?.Enter();
    }

    public List<EnemyAgent> GetNeighbours()
    {
        return neighbours;
    }
}
