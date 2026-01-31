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
    protected Animator anim; 

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>(); // Initialize animator
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}");

        // Trigger 'Hurt' animation if it exists
        if (anim != null) anim.SetTrigger("Hurt");

        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (anim != null)
        {
            // Trigger the Death animation
            anim.SetTrigger("Die");

            // Disable AI and physics to prevent "zombie" attacks
            this.enabled = false;
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic; // Stop gravity/forces
            }

            GetComponent<Collider2D>().enabled = false;

            // Destroy after animation (tweak the 1f to match your clip length)
            Destroy(gameObject, 1f);
        }
        else
        {
            Destroy(gameObject);
        }
    } // Brace was missing here!

    private IEnumerator FlashRed()
    {
        if (sprite == null) yield break;
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Your Player Damage Logic here
            Debug.Log("Dealt damage to player!");
        }
    }
}