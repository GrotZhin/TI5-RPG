using System.Collections;

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
  [HideInInspector]
  public bool droppedLoot;
  bool waitingFlashEffect;

  IState state;
  void Awake()
  {
    agent = GetComponent<NavMeshAgent>();

    group = transform.parent.GetComponent<GroupFormation>();
  

    if (isLeader)
    {
      group.leader = this;
      GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.red;

    }
    else
      group.list.Add(this);
    
     state = new Idle(this);
    state?.Enter();

  }
  public void AttackPlayer()
  {
    var rd = Random.Range(0, group.list.Count);
    for (int i = 0; i < rd; i++)
    {
      var enemy = group.list[Random.Range(0, group.list.Count)];
      enemy.ChangeState(new GiveDamage(enemy));
    }
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


  public void FlashEffect(float duration, SkinnedMeshRenderer mat)
  {

    if (waitingFlashEffect) return;

    mat.material.EnableKeyword("_EMISSION");
    Color actualColor = mat.material.GetColor("_EmissionColor");
    mat.material.SetColor("_Emissive_Color", Color.white * 3f);

    StartCoroutine(ReturnColor(duration, mat, actualColor));
  }
  IEnumerator ReturnColor(float duration, SkinnedMeshRenderer mat, Color color)
  {
    waitingFlashEffect = true;
    yield return new WaitForSecondsRealtime(duration);
    mat.material.SetColor("_Emissive_Color", color);
    waitingFlashEffect = false;
  }
  public void ChangeState(IState state)
  {
    this.state?.Exit();
    this.state = state;
    state?.Enter();
  }

  public void Remove()
  {
    if (IslandManager.islandManager != null)
      IslandManager.islandManager.enemies.Remove(this);
    Destroy(gameObject);
  }
  void OnDestroy()
  {
    if (!isLeader)
    {
      group.list.Remove(this);
    }

    else
    {
      if (group.list.Count <= 0) return;
      group.leader = group.list[Random.Range(0, group.list.Count)];
      group.leader.isLeader = true;

    }



  }
}
