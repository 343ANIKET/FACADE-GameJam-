using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 6f;
    public float runSpeed = 10f;

    [Header("Jump")]
    public float jumpForce = 9f;
    public float jumpHoldForce = 6f;
    public float maxJumpHoldTime = 0.15f;

    [Header("Wall Slide")]
    public float wallSlideSpeed = 2f;
    public float wallJumpForceX = 8f;
    public float wallJumpForceY = 9f;
    public Vector2 wallCheckSize = new Vector2(0.3f, 1.0f);
    public float wallDetachCooldown = 0.12f;

    [Header("Gravity")]
    public float fallMultiplier = 4f;
    public float lowJumpMultiplier = 3f;

    [Header("Checks")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    // ---- STATES FOR ANIMATION ----
    public bool IsGrounded { get; private set; }
    public bool IsWallSliding { get; private set; }

    Rigidbody2D rb;

    float moveInput;
    bool isJumpHolding;
    float jumpTimeCounter;

    float wallDetachTimer;
    int wallDir; // -1 left, +1 right
    bool justWallJumped;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // ---------- GROUND CHECK ----------
        IsGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // ---------- WALL CHECK ----------
        bool wallLeft = Physics2D.BoxCast(
            transform.position,
            wallCheckSize,
            0f,
            Vector2.left,
            0.1f,
            wallLayer
        );

        bool wallRight = Physics2D.BoxCast(
            transform.position,
            wallCheckSize,
            0f,
            Vector2.right,
            0.1f,
            wallLayer
        );

        bool touchingWall = wallLeft || wallRight;

        if (wallLeft) wallDir = -1;
        else if (wallRight) wallDir = 1;

        bool pressingTowardWall =
            (wallLeft && moveInput < 0) ||
            (wallRight && moveInput > 0);

        // ---------- WALL DETACH TIMER ----------
        if (wallDetachTimer > 0)
            wallDetachTimer -= Time.deltaTime;

        // ---------- WALL SLIDE ----------
        IsWallSliding =
            !IsGrounded &&
            touchingWall &&
            pressingTowardWall &&
            wallDetachTimer <= 0;

        if (IsWallSliding && rb.linearVelocity.y < -wallSlideSpeed)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                -wallSlideSpeed
            );
        }

        // ---------- WALL JUMP ----------
        if (IsWallSliding && Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocity = new Vector2(
                -wallDir * wallJumpForceX,
                wallJumpForceY
            );

            IsWallSliding = false;
            wallDetachTimer = wallDetachCooldown;
            justWallJumped = true;
            isJumpHolding = false;
        }

        // ---------- NORMAL JUMP ----------
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumpHolding = true;
            jumpTimeCounter = maxJumpHoldTime;
        }

        // ---------- JUMP HOLD ----------
        if (Input.GetKey(KeyCode.Space) && isJumpHolding)
        {
            if (jumpTimeCounter > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHoldForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumpHolding = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
            isJumpHolding = false;

        // ---------- BETTER GRAVITY ----------
        if (!IsWallSliding)
        {
            if (rb.linearVelocity.y < 0)
            {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y
                    * (fallMultiplier - 1f) * Time.deltaTime;
            }
            else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
            {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y
                    * (lowJumpMultiplier - 1f) * Time.deltaTime;
            }
        }
    }

    void FixedUpdate()
    {
        // Protect wall jump impulse for one physics frame
        if (justWallJumped)
        {
            justWallJumped = false;
            return;
        }

        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }
}
