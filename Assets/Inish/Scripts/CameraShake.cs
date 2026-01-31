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

    Camera cam;
    Coroutine shakeRoutine;
    Coroutine zoomRoutine;

    Vector3 shakeOffset;

    void Awake()
    {
        cam = GetComponent<Camera>();
        shakeOffset = Vector3.zero;
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
            shakeOffset = new Vector3(offset.x, offset.y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
        shakeRoutine = null;
    }

    public Vector3 GetShakeOffset()
    {
        return shakeOffset;
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
