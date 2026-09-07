using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class inimigoagente : MonoBehaviour
{
    Istateinimigos state;
    public agenteplayer player;
    public SkinnedMeshRenderer renderer;
    List<inimigoagente> vizinho = new List<inimigoagente>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        if (Geralageteinimigo.Geralinimigo != null)
        {
            Geralageteinimigo.Geralinimigo.Addageteinimigo(this);
        }
        state = new Idleinimigo(this, renderer);
        state?.Enter();
    }

    // Update is called once per frame
    void Update()
    {
        state?.Execute(Time.deltaTime);
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            this.ChangeState(new Danoinimigo(this));
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("inimigo") && (this.state is Moveinimigo or Fogeinimigo))
        {
            vizinho.Add(other.GetComponent<inimigoagente>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("inimigo") && (this.state is Moveinimigo or Fogeinimigo))
        {
            vizinho.Remove(other.GetComponent<inimigoagente>());
        }
    }

    public void ChangeState(Istateinimigos state)
    {
        this.state.Exite();
        this.state = state;
        state.Enter();
    }

    public List<inimigoagente> passavizinho()
    {
        return vizinho;
    }
}
