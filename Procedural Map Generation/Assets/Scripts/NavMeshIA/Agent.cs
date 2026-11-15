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
  public EnemyStatusManager stats;
  public Animator animator;
  public EnemyAttackCheck attackCheck;
  public SkinnedMeshRenderer renderer;
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
  public void Update()
  {
    state?.Execute(Time.deltaTime);
  }



  private void OnAnimatorMove()
  {

    Vector3 rootPosition = animator.rootPosition;
    rootPosition.y = agent.nextPosition.y;
    agent.transform.position = rootPosition;
    agent.nextPosition = rootPosition;
  }


  public void ChangeState(IState state)
  {
    this.state?.Exit();
    this.state = state;
    state?.Enter();
  }

  public void Remove()
  {
    Destroy(this);
  }
  void OnDestroy()
  {
    if (isLeader)

      group.leader = group.list[Random.Range(0, group.list.Count)];

    else

      group.list.Remove(this);

    if (IslandManager.islandManager != null)
      IslandManager.islandManager.enemies.Remove(agent.gameObject);
  }
}
