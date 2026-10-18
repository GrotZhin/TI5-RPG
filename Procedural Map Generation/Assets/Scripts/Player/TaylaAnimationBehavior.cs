using UnityEngine;

public class TaylaAnimationBehavior : StateMachineBehaviour
{
    PlayerMove player;
    int inputWindowTag = Animator.StringToHash("Input Window");
    int inputLockTag = Animator.StringToHash("Input Lock");
    int inputX = Animator.StringToHash("XInput");
    int inputY = Animator.StringToHash("YInput");
    int inputCharged = Animator.StringToHash("ChargedInput");
    int inputCharging = Animator.StringToHash("Charging");

    public bool canAttack = true;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(player == null) 
            player = animator.GetComponent<PlayerMove>();
        if (stateInfo.IsName("Jump"))
            player.Jump();
        if (stateInfo.IsName("DoubleJump"))
            player.Jump(true);
        if (stateInfo.IsName("DashA") || stateInfo.IsName("DashB") || stateInfo.IsName("ArialDash"))
            player.Dash();

        if (stateInfo.IsName("Fall"))
            player.airAction = true;

        if (stateInfo.tagHash == inputWindowTag)
            canAttack = true;
        if (stateInfo.tagHash == inputLockTag)
        {
            canAttack = false;
            animator.ResetTrigger(inputX);
            animator.ResetTrigger(inputY);
            animator.SetBool(inputCharging, false);
            animator.SetInteger(inputCharged, 0);
        }


    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("DashA") || stateInfo.IsName("DashB") || stateInfo.IsName("ArialDash"))
            player.ResetDash();

        if(stateInfo.tagHash == inputLockTag)
        {
            canAttack = true;
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
