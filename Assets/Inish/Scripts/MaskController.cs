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

    // 🔑 Mask availability comes from GameProgress
    public bool HasMask => GameProgress.HasMask;

    public event Action<MaskState> OnMaskStateChanged;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        SetState(MaskState.Physical);
    }

    void Update()
    {
        // ❌ Mask not unlocked yet
        if (!HasMask)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleMask();
        }
    }

    public void EnableMask()
    {
        GameProgress.HasMask = true;
        Debug.Log("Mask unlocked permanently");
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
        }
        else
        {
            mainCamera.cullingMask =
                LayerMask.GetMask("Player", "Enemies");
        }

        cameraShake?.Shake();

        if (newState == MaskState.Spirit)
            cameraShake?.ZoomToSpirit();
        else
            cameraShake?.ZoomToPhysical();

        OnMaskStateChanged?.Invoke(newState);
    }
}
