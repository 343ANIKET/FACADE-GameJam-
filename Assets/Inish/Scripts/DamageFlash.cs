using UnityEngine;
using System.Collections;

public class DamageFlashPlayer : MonoBehaviour
{
    public Color damageColor = Color.red;
    public float blinkInterval = 0.1f;

    SpriteRenderer sr;
    Color originalColor;
    Coroutine flashRoutine;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    // Call this when damage starts
    public void StartFlash(float duration)
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine(duration));
    }

    IEnumerator FlashRoutine(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            sr.color = damageColor;
            yield return new WaitForSeconds(blinkInterval);
            sr.color = originalColor;
            yield return new WaitForSeconds(blinkInterval);

            elapsed += blinkInterval * 2f;
        }

        sr.color = originalColor;
        flashRoutine = null;
    }
}
