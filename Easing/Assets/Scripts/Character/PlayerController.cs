using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController2D : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    private bool wantJump;
    private bool wantAttack;
    private float moveInput; // -1..1 from input system
    public float CurrentMoveInput => moveInput;   // -1..1 from input
    public float CurrentHorizontalSpeed =>
        movePhysically ? rb.linearVelocity.x : moveInput * moveSpeed;

    [Header("Movement")]
    [SerializeField] private bool movePhysically = true;   // If false, player stays in place (for endless runner BG trick)
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.12f;
    [SerializeField] private LayerMask groundMask;

    private static readonly int YVelHash = Animator.StringToHash("YVelocity");
    private static readonly int GroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int SpeedHash = Animator.StringToHash("Speed"); // drives run anim

    bool IsGrounded() => Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
        var grounded = IsGrounded();

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

        if (wantJump && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        wantJump = false;
    }

    public void Jump() => wantJump = true;
    public void Attack() => wantAttack = true;

    public void SetMoveInput(float input)
    {
        moveInput = Mathf.Clamp(input, -1f, 1f);
    }

    public void SetMovePhysically(bool value) => movePhysically = value;
}