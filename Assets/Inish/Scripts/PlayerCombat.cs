using UnityEngine;
using System.Collections;

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

    /* ===================== FEEDBACK ===================== */

    public CameraShake cameraShake;
    public DamageFlashPlayer damageFlash;

    /* ===================== STATES ===================== */

    public bool IsTakingDamage { get; private set; }
    public bool IsShieldActive => shieldActive;
    public bool ShieldJustBroke { get; private set; }
    public bool IsDead { get; private set; }

    Rigidbody2D rb;

    bool shieldActive;
    bool invincible;
    float shieldCooldownTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // IMPORTANT: make sure runtime values are correct
        currentHealth = maxHealth;
        currentShield = maxShield;
    }

    void Update()
    {
        if (IsDead) return;

        HandleShieldInput();
        HandleShieldDrain();
        HandleShieldRegen();

//#if UNITY_EDITOR
//        // ===================== DEBUG DAMAGE (PRESS G) =====================
//        if (Input.GetKeyDown(KeyCode.G))
//        {
//            Vector2 fakeSource = (Vector2)transform.position + Vector2.left;
//            TakeDamage(20f, fakeSource);
//        }
//#endif
    }

    /* ===================== SHIELD ===================== */

    void HandleShieldInput()
    {
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
        {
            BreakShield();
        }
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

    public void TakeDamage(float damage, Vector2 sourcePosition)
    {
        if (invincible || IsDead) return;

        float remainingDamage = damage;

        // ---------- SHIELD ----------
        if (shieldActive && currentShield > 0f)
        {
            float shieldDamage = damage * shieldDamageMultiplier;
            currentShield -= shieldDamage;

            if (currentShield <= 0f)
            {
                // Convert negative shield back to raw damage to apply to health
                remainingDamage = Mathf.Abs(currentShield) / shieldDamageMultiplier;
                BreakShield();
            }
            else
            {
                remainingDamage = 0f;
                // Optional: Play a "shield hit" sound here
            }
        }

        // ---------- HEALTH ----------
        if (remainingDamage > 0f)
        {
            currentHealth -= remainingDamage;
            currentHealth = Mathf.Max(currentHealth, 0f);

            // Play Random Hurt Sound with Pitch Shift
            PlayHurtSound();

            if (currentHealth <= 0f)
            {
                Die();
                return;
            }

            IsTakingDamage = true;
            damageFlash?.StartFlash(invincibilityDuration);
            cameraShake?.Shake();

            StartCoroutine(DamageStateRoutine());
            StartCoroutine(InvincibilityRoutine());
        }
    }

    /* ===================== DEATH ===================== */

   void Die()
    {
        IsDead = true;

        shieldActive = false;
        StopAllCoroutines();

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
    }



   public void ResetPlayerState()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;

        IsDead = false;
        IsTakingDamage = false;

        shieldActive = false;
        invincible = false;
    }



    // Animation Event
   public void OnDeathAnimationComplete()
    {
        PlayerRespawn respawn = GetComponent<PlayerRespawn>();
        if (respawn != null)
            respawn.Respawn();
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

    private void PlayHurtSound()
    {
        if (audioSource != null && hurtSounds.Length > 0)
        {
            audioSource.pitch = Random.Range(1f - pitchVariation, 1f + pitchVariation);
            audioSource.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Length)]);
            audioSource.pitch = 1f; // Reset pitch
        }
    }
}
