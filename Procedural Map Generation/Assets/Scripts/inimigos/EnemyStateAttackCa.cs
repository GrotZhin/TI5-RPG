using UnityEngine;
using UnityEngine.UIElements;

public class EnemyStateAttackCa : IState
{
    EnemyAgentCa agent;
    float time;
    int chace;
    SkinnedMeshRenderer renderer;
    Animator animator;
    Vector3 dir;
    string ani1;
    public EnemyStateAttackCa(EnemyAgentCa agent, SkinnedMeshRenderer renderer)
    {
        this.agent = agent;
        this.renderer = renderer;
    }

    public void Enter()
    {
        //Debug.Log("Atack entrou");
        renderer.material.color = Color.red;
        agent.nav.updateRotation = false;
        if (agent.player != null)
        {
            //agent.player.dano(10);
            //Debug.Log("atack saiu  " + agent.player.vida);
        }
        dir = agent.player.transform.position - agent.transform.position;
        dir.y = 0;
        agent.transform.rotation = Quaternion.LookRotation(dir);
        animator = agent.GetComponent<Animator>();
        animator.SetBool("atacar", true);
        chace = Random.Range(0, 100);
        time = 1;
        agent.nav.SetDestination(agent.player.transform.position + Random.insideUnitSphere);
    }

    public void Execute(float delta)
    {
        //Debug.Log("atack executando");
        
        if (animator.GetBool("transformar"))
        {
            agent.nav.speed = 10;
            agent.nav.acceleration = 10;
            if (animator.GetBool("atacar"))
            {
                //agent.cc.Move(dir.normalized * 0.3f * Time.timeScale);
                
                
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !animator.IsInTransition(0))
                {
                    agent.ChangeState(new EnemyStateTransformarCa(agent, renderer));
                    ani1 = animator.GetCurrentAnimatorStateInfo(0).shortNameHash.ToString();
                }
            }
        }
        else
        {
            agent.nav.speed = 0;
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f && !animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).shortNameHash.ToString() != ani1)
            {
                if (chace < 50 || agent.player == null)
                {
                    agent.ChangeState(new EnemyStateMoveCa(agent, renderer));
                }
                else if (chace < 65)
                {
                    agent.ChangeState(new EnemyStateAttackCa(agent, renderer));
                }
                else
                {
                    agent.ChangeState(new EnemyStateFleeCa(agent, renderer));
                }
            }
        }
    }

    public void Exit()
    {
        //animator.SetBool("IsMoving", false);
        //animator.SetFloat("Input Magnitude", 0, 0, 0);
        animator.SetBool("atacar", false);
        agent.GetNeighbours().Clear();
        agent.nav.ResetPath();
        agent.nav.speed = 0.5f;
        agent.nav.acceleration = 8;
        agent.nav.updateRotation = true;
    }

}
