using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    public Animator animator;
    public Transform visual;

    [Header("Audio")]
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
    }

    // =========================================================
    // FOOTSTEP FUNCTION (Call this from Animation Events)
    // =========================================================
    public void PlayFootstep()
    {
        // Only play if we are on the ground and not dead
        if (movement.IsGrounded && !combat.IsDead && footstepSounds.Length > 0)
        {
            if (audioSource != null)
            {
                // Randomize pitch for "organic" feel
                audioSource.pitch = Random.Range(1f - pitchVariation, 1f + pitchVariation);

                int index = Random.Range(0, footstepSounds.Length);
                audioSource.PlayOneShot(footstepSounds[index], volume);

                audioSource.pitch = 1f; // Reset pitch
            }
        }
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

        bool isJumping = !movement.IsGrounded && yVel > 0.1f;
        animator.SetBool("IsJumping", isJumping);

        /* ===================== LOCOMOTION ===================== */
        bool isMoving = Mathf.Abs(xVel) > 0.01f;
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);

        animator.SetBool("IsWalk", isMoving && movement.IsGrounded && !isRunning);
        animator.SetBool("IsRun", isRunning && movement.IsGrounded);

        /* ===================== FACING ===================== */
        if (xVel > 0.01f)
            visual.localScale = new Vector3(1, 1, 1);
        else if (xVel < -0.01f)
            visual.localScale = new Vector3(-1, 1, 1);
    }
}