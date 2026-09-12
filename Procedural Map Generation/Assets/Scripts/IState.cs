using UnityEngine;

public interface IState
{
    public void Enter();
    public void Execute(float delta);
    public void Exit();
}
