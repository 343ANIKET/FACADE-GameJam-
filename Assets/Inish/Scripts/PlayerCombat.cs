using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    /* ===================== HEALTH ===================== */

    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

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

#if UNITY_EDITOR
        // ===================== DEBUG DAMAGE (PRESS G) =====================
        if (Input.GetKeyDown(KeyCode.G))
        {
            Vector2 fakeSource = (Vector2)transform.position + Vector2.left;
            TakeDamage(20f, fakeSource);
        }
#endif
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
                remainingDamage = -currentShield;
                BreakShield();
            }
            else
            {
                remainingDamage = 0f;
            }
        }

        // ---------- HEALTH ----------
        if (remainingDamage > 0f)
        {
            currentHealth -= remainingDamage;
            currentHealth = Mathf.Max(currentHealth, 0f);

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

    // Animation Event
    public void OnDeathAnimationComplete()
    {
        // Destroy(gameObject);
        Debug.Log("Player has died. Game Over.");
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
}
