using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    UnityAction<float> Move;
    CharacterController cc;
    Animator animator;

    Vector3 moveInput = Vector3.zero;
    Vector3 moveDir;
    float ySpeed, rotationSpeed = 10;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        Move = MoveUnlocked;
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        moveInput.x = input.x;
        moveInput.z = input.y;
    }

    public void OnLockTarget(InputAction.CallbackContext context)
    {
        Move = context.performed? MoveLocked : MoveUnlocked;
    }

    void MoveUnlocked(float deltaTime)
    {
        moveDir = moveInput;
        float inputMagnitude = Mathf.Clamp01(moveDir.magnitude);

        animator.SetFloat("Input Magnitude", inputMagnitude, 0.05f, deltaTime);

        moveDir = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up) * moveInput;
        moveDir.Normalize();

        //ySpeed += Physics.gravity.y * deltaTime;

        if (moveDir != Vector3.zero)
        {
            animator.SetBool("IsMoving", true);

            Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed);
            
            float angle = Vector3.SignedAngle(transform.forward, moveDir, Vector3.up);
            angle = Mathf.Clamp(angle/180, -1f, 1f);


            animator.SetFloat("Turn Magnitude", angle);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    void MoveLocked(float deltaTime)
    {
        moveDir = moveInput;

        float inputMagnitude = Mathf.Clamp01(moveDir.magnitude);

        animator.SetFloat("Input Magnitude", inputMagnitude, 0.05f, deltaTime);

        moveDir = Quaternion.AngleAxis(cc.transform.rotation.eulerAngles.y, Vector3.up) * moveDir;
        moveDir.Normalize();

        //ySpeed += Physics.gravity.y * deltaTime;

        if (moveDir != Vector3.zero)
        {
            animator.SetBool("IsMoving", true);
            animator.SetBool("IsStrafing", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
            animator.SetBool("IsStrafing", false);

        }
    }

    void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);

        //Vector3 velocity = moveDir * 4 * Time.fixedDeltaTime;
        //velocity.y = ySpeed * Time.fixedDeltaTime;

        //cc.Move(velocity);
    }

    private void OnAnimatorMove()
    {
        Vector3 velocity = animator.deltaPosition;
        //velocity.y = ySpeed * Time.deltaTime;

        cc.Move(velocity);
        transform.rotation *= animator.deltaRotation;
    }
}
