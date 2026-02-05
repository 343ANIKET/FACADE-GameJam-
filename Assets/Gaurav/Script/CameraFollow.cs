using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Offset")]
    public Vector3 offset = new Vector3(0f, 2f, -10f);


    [Header("Smooth Follow")]
    public float smoothTime = 0.2f;

    [Header("Look Up / Down")]
    public float lookOffsetY = 3f;
    public float lookSmoothTime = 0.15f;
    public KeyCode lookUpKey = KeyCode.I;
    public KeyCode lookDownKey = KeyCode.K;

    [Header("Look Ahead")]
    public float lookAheadDistance = 2f;
    public float lookAheadSmooth = 0.15f;

    [Header("Landing Impact")]
    public float landingDrop = 0.4f;
    public float landingSmooth = 0.12f;

    [Header("Bounds")]
    public Transform leftBound;
    public Transform rightBound;
    public Transform topBound;
    public Transform bottomBound;

    [Header("References")]
    public Camera cam;
    public CameraShake cameraShake;

    Vector3 velocity;
 

    float currentLookOffsetY;
    float lookVelocityY;

    float lookAheadX;
    float lookAheadVelocity;

    float landingOffset;
    float landingVelocity;

    bool wasGrounded;

    Rigidbody2D targetRb;
    PlayerMovement movement;

    void Start()
    {
        if (target == null) return;

        
        targetRb = target.GetComponent<Rigidbody2D>();
        movement = target.GetComponent<PlayerMovement>();
    }

    void LateUpdate()
    {
        if (target == null || cam == null) return;

        /* ===================== LOOK UP / DOWN ===================== */
        float targetLookY = 0f;

        if (Input.GetKey(lookUpKey))
            targetLookY = lookOffsetY;
        else if (Input.GetKey(lookDownKey))
            targetLookY = -lookOffsetY;

        currentLookOffsetY = Mathf.SmoothDamp(
            currentLookOffsetY,
            targetLookY,
            ref lookVelocityY,
            lookSmoothTime
        );

        /* ===================== LOOK AHEAD ===================== */
            float inputX = Input.GetAxisRaw("Horizontal");
            float desiredLookX = inputX * lookAheadDistance;

            lookAheadX = Mathf.SmoothDamp(
                lookAheadX,
                desiredLookX,
                ref lookAheadVelocity,
                lookAheadSmooth
            );


        /* ===================== LANDING IMPACT ===================== */
        bool grounded = movement != null && movement.IsGrounded;

        if (!wasGrounded && grounded)
            landingOffset = -landingDrop;

        landingOffset = Mathf.SmoothDamp(
            landingOffset,
            0f,
            ref landingVelocity,
            landingSmooth
        );

        wasGrounded = grounded;

        /* ===================== FOLLOW ===================== */
        Vector3 desiredPosition = target.position + offset;
        desiredPosition.y += currentLookOffsetY + landingOffset;
        desiredPosition.x += lookAheadX;

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
        if (leftBound && rightBound)
        {
            basePosition.x = Mathf.Clamp(
                basePosition.x,
                leftBound.position.x + camWidth,
                rightBound.position.x - camWidth
            );
        }

        if (bottomBound && topBound)
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
