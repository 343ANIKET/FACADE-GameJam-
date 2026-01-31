using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public PlayerCombat player;
    public Image healthFill;
    public Image shieldFill;

    void Update()
    {
        healthFill.fillAmount = player.currentHealth / player.maxHealth;
        shieldFill.fillAmount = player.currentShield / player.maxShield;

        shieldFill.enabled = player.currentShield > 0;
    }
}
