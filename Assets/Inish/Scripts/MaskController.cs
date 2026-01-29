using System;          // REQUIRED for Action
using UnityEngine;

public enum MaskState
{
    Physical,
    Spirit
}

public class MaskController : MonoBehaviour
{
    public static MaskController Instance;

    [Header("Cameras")]
    public Camera PhysicalCamera;
    public Camera SpiritCamera;

    public MaskState CurrentState { get; private set; }

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
        // Free toggle input
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleMask();
        }
    }

    public void ToggleMask()
    {
        if (CurrentState == MaskState.Physical)
            SetState(MaskState.Spirit);
        else
            SetState(MaskState.Physical);
    }

    public void SetState(MaskState newState)
    {
        CurrentState = newState;

        PhysicalCamera.enabled = (newState == MaskState.Physical);
        SpiritCamera.enabled   = (newState == MaskState.Spirit);

        OnMaskStateChanged?.Invoke(newState);
    }
}
