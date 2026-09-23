using System;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    [Serializable]
    public struct MinMax {
        [Range(0, 0.5f)]
        public float minTime, maxTime; 
    }

    public MinMax pauseBufferTimer;

    void Start()
    {
        animator = GetComponent<Animator>();
        InputInfo.OnAttackEvent += OnAttack;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    void OnAttack()
    {
        if(animator.GetBehaviour<TaylaAnimationBehavior>().canAttack)
            animator.SetTrigger("XInput");
    }
}
