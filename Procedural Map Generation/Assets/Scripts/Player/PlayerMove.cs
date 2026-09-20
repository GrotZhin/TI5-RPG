using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

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
    public float ySpeed, rotationSpeed = 14, airSpeed = 3.75f;
    public bool airAction = true;
    public float jumpHeight = 4.4f, jumpTime = 0.4f;
    float velocityMultiplier = 1;
    float gravityMultiplier = 1;
    float rotationMultiplier = 1;
    float dashCooldown = 0;

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
        InputInfo.OnDashEvent += OnDash;

        CalculateJump(jumpHeight, jumpTime);
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
        else if (airAction)
        {
            animator.SetTrigger("Jump");
            airAction = false;
        }
    }

    public void OnDash()
    {
        if (cc.isGrounded && dashCooldown <= 0)
        {
            animator.SetTrigger("Dash");
        }
        else if (airAction)
        {
            animator.SetTrigger("Dash");
            airAction = false;
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

            if (animator.deltaRotation.eulerAngles.y < 1)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * rotationMultiplier);
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
            if (ySpeed >= 1.5f)
            {
                ySpeed += -gravity * deltaTime * gravityMultiplier;
            }
            else if (ySpeed < 1.5f && ySpeed > -0.2f)
            {
                ySpeed += Physics.gravity.y/1.85f * deltaTime * gravityMultiplier;

            }
            else
                ySpeed += Physics.gravity.y * deltaTime * gravityMultiplier;
        }
        else if (ySpeed < -2)
        {
            ySpeed = -2;
        }
    }


    void FixedUpdate()
    {
        if(dashCooldown > 0)
            dashCooldown -= Time.fixedDeltaTime;
        
        Move(Time.fixedDeltaTime);
        Gravity(Time.fixedDeltaTime);

        animator.SetBool("IsGrounded", cc.isGrounded);
    }

    private void OnAnimatorMove()
    {
        if(cc.isGrounded)
            targetVelocity = animator.deltaPosition * velocityMultiplier;
        else
        {
            var horizontal = animator.deltaPosition;
            horizontal.y = 0;
            if(horizontal.sqrMagnitude > 0.001)
                targetVelocity = animator.deltaPosition * velocityMultiplier;
            else
                targetVelocity = moveDir * Time.deltaTime * airSpeed * velocityMultiplier;
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

    float gravity;
    public float InitialVelocity { get; private set; }

    [ContextMenu("Testa Calculo")]
    public void CalculateJump(float jumpHeight, float jumpTime)
    {
        //v² = vo² + 2ad
        //0 = initialVelocity² + 2 * gravity * jumpHeight

        //float initialVelocity;
        //float distante = jumpHeight;
        //float acceleration = Physics.gravity.y;
        //initialVelocity = math.sqrt(-(2 * distante * acceleration));
        //Debug.Log(initialVelocity);

        //s = so + vt - at²/2
        //jumpHeight = 0 + initialVelocity * jumpTime - gravity * jumpTime²/2

        gravity = (2 * jumpHeight) / (jumpTime * jumpTime);
        InitialVelocity = gravity * jumpTime;
    }
    public void Jump(bool airJump = false)
    {
        if(!airJump)
            CalculateJump(jumpHeight, jumpTime);
        else
            CalculateJump(jumpHeight*4/5, jumpTime);

        ySpeed = InitialVelocity;
    }
    public void Dash()
    {
        dashCooldown = 1.35f;
        velocityMultiplier = 1.85f;
        gravityMultiplier = 0;
        rotationMultiplier = 0;
    }
    public void ResetDash()
    {
        velocityMultiplier = 1;
        gravityMultiplier = 1;
        rotationMultiplier = 1;
    }
}
