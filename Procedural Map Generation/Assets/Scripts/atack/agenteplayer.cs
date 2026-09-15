using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAgent : MonoBehaviour
{
    IState state;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
    public void bater(InputAction.CallbackContext callback)
    {
        ChangeState(new Ataque1player(this));
    }

}
