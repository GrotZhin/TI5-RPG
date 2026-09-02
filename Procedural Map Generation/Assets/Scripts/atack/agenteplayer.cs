using UnityEngine;

public class agenteplayer : MonoBehaviour
{
    Istateinimigos stat;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stat = new Idleplayer(this);
        stat?.Enter();
    }

    // Update is called once per frame
    void Update()
    {
        stat?.Execute(Time.fixedDeltaTime);
        
    }

    public void ChangeState(Istateinimigos state)
    {
        this.stat.Exite();
        this.stat = state;
        state.Enter();
    }
}
