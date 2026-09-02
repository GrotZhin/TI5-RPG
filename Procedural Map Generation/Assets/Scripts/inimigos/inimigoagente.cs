using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class inimigoagente : MonoBehaviour
{
    Istateinimigos state;
    public agenteplayer player;
    public SkinnedMeshRenderer renderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

    public void ChangeState(Istateinimigos state)
    {
        this.state.Exite();
        this.state = state;
        state.Enter();
    }
}
