using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Timing")]
    public float fadeDuration = 0.4f;
    public float respawnDelay = 0.2f;

    SpriteRenderer sr;
    PlayerMovement movement;
    PlayerCombat combat;
    Rigidbody2D rb;
    Animator animator; // ⭐ added

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // ⭐ added
    }

    void Start()
    {
        if (SpawnPoint.Active != null)
            transform.position = SpawnPoint.Active.position;
    }

    public void Respawn()
    {
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        movement.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        yield return StartCoroutine(Fade(1f, 0f));
        yield return new WaitForSeconds(respawnDelay);

        if (SpawnPoint.Active != null)
            transform.position = SpawnPoint.Active.position;

        combat.ResetPlayerState();

        // ⭐⭐⭐ RESET ANIMATION HERE ⭐⭐⭐
        animator.Rebind();        // full reset (best)
        animator.Update(0f);
        // OR animator.Play("Idle");

        yield return StartCoroutine(Fade(0f, 1f));

        rb.simulated = true;
        movement.enabled = true;
    }

    IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        Color c = sr.color;

        while (t < fadeDuration)
        {
            float a = Mathf.Lerp(from, to, t / fadeDuration);
            sr.color = new Color(c.r, c.g, c.b, a);
            t += Time.deltaTime;
            yield return null;
        }

        sr.color = new Color(c.r, c.g, c.b, to);
    }
}
