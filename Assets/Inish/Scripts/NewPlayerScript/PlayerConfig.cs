using UnityEngine;

[CreateAssetMenu(menuName = "Player/Player Config")]
public class PlayerConfig : ScriptableObject
{
    /* ===================== MOVEMENT ===================== */
    [Header("Movement")]
    public float walkSpeed = 6f;
    public float runSpeed = 10f;

    /* ===================== JUMP ===================== */
    [Header("Jump")]
    public float jumpForce = 9f;
    public float jumpHoldForce = 6f;
    public float maxJumpHoldTime = 0.15f;

    /* ===================== GRAVITY ===================== */
    [Header("Gravity")]
    public float fallMultiplier = 4f;
    public float lowJumpMultiplier = 3f;

    /* ===================== CHECKS ===================== */
    [Header("Ground Check")]
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    /* ===================== DASH (future-proof) ===================== */
    [Header("Dash (Optional)")]
    public float dashSpeed = 16f;
    public float dashDuration = 0.15f;

    /* ===================== COMBAT ===================== */
    [Header("Health")]
    public float maxHealth = 100f;

    [Header("Shield")]
    public float maxShield = 50f;
    public float shieldDrainPerSecond = 2f;
    public float shieldRegenPerSecond = 5f;
    public float shieldCooldown = 1.5f;
    public float shieldDamageMultiplier = 2f;

    [Header("Invincibility")]
    public float invincibilityDuration = 0.6f;

    /* ===================== RESPAWN ===================== */
    [Header("Respawn")]
    public float fadeDuration = 0.4f;
    public float respawnDelay = 0.2f;
}
