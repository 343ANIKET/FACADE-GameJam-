using UnityEngine;
using UnityEngine.UI;

public class PlayerUI1 : MonoBehaviour
{
    [Header("References")]
    public PlayerCombat1 player;
    public PlayerConfig config;

    [Header("UI")]
    public Image healthFill;
    public Image shieldFill;

    void Update()
    {
        if (player == null || config == null)
            return;

        // ---------- HEALTH ----------
        healthFill.fillAmount = Mathf.Clamp01(
            player.currentHealth / config.maxHealth
        );

        // ---------- SHIELD ----------
        shieldFill.fillAmount = Mathf.Clamp01(
            player.currentShield / config.maxShield
        );

        shieldFill.enabled = player.currentShield > 0f;
    }
}
