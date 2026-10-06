using UnityEngine;

public class EnemyStateAttack : IState
{
    EnemyAgent agent;
    float time;
    int chace;
    SkinnedMeshRenderer renderer;
    Animator animator;
    Vector3 dir;
    string ani1;
    public EnemyStateAttack(EnemyAgent agent, SkinnedMeshRenderer renderer)
    {
        this.agent = agent;
        this.renderer = renderer;
    }

    public void Enter()
    {
        //Debug.Log("Atack entrou");
        renderer.material.color = Color.red;
        if (agent.player != null)
        {
            agent.player.dano(10);
            Debug.Log("atack saiu  " + agent.player.vida);
        }
        dir = GameObject.FindGameObjectWithTag("Player").transform.position - agent.transform.position;
        dir.y = 0;
        Quaternion toRotation = Quaternion.LookRotation(dir, Vector3.up);
        animator = agent.GetComponent<Animator>();
        animator.SetBool("atacar", true);
        chace = Random.Range(0, 100);
        time = 1;
    }

    public void Execute(float delta)
    {
        //Debug.Log("atack executando");
        
        if (animator.GetBool("transformar"))
        {
            if (animator.GetBool("atacar"))
            {
                agent.cc.Move(dir.normalized * 0.3f * Time.timeScale);
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !animator.IsInTransition(0))
                {
                    agent.ChangeState(new EnemyStateTransformar(agent, renderer));
                    ani1 = animator.GetCurrentAnimatorStateInfo(0).shortNameHash.ToString();
                }
            }
        }
        else
        {
            
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f && !animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).shortNameHash.ToString() != ani1)
            {
                if (chace < 50 || agent.player == null)
                {
                    agent.ChangeState(new EnemyStateMove(agent, renderer));
                }
                else if (chace < 65)
                {
                    agent.ChangeState(new EnemyStateAttack(agent, renderer));
                }
                else
                {
                    agent.ChangeState(new EnemyStateFlee(agent, renderer));
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
    }

}
