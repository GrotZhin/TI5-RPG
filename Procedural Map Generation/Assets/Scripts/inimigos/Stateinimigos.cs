using UnityEngine;

public class Stateinimigos : Istateinimigos
{
    inimigoagente agente;
    public Stateinimigos(inimigoagente agente) 
    {
        this.agente = agente;
    }

    public void Enter()
    {
        Debug.Log("Entrou");
    }
    public void Execute(float delta)
    {
        Debug.Log("Executando");
    }
    public void Exite()
    {
        Debug.Log("Saiu");
    }
}
