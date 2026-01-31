using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Offset")]
    public Vector3 offset = new Vector3(3f, 2f, -10f);

    [Header("Smooth")]
    public float smoothTime = 0.2f;

    [Header("Bounds (Transforms)")]
    public Transform leftBound;
    public Transform rightBound;
    public Transform topBound;
    public Transform bottomBound;

    [Header("References")]
    public Camera cam;
    public CameraShake cameraShake;

    Vector3 velocity;
    float originalOffsetX;

    void Start()
    {
        originalOffsetX = Mathf.Abs(offset.x);
    }

    void LateUpdate()
    {
        if (target == null || cam == null)
            return;

        /* ===================== FLIP OFFSET ===================== */

        float facing = Mathf.Sign(target.localScale.x);
        offset.x = originalOffsetX * facing;

        /* ===================== FOLLOW ===================== */

        Vector3 desiredPosition = target.position + offset;

        Vector3 basePosition = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            smoothTime
        );

        /* ===================== CAMERA SIZE ===================== */

        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        /* ===================== APPLY BOUNDS ===================== */

        if (leftBound != null && rightBound != null)
        {
            basePosition.x = Mathf.Clamp(
                basePosition.x,
                leftBound.position.x + camWidth,
                rightBound.position.x - camWidth
            );
        }

        if (bottomBound != null && topBound != null)
        {
            basePosition.y = Mathf.Clamp(
                basePosition.y,
                bottomBound.position.y + camHeight,
                topBound.position.y - camHeight
            );
        }

        /* ===================== SHAKE ===================== */

        Vector3 shake = cameraShake != null
            ? cameraShake.GetShakeOffset()
            : Vector3.zero;

        transform.position = basePosition + shake;
    }
}
