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

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Snap player to spawn point on scene start
        if (SpawnPoint.Active != null)
            transform.position = SpawnPoint.Active.position;
    }

    public void Respawn()
    {
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        // Disable control
        movement.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        // Fade out
        yield return StartCoroutine(Fade(1f, 0f));
        yield return new WaitForSeconds(respawnDelay);

        // Move player to spawn
        if (SpawnPoint.Active != null)
            transform.position = SpawnPoint.Active.position;

        // Reset stats
        combat.ResetPlayerState();

        // Fade in
        yield return StartCoroutine(Fade(0f, 1f));

        // Re-enable control
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
