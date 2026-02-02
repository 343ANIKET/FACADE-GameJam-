using UnityEngine;
using System.Collections;

public class PlayerShieldVisual1 : MonoBehaviour
{
    [Header("Config")]
    public PlayerConfig config;

    [Header("Blink Settings")]
    public float breakBlinkDuration = 0.4f;
    public float blinkInterval = 0.08f;

    PlayerCombat1 combat;
    SpriteRenderer sr;
    Coroutine blinkRoutine;

    void Awake()
    {
        combat = GetComponentInParent<PlayerCombat1>();
        sr = GetComponent<SpriteRenderer>();

        // Keep GameObject active, hide sprite only
        if (sr != null)
            sr.enabled = false;
    }

    void OnDisable()
    {
        // Safety reset (respawn / scene reload)
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        blinkRoutine = null;

        if (sr != null)
            sr.enabled = false;
    }

    void Update()
    {
        if (combat == null || sr == null)
            return;

        /* ===================== NORMAL SHIELD ===================== */
        if (combat.IsShieldActive)
        {
            if (!sr.enabled && blinkRoutine == null)
                sr.enabled = true;
        }
        else
        {
            if (sr.enabled && blinkRoutine == null)
                sr.enabled = false;
        }

        /* ===================== SHIELD BREAK FEEDBACK ===================== */
        if (combat.ShieldJustBroke && blinkRoutine == null)
        {
            blinkRoutine = StartCoroutine(BreakBlinkRoutine());
        }
    }

    IEnumerator BreakBlinkRoutine()
    {
        float elapsed = 0f;
        sr.enabled = true;

        while (elapsed < breakBlinkDuration)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        sr.enabled = false;
        blinkRoutine = null;
    }
}
