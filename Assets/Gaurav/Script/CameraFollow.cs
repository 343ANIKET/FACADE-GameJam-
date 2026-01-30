using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;          // Player transform
    public Vector3 offset;            // Camera distance from player
    public float smoothTime = 0.2f;   // Delay amount (lower = faster, higher = slower)

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}
