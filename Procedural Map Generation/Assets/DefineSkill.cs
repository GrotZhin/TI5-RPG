using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DefineSkill : StateMachineBehaviour
{
    Combat player;
    public SkillData skill;
    public static UnityAction ShootArrow;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if(player == null) player = animator.GetComponent<Combat>();
        player.skill = skill;
        player.alreadyHit.Clear();              
                     
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (skill != null)
            if (skill.Arrow) 
                ShootArrow();
    }
   
}
