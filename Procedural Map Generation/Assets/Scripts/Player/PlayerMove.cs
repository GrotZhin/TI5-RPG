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

    [Header("Lock-On")]
    [SerializeField] private LockOnSystem lockOnSystem;
    [Tooltip("Velocidade de giro pra encarar o alvo travado")]
    [SerializeField] private float faceTargetRotationSpeed = 18f;
    private Transform lockedTarget;

    // [SAVE-INTEGRATION] custos de estamina (PlayerStats)
    [Header("Estamina (PlayerStats)")]
    [SerializeField] private float jumpStaminaCost = 15f;
    [SerializeField] private float dashStaminaCost = 20f;
    [SerializeField] private float sprintStaminaCostPerSecond = 15f;
    private bool sprintHeld;
    //------------------------------------------------------------

    public Vector3 targetVelocity;
    public float ySpeed, rotationSpeed = 14, airSpeed = 3.75f;
    public bool jumpAir = true;
    public bool dashAir = true;

    public float jumpHeight = 4.4f, jumpTime = 0.4f;
    float velocityMultiplier = 1;
    float gravityMultiplier = 1;
    float rotationMultiplier = 1;
    float dashCooldown = 0;

    void Awake()
    {
        input.Initialize();
        if(AssignCam.atribute != null)
        AssignCam.atribute.trackingTarget = transform;
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

        if (lockOnSystem != null)
        {
            lockOnSystem.OnTargetLocked += HandleLockTargetChanged;
            lockOnSystem.OnTargetChanged += HandleLockTargetChanged;
            lockOnSystem.OnTargetUnlocked += HandleLockTargetCleared;
        }

        CalculateJump(jumpHeight, jumpTime);
    }

    void OnDestroy()
    {
        if (lockOnSystem != null)
        {
            lockOnSystem.OnTargetLocked -= HandleLockTargetChanged;
            lockOnSystem.OnTargetChanged -= HandleLockTargetChanged;
            lockOnSystem.OnTargetUnlocked -= HandleLockTargetCleared;
        }
    }
    private void HandleLockTargetChanged(Transform target)
    {
        lockedTarget = target;
    }
    private void HandleLockTargetCleared()
    {
        lockedTarget = null;
    }

    // [SAVE-INTEGRATION] devolve false se não houver estamina (ou se não existir PlayerStats, libera).
    bool TrySpend(float amount)
    {
        return PlayerStats.Instance == null || PlayerStats.Instance.TrySpendStamina(amount);
    }

    // [SAVE-INTEGRATION] corrida gasta estamina continuamente; sem estamina, volta a andar.
    void Update()
    {
        if (animator == null) return;

        var stats = PlayerStats.Instance;
        bool running = sprintHeld;

        bool onRunningState = animator.GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("Run");
        if (onRunningState && moveInput.sqrMagnitude > 0.01f) 
        {
            if( stats != null)
                running = !stats.IsExhausted && stats.TrySpendStamina(sprintStaminaCostPerSecond * Time.deltaTime);
            else running = true;
        }

        animator.SetBool("IsRunning", running);
    }
    //-----------------------------------------------------------------------
    public void OnMoveInput(Vector2 v2)
    {
        moveInput.x = v2.x;
        moveInput.z = v2.y;
    }

    // [SAVE-INTEGRATION] só guarda o estado do botão; o Update() decide se corre.
    public void OnSprint(bool context)
    {
        sprintHeld = context;
    }
    //---------------------------------------------------------------------------
    public void OnJump()
    {
        if (cc.isGrounded)
        {
            if (!TrySpend(jumpStaminaCost)) return;
            animator.SetTrigger("Jump");
        }
        else if (jumpAir)
        {
            if (!TrySpend(jumpStaminaCost)) return;
            animator.SetTrigger("Jump");
            jumpAir = false;
        }
    }

    public void OnDash()
    {
        if (cc.isGrounded && dashCooldown <= 0)
        {
            if (!TrySpend(dashStaminaCost)) return;
            animator.SetTrigger("Dash");
        }
        else if (dashAir)
        {
            if (!TrySpend(dashStaminaCost)) return;
            animator.SetTrigger("Dash");
            dashAir = false;
        }
    }

    public void OnLockTarget(InputAction.CallbackContext context)
    {
        Move = context.performed ? MoveLocked : MoveUnlocked;
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

        animator.SetFloat("Input Magnitude", inputMagnitude, 0.03f, deltaTime);

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
            else if (ySpeed < 1.3f && ySpeed > -0.08f)
            {
                ySpeed += Physics.gravity.y/2f * deltaTime * gravityMultiplier;

            }
            else
                ySpeed += Physics.gravity.y * 2.1f * deltaTime * gravityMultiplier;
        }
        else if (ySpeed < -2)
        {
            ySpeed = -2;
        }
    }

    void FixedUpdate()
    {
        if (dashCooldown > 0)
            dashCooldown -= Time.fixedDeltaTime;

        Move(Time.fixedDeltaTime);
        Gravity(Time.fixedDeltaTime);

        animator.SetBool("IsGrounded", cc.isGrounded);
        animator.SetFloat("Fall Magnitude", ySpeed);
    }

    private void OnAnimatorMove()
    {
        if (cc.isGrounded)
            targetVelocity = animator.deltaPosition * velocityMultiplier;
        else
        {
            var horizontal = animator.deltaPosition;
            horizontal.y = 0;
            if (horizontal.sqrMagnitude > 0.001)
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

    public void CalculateJump(float jumpHeight, float jumpTime)
    {
        // s = so + vt - at²/2  ->  jumpHeight = initialVelocity * jumpTime - gravity * jumpTime²/2
        gravity = (2 * jumpHeight) / (jumpTime * jumpTime);
        InitialVelocity = gravity * jumpTime;
    }
    public void Jump(bool airJump = false)
    {
        if (!airJump)
            CalculateJump(jumpHeight, jumpTime);
        else
            CalculateJump(jumpHeight * 4 / 5, jumpTime);

        ySpeed = InitialVelocity;
    }
    public void Dash()
    {
        dashCooldown = 0.35f;
        velocityMultiplier = 1.85f;
        gravityMultiplier = 0;
        ySpeed = 0;
        rotationMultiplier = 0;
        if (moveDir.magnitude > 0)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDir.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 180);
        }

    }
    public void ResetDash()
    {
        velocityMultiplier = 1;
        gravityMultiplier = 1;
        rotationMultiplier = 1;
    }
}