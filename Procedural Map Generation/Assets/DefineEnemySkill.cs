using Unity.VisualScripting;
using UnityEngine;

public class DefineEnemySkill : StateMachineBehaviour
{
    EnemyAgent enemy;
    public SkillData skill;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if(enemy == null) enemy = animator.GetComponent<EnemyAgent>();
        enemy.attackCheck.skill = skill;
        enemy.attackCheck.alreadyHit.Clear();              
    }

   
}
