using UnityEngine;
using System.Collections;

public class BossFlyer : EnemyBase
{
    [Header("Boss Movement")]
    public float moveSpeed = 2f;
    public float idleDuration = 3f;
    public float moveDuration = 2f;

    [Header("Idle Jitter")]
    public float swayIntensity = 1.5f; // How far it drifts
    public float swaySpeed = 2f;      // How fast it wobbles

    private bool isMoving;
    private Vector2 idleAnchorPoint;
    public PlayerCombat playercombat;

    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(BossLoop());
    }

    private IEnumerator BossLoop()
    {
        while (currentHealth > 0)
        {
            // --- STATE: IDLE (Swaying in place) ---
            isMoving = false;
            idleAnchorPoint = transform.position; // Remember where we started idling
            if (anim != null) anim.SetBool("IsMoving", false);

            float idleTimer = 0;
            while (idleTimer < idleDuration)
            {
                ApplySway();
                idleTimer += Time.deltaTime;
                yield return null;
            }

            // --- STATE: MOVE (Chasing Player) ---
            isMoving = true;
            if (anim != null) anim.SetBool("IsMoving", true);

            float moveTimer = moveDuration;
            while (moveTimer > 0)
            {
                FlyTowardsPlayer();
                moveTimer -= Time.deltaTime;
                yield return null;
            }
        }
    }

    private void ApplySway()
    {
        // Uses Sine and Cosine to create a smooth loopy movement
        float x = Mathf.Sin(Time.time * swaySpeed) * swayIntensity;
        float y = Mathf.Cos(Time.time * swaySpeed * 0.5f) * swayIntensity;

        Vector2 targetPos = idleAnchorPoint + new Vector2(x, y);

        // Move slightly towards the sway position
        rb.MovePosition(Vector2.Lerp(rb.position, targetPos, Time.deltaTime * swaySpeed));
    }

    private void FlyTowardsPlayer()
    {
        if (playerCombat == null) return;

        Vector2 direction = (playerCombat.transform.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        // Flip sprite
        if (direction.x > 0) sprite.flipX = false;
        else if (direction.x < 0) sprite.flipX = true;
    }

    protected override void Die()
    {
        StopAllCoroutines();
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 1;
        base.Die();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"{gameObject.name} hit the player!");

            anim.SetTrigger("Attack");
            // Play attack sound on contact

            playercombat.TakeDamage(contactDamage, transform.position);

           
        }
    }

    



}