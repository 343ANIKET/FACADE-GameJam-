using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement1 : MonoBehaviour
{
    public PlayerConfig config;
    public Transform groundCheck;

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

        IsGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            config.groundCheckRadius,
            config.groundLayer
        );

        Debug.DrawLine(
        groundCheck.position,
        groundCheck.position + Vector3.down * 0.2f,
        IsGrounded ? Color.green : Color.red
        );

        // ---------- JUMP ----------
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, config.jumpForce);
            isJumpHolding = true;
            jumpTimeCounter = config.maxJumpHoldTime;
        }

        // ---------- VARIABLE JUMP ----------
        if (Input.GetKey(KeyCode.Space) && isJumpHolding)
        {
            if (jumpTimeCounter > 0f)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    config.jumpHoldForce
                );
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
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y *
                (config.fallMultiplier - 1f) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y *
                (config.lowJumpMultiplier - 1f) * Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        float speed = Input.GetKey(KeyCode.LeftShift)
            ? config.runSpeed
            : config.walkSpeed;

        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }
}
