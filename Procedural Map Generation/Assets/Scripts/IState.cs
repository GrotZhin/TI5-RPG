using UnityEngine;

public interface IState
{
    public void Enter();
    public virtual void Execute(float delta){}
    public void Exit();
}
