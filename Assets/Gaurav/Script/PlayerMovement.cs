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

    [Header("Forgiveness")]
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;

    [Header("Apex Boost")]
    public float apexBoost = 2.5f;
    public float apexThreshold = 1f;

    [Header("Gravity")]
    public float fallMultiplier = 4f;
    public float lowJumpMultiplier = 3f;

    [Header("Fall Clamp")]
    public float maxFallSpeed = -18f;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.15f;
    public KeyCode dashKey = KeyCode.LeftControl;

    [Header("Dash Afterimage")]
    public DashAfterimage afterimagePrefab;
    public float afterimageSpawnInterval = 0.04f;
    public int maxAfterimages = 4;

    [Header("Checks")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public bool IsGrounded { get; private set; }

    Rigidbody2D rb;
    SpriteRenderer playerSprite;

    float moveInput;
    bool isJumpHolding;
    float jumpTimeCounter;

    float coyoteTimer;
    float jumpBufferTimer;

    bool isDashing;
    float dashTimer;
    float dashCooldownTimer;
    Vector2 dashDirection;
    int dashAvailable = 1;

    float afterimageTimer;
    int afterimageCount;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        dashCooldownTimer -= Time.deltaTime;

        /* ===================== DASH UPDATE ===================== */
        if (isDashing)
        {
            // Dash → Jump cancel
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CancelDashIntoJump();
                return;
            }

            dashTimer -= Time.deltaTime;
            rb.linearVelocity = dashDirection * dashSpeed;

            // Afterimages (exactly maxAfterimages)
            afterimageTimer -= Time.deltaTime;
            if (afterimageTimer <= 0f && afterimageCount < maxAfterimages)
            {
                SpawnAfterimage();
                afterimageTimer = afterimageSpawnInterval;
                afterimageCount++;
            }

            if (dashTimer <= 0f)
                EndDash();

            return; // IMPORTANT: skip normal movement
        }

        /* ===================== GROUND CHECK ===================== */
        IsGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (IsGrounded)
        {
            coyoteTimer = coyoteTime;
            dashAvailable = 1;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        /* ===================== JUMP BUFFER ===================== */
        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;

        /* ===================== DASH INPUT ===================== */
        if (Input.GetKeyDown(dashKey) && dashAvailable > 0 && dashCooldownTimer <= 0f)
        {
            StartDash();
            return;
        }

        /* ===================== JUMP EXECUTION ===================== */
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumpHolding = true;
            jumpTimeCounter = maxJumpHoldTime;

            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }

        /* ===================== VARIABLE JUMP ===================== */
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

        /* ===================== BETTER GRAVITY ===================== */
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y *
                (fallMultiplier - 1f) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y *
                (lowJumpMultiplier - 1f) * Time.deltaTime;
        }

        /* ===================== FALL CLAMP ===================== */
        if (rb.linearVelocity.y < maxFallSpeed)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxFallSpeed);

        /* ===================== APEX BOOST ===================== */
        if (!IsGrounded && Mathf.Abs(rb.linearVelocity.y) < apexThreshold)
        {
            rb.linearVelocity += new Vector2(
                moveInput * apexBoost * Time.deltaTime,
                0f
            );
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }

    /* ===================== DASH METHODS ===================== */

    void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        dashAvailable--;

        afterimageTimer = 0f;
        afterimageCount = 0;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (x == 0 && y == 0)
            x = transform.localScale.x;

        dashDirection = new Vector2(x, y).normalized;
        rb.linearVelocity = dashDirection * dashSpeed;
    }

    void EndDash()
    {
        isDashing = false;
    }

    void CancelDashIntoJump()
    {
        isDashing = false;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isJumpHolding = true;
        jumpTimeCounter = maxJumpHoldTime;
    }

    void SpawnAfterimage()
    {
        DashAfterimage img = Instantiate(
            afterimagePrefab,
            transform.position,
            Quaternion.identity
        );

        img.Init(
            playerSprite.sprite,
            playerSprite.flipX,
            transform.localScale,
            afterimageCount,
            maxAfterimages
        );
    }
}
