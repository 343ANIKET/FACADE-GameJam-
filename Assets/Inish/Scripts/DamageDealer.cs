using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public float damage = 20f;
    public LayerMask playerLayer;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) == 0)
            return;

        PlayerCombat player = other.GetComponent<PlayerCombat>();
        if (player != null)
            player.TakeDamage(damage, transform.position);
    }
}
