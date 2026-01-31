using UnityEngine;
using System.Collections;

public class PlayerShieldVisual : MonoBehaviour
{
    public float breakBlinkDuration = 0.4f;
    public float blinkInterval = 0.08f;

    PlayerCombat combat;
    SpriteRenderer sr;
    Coroutine blinkRoutine;

    void Awake()
    {
        combat = GetComponentInParent<PlayerCombat>();
        sr = GetComponent<SpriteRenderer>();

        // IMPORTANT: keep GameObject active, hide sprite only
        sr.enabled = false;
    }

    void Update()
    {
        // ---------- NORMAL SHIELD VISIBILITY ----------
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

        // ---------- SHIELD BREAK FEEDBACK ----------
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
