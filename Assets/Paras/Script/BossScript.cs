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

    [Header("Audio Settings")]
    public AudioClip spawnSound;
    public AudioClip attackSound;
    
    [Range(0f, 1f)] public float spawnVolume = 1f;
    [Range(0f, 1f)] public float attackVolume = 1f;

    public AudioSource spawnAudioSource; // For spawn sound
    public AudioSource attackAudioSource; // For damage sound


    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(BossLoop());
        SetupAudio();
        PlaySpawnSound();
    }

    private void SetupAudio()
    {
        // Create first audio source for spawn/death sounds
        spawnAudioSource = gameObject.AddComponent<AudioSource>();
        spawnAudioSource.playOnAwake = false;
        spawnAudioSource.loop = false;

        // Create second audio source for damage/attack sounds
        attackAudioSource = gameObject.AddComponent<AudioSource>();
        attackAudioSource.playOnAwake = false;
        attackAudioSource.loop = false;

        // Configure 3D sound settings (adjust based on your game)
        ConfigureAudioSource(spawnAudioSource);
        ConfigureAudioSource(attackAudioSource);
    }

    private void ConfigureAudioSource(AudioSource source)
    {
        source.spatialBlend = 0f; // 1 = full 3D, 0 = 2D
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.minDistance = 5f;
        source.maxDistance = 50f;
        source.dopplerLevel = 0f; // Reduce doppler effect for smoother sound
    }

    private void PlaySpawnSound()
    {
        if (spawnSound != null && spawnAudioSource != null)
        {
            spawnAudioSource.clip = spawnSound;
            spawnAudioSource.volume = spawnVolume;
            spawnAudioSource.Play();
            Debug.Log("Boss spawn sound played");
        }
    }


    private void PlayAttackSound()
    {
        if (attackSound != null && attackAudioSource != null)
        {
            attackAudioSource.PlayOneShot(attackSound, attackVolume);
            Debug.Log("Boss attack sound played");
        }
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
            PlayAttackSound();  
            // Play attack sound on contact

            playercombat.TakeDamage(contactDamage, transform.position);

           
        }
    }

    



}