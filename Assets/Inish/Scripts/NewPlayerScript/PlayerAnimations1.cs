using UnityEngine;

public class PlayerAnimations1 : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Transform visual; // sprite root (for flip)

    [Header("Footstep Audio")]
    public AudioSource audioSource;
    public AudioClip[] footstepSounds;
    [Range(0.1f, 0.5f)] public float pitchVariation = 0.2f;
    [Range(0f, 1f)] public float volume = 0.4f;

    PlayerMovement movement;
    PlayerCombat combat;
    Rigidbody2D rb;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        float xVel = rb.linearVelocity.x;
        float yVel = rb.linearVelocity.y;

        /* ===================== DEATH OVERRIDE ===================== */
        animator.SetBool("IsDead", combat.IsDead);
        if (combat.IsDead) return;

        /* ===================== DAMAGE OVERRIDE ===================== */
        animator.SetBool("IsTakingDamage", combat.IsTakingDamage);
        if (combat.IsTakingDamage) return;

        /* ===================== CORE STATES ===================== */
        animator.SetBool("IsGrounded", movement.IsGrounded);
        animator.SetFloat("YVelocity", yVel);

        bool isJumpingUp = !movement.IsGrounded && yVel > 0.1f;
        bool isFalling = !movement.IsGrounded && yVel < -0.1f;

        animator.SetBool("IsJumping", isJumpingUp);
        animator.SetBool("IsFalling", isFalling);

        /* ===================== LOCOMOTION ===================== */
        bool isMoving = Mathf.Abs(xVel) > 0.05f;
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);

        animator.SetBool("IsWalk", isMoving && movement.IsGrounded && !isRunning);
        animator.SetBool("IsRun", isRunning && movement.IsGrounded);

        /* ===================== FACING ===================== */
        HandleFacing(xVel);
    }

    void HandleFacing(float xVel)
    {
        if (xVel > 0.05f)
            visual.localScale = new Vector3(1, 1, 1);
        else if (xVel < -0.05f)
            visual.localScale = new Vector3(-1, 1, 1);
    }

    /* =========================================================
     * FOOTSTEP (Called from Animation Event)
     * ========================================================= */
    public void PlayFootstep()
    {
        if (!movement.IsGrounded) return;
        if (combat.IsDead) return;
        if (footstepSounds.Length == 0) return;
        if (audioSource == null) return;

        audioSource.pitch = Random.Range(
            1f - pitchVariation,
            1f + pitchVariation
        );

        audioSource.PlayOneShot(
            footstepSounds[Random.Range(0, footstepSounds.Length)],
            volume
        );

        audioSource.pitch = 1f;
    }
}
