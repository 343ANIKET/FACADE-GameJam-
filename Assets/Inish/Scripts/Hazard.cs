using UnityEngine;

public class Hazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerCombat combat = other.GetComponent<PlayerCombat>();
        if (combat != null)
        {
            combat.Kill();
        }
    }
}
