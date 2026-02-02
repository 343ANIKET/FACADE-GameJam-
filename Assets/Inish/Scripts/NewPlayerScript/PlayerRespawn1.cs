using UnityEngine;
using System.Collections;

public class PlayerRespawn1 : MonoBehaviour
{
    public PlayerConfig config;

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
        yield return new WaitForSeconds(config.respawnDelay);

        transform.position = SpawnPoint.Active.position;
        combat.ResetPlayerState();

        yield return StartCoroutine(Fade(0f, 1f));

        rb.simulated = true;
        movement.enabled = true;
    }

    IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        Color c = sr.color;

        while (t < config.fadeDuration)
        {
            sr.color = new Color(c.r, c.g, c.b, Mathf.Lerp(from, to, t / config.fadeDuration));
            t += Time.deltaTime;
            yield return null;
        }

        sr.color = new Color(c.r, c.g, c.b, to);
    }
}
