using UnityEngine;
using System.Collections;

public class DamageFlashPlayer1 : MonoBehaviour
{
    [Header("Flash Settings")]
    public Color flashColor = Color.white;
    public float flashInterval = 0.08f;

    SpriteRenderer sr;
    Color originalColor;
    Coroutine flashRoutine;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    /// <summary>
    /// Call this when the player takes damage
    /// </summary>
    public void StartFlash(float duration)
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine(duration));
    }

    IEnumerator FlashRoutine(float duration)
    {
        float timer = 0f;
        bool toggle = false;

        while (timer < duration)
        {
            toggle = !toggle;
            sr.color = toggle ? flashColor : originalColor;

            yield return new WaitForSeconds(flashInterval);
            timer += flashInterval;
        }

        sr.color = originalColor;
        flashRoutine = null;
    }

    /// <summary>
    /// Force reset (used on respawn / death)
    /// </summary>
    public void ResetFlash()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        sr.color = originalColor;
        flashRoutine = null;
    }
}
