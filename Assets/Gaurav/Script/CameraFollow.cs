using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Offset")]
    public Vector3 offset = new Vector3(3f, 2f, -10f); // X will auto flip

    [Header("Smooth")]
    public float smoothTime = 0.2f;

    private Vector3 velocity = Vector3.zero;
    private float originalOffsetX;

    void Start()
    {
        originalOffsetX = Mathf.Abs(offset.x); // store positive value
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Detect flip using localScale
        float facing = Mathf.Sign(target.localScale.x);

        // Flip camera offset based on player direction
        offset.x = originalOffsetX * facing;

        Vector3 targetPosition = target.position + offset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}
