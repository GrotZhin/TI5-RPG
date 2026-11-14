using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class Agent : MonoBehaviour
{
  [HideInInspector]
  public NavMeshAgent agent;
  public bool isLeader = false;
  [HideInInspector]
  public GroupFormation group;
  public UnityEvent done;
  public EnemyStats stats;
  public Animator animator;
  public EnemyAttackCheck attackCheck;
  IState state;
  void Start()
  {
    agent = GetComponent<NavMeshAgent>();
    group = transform.parent.GetComponent<GroupFormation>();
    state = new Idle(this);
    state?.Enter();
    
    if (isLeader)
    {
      group.leader = this;
      GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.red;

    }
    else
      group.list.Add(this);

   
  }

    private void OnAnimatorMove()
    {
        Vector3 rootPosition = animator.rootPosition;
        rootPosition.y = agent.nextPosition.y;
        transform.position = rootPosition;
        agent.nextPosition = rootPosition;
    }
    
   
  public void ChangeState(IState state)
  {
    this.state?.Exit();
    this.state = state;
    state?.Enter();
  }
 
  void OnDestroy()
  {
    if (isLeader)

      group.leader = group.list[Random.Range(0, group.list.Count)];

    else

      group.list.Remove(this);

  }
}
