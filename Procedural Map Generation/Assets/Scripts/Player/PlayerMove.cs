using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    UnityAction<float> Move;
    CharacterController cc;
    Animator animator;

    [SerializeField]
    InputInfo input;
    Vector3 moveInput = Vector3.zero;
    Vector3 moveDir;
    float ySpeed, rotationSpeed = 14;

    void Awake()
    {
        input.Initialize();
    }
    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        Move = MoveUnlocked;
        InputInfo.OnMoveEvent += OnMoveInput;
        InputInfo.OnSprintEvent += OnSprint;
        InputInfo.OnJumpEvent += OnJump;
    }

    public void OnMoveInput(Vector2 v2)
    {
        moveInput.x = v2.x;
        moveInput.z = v2.y;
    }

    public void OnSprint(bool context)
    {
        animator.SetBool("IsRunning", context);
    }

    public void OnJump()
    {
        if (cc.isGrounded)
        {
            animator.SetTrigger("Jump");
            ySpeed = 4.7f;
        }
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

        ySpeed += Physics.gravity.y * deltaTime;

        if (moveDir != Vector3.zero)
        {
            animator.SetBool("IsMoving", true);

            Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            
            float angle = Vector3.SignedAngle(transform.forward, moveDir, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed);
            //if (Mathf.Abs(angle) < 45)
            //{
            //    transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed);
            //    animator.SetFloat("Turn Magnitude", 0);
            //}
            //else
            //{
            //    angle = angle/140;
            //    animator.SetFloat("Turn Magnitude", angle, 0.035f, deltaTime);
            //}
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

        ySpeed += Physics.gravity.y * deltaTime;

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
    }

    private void OnAnimatorMove()
    {
        Vector3 velocity = animator.deltaPosition;
        velocity.y = ySpeed * Time.deltaTime;

        cc.Move(velocity);
        transform.rotation *= animator.deltaRotation;
    }
}
