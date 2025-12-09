using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    private bool wantAttack;
    private float moveInput;

    public float CurrentMoveInput => moveInput;
    public float CurrentHorizontalSpeed => movePhysically ? rb.linearVelocity.x : moveInput * moveSpeed;
    public float CurrentInputSpeed => moveInput * moveSpeed;
    public float GroundSpeed => groundVelocity.x;

    [Header("Movement")]
    [SerializeField] private bool movePhysically = true;
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.12f;
    [SerializeField] private LayerMask groundMask;

    [Header("Physics Improvements")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Jump Assist")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    public float CoyoteTimeCounter => coyoteTimeCounter;
    public float JumpBufferCounter => jumpBufferCounter;
    public float MaxCoyoteTime => coyoteTime;
    public float MaxJumpBufferTime => jumpBufferTime;

    private bool grounded;
    private bool wasGrounded;
    private bool isJumpHeld;
    private bool hasJumpedSinceGrounded;
    private Vector2 groundVelocity;

    private static readonly int YVelHash = Animator.StringToHash("YVelocity");
    private static readonly int GroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    bool IsGrounded() => Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        UpdateGroundedState();
        UpdateJumpTimers();
        HandleFacingDirection();
        HandleAttack();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
        ApplyGravityMultipliers();
    }

    void UpdateGroundedState()
    {
        wasGrounded = grounded;
        grounded = IsGrounded();
    }

    void UpdateJumpTimers()
    {
        if (grounded)
        {
            if (hasJumpedSinceGrounded)
                coyoteTimeCounter = 0;
            else
                coyoteTimeCounter = coyoteTime;

            if (!wasGrounded)
                hasJumpedSinceGrounded = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        coyoteTimeCounter = Mathf.Max(coyoteTimeCounter, 0f);

        if (jumpBufferCounter > 0f)
        {
            jumpBufferCounter -= Time.deltaTime;
            jumpBufferCounter = Mathf.Max(jumpBufferCounter, 0f);
        }
    }

    void HandleFacingDirection()
    {
        if (anim == null) return;
        if (Mathf.Abs(moveInput) <= 0.01f) return;

        var scale = anim.transform.localScale;
        scale.x = Mathf.Sign(moveInput) * Mathf.Abs(scale.x);
        anim.transform.localScale = scale;
    }

    void HandleAttack()
    {
        if (!wantAttack || anim == null) return;

        var st = anim.GetCurrentAnimatorStateInfo(0);
        bool inAttack = (st.IsTag("Attack") || st.IsName("Attack")) && st.normalizedTime < 1f;

        if (!inAttack && !anim.IsInTransition(0))
            anim.SetTrigger(AttackHash);

        wantAttack = false;
    }

    void UpdateAnimator()
    {
        if (anim == null) return;

        anim.SetBool(GroundedHash, grounded);
        anim.SetFloat(YVelHash, rb.linearVelocity.y);
        anim.SetFloat(SpeedHash, Mathf.Abs(moveInput));
    }

    void HandleMovement()
    {
        var vel = rb.linearVelocity;
        float baseX = movePhysically ? moveInput * moveSpeed : 0f;
        vel.x = baseX + groundVelocity.x;
        rb.linearVelocity = vel;
    }

    void HandleJump()
    {
        if (coyoteTimeCounter > 0f &&
            jumpBufferCounter > 0f &&
            !hasJumpedSinceGrounded)
        {
            PerformJump();
        }
    }

    void PerformJump()
    {
        var vel = rb.linearVelocity;
        vel.y = 0f;
        rb.linearVelocity = vel;

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        coyoteTimeCounter = 0f;
        jumpBufferCounter = 0f;
        hasJumpedSinceGrounded = true;
    }

    void ApplyGravityMultipliers()
    {
        var vel = rb.linearVelocity;

        if (vel.y < 0f)
        {
            vel += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (vel.y > 0f && !isJumpHeld)
        {
            vel += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }

        rb.linearVelocity = vel;
    }

    public void Jump()
    {
        jumpBufferCounter = jumpBufferTime;
        isJumpHeld = true;
    }

    public void OnJumpUp()
    {
        isJumpHeld = false;
    }

    public void Attack() => wantAttack = true;

    public void SetMoveInput(float input)
    {
        moveInput = Mathf.Clamp(input, -1f, 1f);
    }

    public void SetMovePhysically(bool value) => movePhysically = value;

    public void SetGroundVelocity(Vector2 velocity)
    {
        groundVelocity = velocity;
    }
}