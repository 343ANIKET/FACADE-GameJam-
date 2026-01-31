using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeDuration = 0.1f;
    public float shakeStrength = 0.08f;

    [Header("Zoom Settings")]
    public float physicalSize = 5f;
    public float spiritSize = 4.7f;
    public float zoomDuration = 0.15f;

    private Vector3 originalPosition;
    private float originalSize;

    private Camera cam;
    private Coroutine shakeRoutine;
    private Coroutine zoomRoutine;

    void Awake()
    {
        cam = GetComponent<Camera>();
        originalPosition = transform.localPosition;
        originalSize = cam.orthographicSize;
    }

    /* ===================== SHAKE ===================== */

    public void Shake()
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeRoutine());
    }

    IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            Vector2 offset = Random.insideUnitCircle * shakeStrength;
            transform.localPosition = originalPosition + (Vector3)offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        shakeRoutine = null;
    }

    /* ===================== ZOOM ===================== */

    public void ZoomToSpirit()
    {
        StartZoom(spiritSize);
    }

    public void ZoomToPhysical()
    {
        StartZoom(physicalSize);
    }

    void StartZoom(float targetSize)
    {
        if (zoomRoutine != null)
            StopCoroutine(zoomRoutine);

        zoomRoutine = StartCoroutine(ZoomRoutine(targetSize));
    }

    IEnumerator ZoomRoutine(float target)
    {
        float start = cam.orthographicSize;
        float t = 0f;

        while (t < zoomDuration)
        {
            cam.orthographicSize = Mathf.Lerp(start, target, t / zoomDuration);
            t += Time.deltaTime;
            yield return null;
        }

        cam.orthographicSize = target;
        zoomRoutine = null;
    }
}
