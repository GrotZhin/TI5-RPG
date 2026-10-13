using System;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    int inputXHash = Animator.StringToHash("XInput");
    int inputYHash = Animator.StringToHash("YInput");
    int inputCharging = Animator.StringToHash("Charging");
    int inputCharged = Animator.StringToHash("ChargedInput");
    [Serializable]
    public struct MinMax {
        [Range(0, 0.5f)]
        public float minTime, maxTime; 
    }

    public MinMax pauseBufferTimer;
    float tickCharge = 1f;
    float chargeTime = 0;
    bool charge = false;


    void Start()
    {
        animator = GetComponent<Animator>();
        InputInfo.OnAttackEvent += OnAttack;
        InputInfo.OnAttackHeavyEvent += OnAttackHeavy;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (charge)
        {
            chargeTime += Time.fixedDeltaTime;
        }

        else 
        {
            if (chargeTime > 0)
            {
                animator.SetBool(inputCharging, false);
                if (chargeTime < tickCharge)
                {
                    animator.SetTrigger(inputYHash);
                }
                else
                {
                    animator.SetInteger(inputCharged, (int)(chargeTime/tickCharge));
                }
                chargeTime = 0;
            }
        }

        if (chargeTime > 0.166f) //10 frames para começar a carregar (60 fps)
        {
            if (animator.GetBehaviour<TaylaAnimationBehavior>().canAttack)
            {
                animator.SetBool(inputCharging, charge);
                animator.SetInteger(inputCharged, (int)(chargeTime / tickCharge));
            }
            else
                animator.SetBool(inputCharging, false);
        }
    }

    void OnAttack()
    {
        if(animator.GetBehaviour<TaylaAnimationBehavior>().canAttack)
            animator.SetTrigger(inputXHash);
    }
    void OnAttackHeavy(bool holding)
    {
        if (animator.GetBehaviour<TaylaAnimationBehavior>().canAttack)
            charge = holding;

        else
            charge = false;
    }
}
