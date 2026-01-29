using UnityEngine;
using System.Collections;   
public abstract class EnemyBase : MonoBehaviour
{
    [Header("Base Stats")]
    public float maxHealth = 100;
    protected float currentHealth;
    public float contactDamage = 10;
    public float knockbackForce = 5f;

    protected Rigidbody2D rb;
    protected SpriteRenderer sprite;

    // 'virtual' allows sub-classes to add their own logic to Start
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}");

        // Flash red effect (Quick Jam Polish)
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // Add particle effects or sound here later
        Destroy(gameObject);
    }

    private IEnumerator FlashRed()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
    }

    // Handle touching the player
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Assuming your player script has a TakeDamage method
            // collision.gameObject.GetComponent<PlayerController>().TakeDamage(contactDamage);
            Debug.Log("Dealt damage to player!");
        }
    }
}