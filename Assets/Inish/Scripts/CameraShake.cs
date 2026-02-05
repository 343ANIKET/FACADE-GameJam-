using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Shake")]
    public float shakeDecay = 6f;
    public float defaultShakeStrength = 0.08f;

    [Header("Zoom")]
    public float physicalSize = 5f;
    public float spiritSize = 4.7f;
    public float zoomSmoothTime = 0.12f;

    Camera cam;

    Vector3 shakeOffset;
    Vector3 shakeVelocity;

    float targetZoom;
    float zoomVelocity;

    void Awake()
    {
        cam = GetComponent<Camera>();
        targetZoom = cam.orthographicSize;
    }

    void LateUpdate()
    {
        /* ===================== SHAKE DECAY ===================== */
        shakeOffset = Vector3.SmoothDamp(
            shakeOffset,
            Vector3.zero,
            ref shakeVelocity,
            1f / shakeDecay
        );

        /* ===================== ZOOM ===================== */
        cam.orthographicSize = Mathf.SmoothDamp(
            cam.orthographicSize,
            targetZoom,
            ref zoomVelocity,
            zoomSmoothTime
        );
    }

    /* ===================== SHAKE ===================== */

    public void Shake()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Shake(randomDir, defaultShakeStrength);
    }

    public void Shake(Vector2 direction, float strength)
    {
        if (direction == Vector2.zero)
            direction = Random.insideUnitCircle;

        shakeOffset = direction.normalized * strength;
    }

    public Vector3 GetShakeOffset()
    {
        return shakeOffset;
    }

    /* ===================== ZOOM ===================== */

    public void ZoomToSpirit()
    {
        targetZoom = spiritSize;
    }

    public void ZoomToPhysical()
    {
        targetZoom = physicalSize;
    }

    public void ZoomTo(float size)
    {
        targetZoom = size;
    }
}
