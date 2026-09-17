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

    public Vector3 targetVelocity;
    public float ySpeed, rotationSpeed = 14, airSpeed = 3.85f;

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

        if (moveDir != Vector3.zero)
        {
            animator.SetBool("IsMoving", true);

            Quaternion toRotation = Quaternion.LookRotation(moveDir.normalized, Vector3.up);
            
            float angle = Vector3.SignedAngle(transform.forward, moveDir.normalized, Vector3.up);

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

    void Gravity(float deltaTime)
    {
        if (!cc.isGrounded)
        {
            if (ySpeed < 1.5f && ySpeed > -0.2f)
            {
                ySpeed += Physics.gravity.y/1.85f * deltaTime;

            }
            else
                ySpeed += Physics.gravity.y * deltaTime;
        }
        else if (ySpeed < -2)
        {
            ySpeed = -2;
        }
    }


    void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);
        Gravity(Time.fixedDeltaTime);

        animator.SetBool("IsGrounded", cc.isGrounded);
    }

    private void OnAnimatorMove()
    {
        if(cc.isGrounded)
            targetVelocity = animator.deltaPosition;
        else
        {
 
            targetVelocity = moveDir * Time.deltaTime * airSpeed;
        }
        targetVelocity.y = ySpeed * Time.deltaTime;

        cc.Move(targetVelocity);
        transform.rotation *= animator.deltaRotation;
    }

    public CharacterController GetControler()
    {
        return cc;
    }

    public void SetTargetVelocity(Vector3 value)
    {
        targetVelocity = value;
    }
}
