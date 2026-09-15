
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

[CreateAssetMenu(menuName = "ScriptableObject/inputReader")]
public class InputInfo : ScriptableObject, Inputs.IPlayerActions
{
    public static InputInfo inputInfo { get; set; }

    Inputs input;

    //events
    public static event Action<Vector2> OnMoveEvent;
    public static event Action<Vector2> OnLockEvent;
    public static event Action OnInteractEvent;
    public static event Action OnMapEvent;
    public static event Action OnCrouchEvent;
    public static event Action OnCrouchReleaseEvent;
    public static event Action OnJumpEvent;
    public static event Action<bool> OnSprintEvent;

    public static event Action OnAttackEvent;
    public static event Action OnAimEvent;

    public static event Action OnMenuEvent;








    public void Initialize()
    {

        ClearEvents();
        if (input == null)
        {
            input = new Inputs();
            input.Player.SetCallbacks(this);

        }
        SetGameplay();

    }
    private void ClearEvents()
    {
        OnMoveEvent = (v) => { };
        OnLockEvent = (v) => { };
        OnInteractEvent = () => { };
        OnMapEvent = () => { };
        OnCrouchEvent = () => { };
        OnJumpEvent = () => { };
        OnAttackEvent = () => { };
        OnAimEvent = () => { };
        OnMenuEvent = () => { };
        OnCrouchReleaseEvent = () => { };



    }
    public void ClearMechanicsEvent()
    {
        OnAttackEvent = () => { };
        OnAimEvent = () => { };

    }
    public void SetGameplay()
    {
        input?.Player.Enable();

        input?.UI.Disable();
    }
    public void SetUi()
    {
        input?.Player.Disable();
        input?.UI.Enable();
    }


    #region PlayerAction
    public void OnMove(InputAction.CallbackContext context)
    {

        OnMoveEvent(context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        OnLockEvent(context.ReadValue<Vector2>());
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
            OnInteractEvent();
    }
    public void OnMap(InputAction.CallbackContext context)
    {
        if (context.started)
            OnMapEvent();
    }
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started)
            OnCrouchEvent();
        else if (context.canceled)
            OnCrouchReleaseEvent();

    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnJumpEvent();
    }
    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.started)
            OnAimEvent();
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
            OnAttackEvent();
    }


    #endregion
    public void OnMenu(InputAction.CallbackContext context)
    {
        if (context.started)
            OnMenuEvent();
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnSprintEvent(true);
        else
            OnSprintEvent(false);



    }
}
