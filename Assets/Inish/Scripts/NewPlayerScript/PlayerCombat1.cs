using UnityEngine;
using System.Collections;

public class PlayerCombat1 : MonoBehaviour
{
    public PlayerConfig config;

    public float currentHealth;
    public float currentShield;

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
        currentHealth = config.maxHealth;
        currentShield = config.maxShield;
    }

    void Update()
    {
        if (IsDead) return;

        HandleShieldInput();
        HandleShieldDrain();
        HandleShieldRegen();
    }

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

        currentShield -= config.shieldDrainPerSecond * Time.deltaTime;

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

        currentShield = Mathf.Min(
            currentShield + config.shieldRegenPerSecond * Time.deltaTime,
            config.maxShield
        );
    }

    void BreakShield()
    {
        currentShield = 0f;
        shieldActive = false;
        shieldCooldownTimer = config.shieldCooldown;

        ShieldJustBroke = true;
        StartCoroutine(ResetShieldBreakFlag());
    }

    IEnumerator ResetShieldBreakFlag()
    {
        yield return null;
        ShieldJustBroke = false;
    }

    public void TakeDamage(float damage, Vector2 source)
    {
        if (invincible || IsDead) return;

        float remainingDamage = damage;

        if (shieldActive && currentShield > 0f)
        {
            float shieldDamage = damage * config.shieldDamageMultiplier;
            currentShield -= shieldDamage;

            if (currentShield <= 0f)
            {
                remainingDamage = Mathf.Abs(currentShield) / config.shieldDamageMultiplier;
                BreakShield();
            }
            else remainingDamage = 0f;
        }

        if (remainingDamage > 0f)
        {
            currentHealth -= remainingDamage;

            if (currentHealth <= 0f)
            {
                Die();
                return;
            }

            IsTakingDamage = true;
            StartCoroutine(DamageRoutine());
            StartCoroutine(InvincibilityRoutine());
        }
    }

    void Die()
    {
        IsDead = true;
        rb.simulated = false;
    }

    IEnumerator DamageRoutine()
    {
        yield return new WaitForSeconds(0.25f);
        IsTakingDamage = false;
    }

    IEnumerator InvincibilityRoutine()
    {
        invincible = true;
        yield return new WaitForSeconds(config.invincibilityDuration);
        invincible = false;
    }

    public void ResetPlayerState()
    {
        currentHealth = config.maxHealth;
        currentShield = config.maxShield;
        IsDead = false;
        IsTakingDamage = false;
        invincible = false;
    }
}
