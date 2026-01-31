using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    public Animator animator;
    public Transform visual;

    PlayerMovement movement;
    Rigidbody2D rb;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float xVel = rb.linearVelocity.x;
        float yVel = rb.linearVelocity.y;

        bool isMoving = Mathf.Abs(xVel) > 0.01f;
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);

        // ---- SEND PARAMETERS ----
        animator.SetBool("IsGrounded", movement.IsGrounded);
        animator.SetBool("IsWallSliding", movement.IsWallSliding);
        animator.SetBool("IsWalk", isMoving && movement.IsGrounded && !isRunning);
        animator.SetBool("IsRun", isRunning && movement.IsGrounded);
        animator.SetFloat("YVelocity", yVel);

        // ---- FACE DIRECTION ----
        if (xVel > 0.01f)
            visual.localScale = new Vector3(1, 1, 1);
        else if (xVel < -0.01f)
            visual.localScale = new Vector3(-1, 1, 1);
    }
}
