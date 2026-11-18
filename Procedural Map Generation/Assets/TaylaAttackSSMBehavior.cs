using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;

public class TaylaAttackSSMBehavior : StateMachineBehaviour
{
    int inputWindowTag = Animator.StringToHash("Input Window");
    int inputWindowAux = Animator.StringToHash("Input Window Aux");
    int inputLockTag = Animator.StringToHash("Input Lock");
    int inputX = Animator.StringToHash("XInput");
    int inputY = Animator.StringToHash("YInput");
    int inputCharged = Animator.StringToHash("ChargedInput");
    int inputCharging = Animator.StringToHash("Charging");
    int groundPound = Animator.StringToHash("Ground Pound");

    public static UnityAction AirSlow, AirFast, ResetAir;

    public static bool attacking = false;
    public static bool canAttack = true;
    public static bool canTurn = true;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.tagHash == inputWindowTag)
        {
            canTurn = true;
            canAttack = true;
        }

        else if (stateInfo.tagHash == inputWindowAux)
        {
            canTurn = false;
            canAttack = true;
        }

        else if (stateInfo.tagHash == inputLockTag)
        {
            canAttack = false;
            canTurn = true;
            animator.ResetTrigger(inputX);
            animator.ResetTrigger(inputY);
            animator.SetBool(inputCharging, false);
            animator.SetInteger(inputCharged, 0);
        }
        if (stateInfo.tagHash == groundPound)
        {
            canAttack = false;
            canTurn = true;
            animator.ResetTrigger(inputX);
            animator.ResetTrigger(inputY);
            animator.SetBool(inputCharging, false);
            animator.SetInteger(inputCharged, 0);
            AirFast();
        }
        else if (animator.GetBool("IsGrounded") == false)
        {
            AirSlow();
        }
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.tagHash == inputLockTag)
        {
            canAttack = true;
        }
        ResetAir();
    }

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        attacking = true;
    }

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        attacking = false;
        canTurn = true;
        canAttack = true;
    }
}
