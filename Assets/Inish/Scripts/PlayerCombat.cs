using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerCombat : MonoBehaviour
{
    /* ===================== HEALTH ===================== */

    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] hurtSounds;
    [Range(0.1f, 0.5f)] public float pitchVariation = 0.2f;

    /* ===================== SHIELD ===================== */

    [Header("Shield")]
    public float maxShield = 50f;
    public float currentShield;
    public float shieldDrainPerSecond = 2f;
    public float shieldDamageMultiplier = 2f;
    public float shieldRegenPerSecond = 5f;
    public float shieldCooldown = 1.5f;

    /* ===================== INVINCIBILITY ===================== */

    public float invincibilityDuration = 0.6f;

    /* ===================== KNOCKBACK ===================== */

    [Header("Knockback")]
    public float knockbackForceX = 10f;
    public float knockbackDisableTime = 0.15f;

    /* ===================== FEEDBACK ===================== */

    public CameraShake cameraShake;
    public DamageFlashPlayer damageFlash;

    /* ===================== STATES ===================== */

    public bool IsTakingDamage { get; private set; }
    public bool IsShieldActive => shieldActive;
    public bool ShieldJustBroke { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsKnockedBack => isKnockedBack;

    Rigidbody2D rb;
    PlayerMovement movement;

    bool shieldActive;
    bool invincible;

    bool isKnockedBack;
    float knockbackTimer;
    float shieldCooldownTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();

        currentHealth = maxHealth;
        currentShield = maxShield;
    }

    void Update()
    {
        if (IsDead) return;

        // Knockback lock timer
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
                isKnockedBack = false;
        }

        HandleShieldInput();
        HandleShieldDrain();
        HandleShieldRegen();
    }

    /* ===================== SHIELD ===================== */

    void HandleShieldInput()
    {
        if (MaskController.Instance == null ||
            !MaskController.Instance.HasMask)
        {
            shieldActive = false;
            return;
        }

        if (Input.GetKey(KeyCode.E) &&
            shieldCooldownTimer <= 0f &&
            currentShield > 0f)
        {
            shieldActive = true;
        }
        else
        {
            shieldActive = false;
        }
    }

    void HandleShieldDrain()
    {
        if (!shieldActive) return;

        currentShield -= shieldDrainPerSecond * Time.deltaTime;

        if (currentShield <= 0f)
            BreakShield();
    }

    void HandleShieldRegen()
    {
        if (shieldActive) return;

        if (shieldCooldownTimer > 0f)
        {
            shieldCooldownTimer -= Time.deltaTime;
            return;
        }

        if (currentShield < maxShield)
        {
            currentShield += shieldRegenPerSecond * Time.deltaTime;
            currentShield = Mathf.Min(currentShield, maxShield);
        }
    }

    void BreakShield()
    {
        currentShield = 0f;
        shieldActive = false;
        shieldCooldownTimer = shieldCooldown;

        ShieldJustBroke = true;
        StartCoroutine(ResetShieldBreakFlag());
    }

    IEnumerator ResetShieldBreakFlag()
    {
        yield return null;
        ShieldJustBroke = false;
    }

    /* ===================== DAMAGE ===================== */

    public void Kill()
    {
        if (IsDead) return;

        currentHealth = 0f;
        Die();
    }

    public void TakeDamage(float damage, Vector2 sourcePosition)
    {
        if (invincible || IsDead) return;

        float remainingDamage = damage;
        bool shieldAbsorbedHit = false;

        /* ---------- SHIELD ---------- */
        if (shieldActive && currentShield > 0f)
        {
            float shieldDamage = damage * shieldDamageMultiplier;
            currentShield -= shieldDamage;

            shieldAbsorbedHit = true;

            if (currentShield <= 0f)
            {
                remainingDamage = Mathf.Abs(currentShield) / shieldDamageMultiplier;
                BreakShield();
            }
            else
            {
                remainingDamage = 0f;
            }
        }

        /* ---------- HEALTH ---------- */
        if (remainingDamage > 0f)
        {
            currentHealth -= remainingDamage;
            currentHealth = Mathf.Max(currentHealth, 0f);

            PlayHurtSound();

            if (currentHealth <= 0f)
            {
                Die();
                return;
            }

            // ✅ Apply knockback ONLY if shield did NOT absorb the hit
            if (!shieldAbsorbedHit)
                ApplyKnockback(sourcePosition);

            IsTakingDamage = true;
            damageFlash?.StartFlash(invincibilityDuration);
            cameraShake?.Shake();

            StartCoroutine(DamageStateRoutine());
            StartCoroutine(InvincibilityRoutine());
        }
    }

    /* ===================== KNOCKBACK ===================== */

    void ApplyKnockback(Vector2 sourcePosition)
    {
        // Cancel dash only when knockback happens
        if (movement != null)
            movement.SendMessage("EndDash", SendMessageOptions.DontRequireReceiver);

        float dir = Mathf.Sign(transform.position.x - sourcePosition.x);
        if (dir == 0f) dir = 1f;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(dir * knockbackForceX, 0f), ForceMode2D.Impulse);

        isKnockedBack = true;
        knockbackTimer = knockbackDisableTime;
    }

    /* ===================== DEATH ===================== */

    void Die()
    {
        if (IsDead) return;

        IsDead = true;
        shieldActive = false;
        StopAllCoroutines();

        rb.linearVelocity = Vector2.zero; // ⭐ FIX (not linearVelocity)
        rb.simulated = false;

        // ⭐⭐⭐ CALL RESPAWN HERE ⭐⭐⭐
        GetComponent<PlayerRespawn>()?.Respawn();

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "BossArena")
        {
            StartCoroutine(LoadLevelAfterDeath("SceneBetwee Boss Arena To Level1"));
        }
    }


    IEnumerator LoadLevelAfterDeath(string sceneName)
    {
        yield return new WaitForSeconds(1.2f);
        SceneManager.LoadScene(sceneName);
    }

    public void ResetPlayerState()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;

        IsDead = false;
        IsTakingDamage = false;

        shieldActive = false;
        invincible = false;
        isKnockedBack = false;
    }

    /* ===================== ROUTINES ===================== */

    IEnumerator DamageStateRoutine()
    {
        yield return new WaitForSeconds(0.25f);
        IsTakingDamage = false;
    }

    IEnumerator InvincibilityRoutine()
    {
        invincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        invincible = false;
    }

    void PlayHurtSound()
    {
        if (audioSource != null && hurtSounds.Length > 0)
        {
            audioSource.pitch = Random.Range(1f - pitchVariation, 1f + pitchVariation);
            audioSource.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Length)]);
            audioSource.pitch = 1f;
        }
    }
}
