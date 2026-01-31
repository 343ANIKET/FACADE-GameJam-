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

    [Header("Gravity")]
    public float fallMultiplier = 4f;
    public float lowJumpMultiplier = 3f;

    [Header("Checks")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    // ---- STATES FOR ANIMATION ----
    public bool IsGrounded { get; private set; }

    Rigidbody2D rb;

    float moveInput;
    bool isJumpHolding;
    float jumpTimeCounter;

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

        // ---------- JUMP ----------
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

    void FixedUpdate()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }
}
