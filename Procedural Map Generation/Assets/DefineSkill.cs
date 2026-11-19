using Unity.VisualScripting;
using UnityEngine;

public class DefineSkill : StateMachineBehaviour
{
    Combat player;
    public SkillData skill;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if(player == null) player = animator.GetComponent<Combat>();
        player.skill = skill;
        player.alreadyHit.Clear();              
    }

   
}
