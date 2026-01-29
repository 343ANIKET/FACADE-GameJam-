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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleMask();
        }
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

        OnMaskStateChanged?.Invoke(newState);
    }
}
