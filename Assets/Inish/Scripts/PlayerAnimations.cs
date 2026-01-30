using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Transform visual;

    [Header("Ground Check (same as PlayerMovement)")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    Rigidbody2D rb;
    bool wasGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // ---------------- PHYSICS DATA ----------------
        float xVel = rb.linearVelocity.x;
        float yVel = rb.linearVelocity.y;

        bool isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        bool isMoving = Mathf.Abs(xVel) > 0.01f;
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);

        // ---------------- FEED ANIMATOR ----------------
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsRun", isRunning);
        animator.SetBool("IsWalk", isMoving && !isRunning);
        animator.SetFloat("YVelocity", yVel);

        // ---------------- FACING ----------------
        if (xVel > 0.01f)
            visual.localScale = new Vector3(1, 1, 1);
        else if (xVel < -0.01f)
            visual.localScale = new Vector3(-1, 1, 1);

        // ---------------- LANDING EVENT (ONE-SHOT) ----------------
        // Animator handles Fall -> Land via IsGrounded == true
        // We ONLY track grounded change for correctness / debugging
        wasGrounded = isGrounded;
    }
}
