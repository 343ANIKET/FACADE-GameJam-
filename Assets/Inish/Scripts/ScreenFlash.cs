using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFlash : MonoBehaviour
{
    public Image flashImage;

    [Header("Flash Settings")]
    public float flashAlpha = 0.3f;
    public float flashDuration = 0.08f;

    Coroutine flashRoutine;

    void Awake()
    {
        flashImage.color = new Color(
            flashImage.color.r,
            flashImage.color.g,
            flashImage.color.b,
            0f
        );
    }

    public void Flash()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        // Flash ON
        flashImage.color = new Color(
            flashImage.color.r,
            flashImage.color.g,
            flashImage.color.b,
            flashAlpha
        );

        yield return new WaitForSeconds(flashDuration);

        // Flash OFF
        flashImage.color = new Color(
            flashImage.color.r,
            flashImage.color.g,
            flashImage.color.b,
            0f
        );

        flashRoutine = null;
    }
}
