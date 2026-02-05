using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public PlayerCombat player;
    public Image healthFill;
    public Image shieldFill;

    void Update()
    {
        if (player == null) return;

        healthFill.fillAmount = player.currentHealth / (float)player.maxHealth;
        shieldFill.fillAmount = player.currentShield / (float)player.maxShield;

        shieldFill.enabled = player.currentShield > 0;
    }
}
