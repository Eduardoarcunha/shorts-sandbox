using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController2D : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    private bool wantAttack;
    private float moveInput;
    public float CurrentMoveInput => moveInput;
    public float CurrentHorizontalSpeed =>
        movePhysically ? rb.linearVelocity.x : moveInput * moveSpeed;

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
    [SerializeField] private float coyoteTime = 0.1f;      // seconds after leaving ground you can still jump
    [SerializeField] private float jumpBufferTime = 0.1f;  // seconds before landing we remember a jump press

    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool grounded;
    private bool isJumpHeld;

    private static readonly int YVelHash = Animator.StringToHash("YVelocity");
    private static readonly int GroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int SpeedHash = Animator.StringToHash("Speed"); // drives run anim

    bool IsGrounded() => Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        grounded = IsGrounded();

        if (grounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f)
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (Mathf.Abs(moveInput) > 0.01f)
        {
            var scale = anim.gameObject.transform.localScale;
            scale.x = Mathf.Sign(moveInput) * Mathf.Abs(scale.x);
            anim.gameObject.transform.localScale = scale;
        }

        if (wantAttack)
        {
            var st = anim.GetCurrentAnimatorStateInfo(0);
            bool inAttack = (st.IsTag("Attack") || st.IsName("Attack")) && st.normalizedTime < 1f;
            if (!inAttack && !anim.IsInTransition(0))
                anim.SetTrigger(AttackHash);

            wantAttack = false;
        }

        anim.SetBool(GroundedHash, grounded);
        anim.SetFloat(YVelHash, rb.linearVelocity.y);
        anim.SetFloat(SpeedHash, Mathf.Abs(moveInput));
    }

    void FixedUpdate()
    {
        if (movePhysically)
        {
            float targetXVel = moveInput * moveSpeed;
            rb.linearVelocity = new Vector2(targetXVel, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            coyoteTimeCounter = 0f;
            jumpBufferCounter = 0f;
        }

        ApplyGravityMultipliers();
    }

    private void ApplyGravityMultipliers()
    {
        // Apply fall multiplier for better feel
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !isJumpHeld)
        {
            // Apply low jump multiplier if jump button is released (variable jump height)
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
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
}