using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float duration = 0.1f;
    public float strength = 0.08f;
    

    private Vector3 originalPosition;
    private Coroutine shakeRoutine;

    void Awake()
    {
        originalPosition = transform.localPosition;
    }

    public void Shake()
    {
        // Prevent stacking shakes
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeRoutine());
    }

    IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector2 offset = Random.insideUnitCircle * strength;
            transform.localPosition = originalPosition + (Vector3)offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        shakeRoutine = null;
    }
}
