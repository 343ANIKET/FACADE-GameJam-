using System;
using UnityEngine;

public enum MaskState
{
    Physical,
    Spirit
}

public class MaskController : MonoBehaviour
{
    public static MaskController Instance;

    [Header("Camera")]
    public Camera mainCamera;

    [Header("Camera Effects")]
    public CameraShake cameraShake;

    public MaskState CurrentState { get; private set; }

    public event Action<MaskState> OnMaskStateChanged;

    // 🔒 LOCK UNTIL MASK IS PICKED UP
    public bool HasMask { get; private set; } = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Start in Physical world but LOCKED
        SetState(MaskState.Physical);
    }

    void Update()
    {
        // ❌ Mask not owned → ignore input
        if (!HasMask)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleMask();
        }
    }

    public void EnableMask()
    {
        HasMask = true;
        Debug.Log("Mask enabled – player can now toggle worlds");
    }

    public void ToggleMask()
    {
        SetState(CurrentState == MaskState.Physical
            ? MaskState.Spirit
            : MaskState.Physical);
    }

    public void SetState(MaskState newState)
    {
        CurrentState = newState;

        if (newState == MaskState.Physical)
        {
            mainCamera.cullingMask =
                LayerMask.GetMask("Player", "Environment");
            cameraShake?.ZoomToPhysical();
        }
        else
        {
            mainCamera.cullingMask =
                LayerMask.GetMask("Player", "Enemies");
            cameraShake?.ZoomToSpirit();
        }

        cameraShake?.Shake();
        OnMaskStateChanged?.Invoke(newState);
    }
}
