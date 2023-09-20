using UnityEngine;
using System;

public class GameInputManager : MonoBehaviour
{
    private static GameInputManager Instance;

    private PlayerInputAction playerInputActions;

    public event EventHandler OnPauseAction;
    public event EventHandler OnBrakeAction;

    private void Awake()
    {
        Instance = this;

        playerInputActions = new PlayerInputAction();
        playerInputActions.Vehicle.Enable();

        playerInputActions.Vehicle.Pause.performed += Pause_performed;
        playerInputActions.Vehicle.Brake.performed += Brake_performed;
    }

    private void Brake_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnBrakeAction?.Invoke(this, EventArgs.Empty);
    }


    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        playerInputActions.Dispose();

        playerInputActions.Vehicle.Pause.performed -= Pause_performed;
        playerInputActions.Vehicle.Brake.performed -= Brake_performed;
    }
    public float GetMovementVertical()
    {
        return playerInputActions.Vehicle.MoveVertically.ReadValue<float>();
    }

    public float GetMovementHorizontal()
    {
        return playerInputActions.Vehicle.MoveHorizontally.ReadValue<float>();
    }

    public bool IsBraking()
    {
        return playerInputActions.Vehicle.Brake.IsPressed();
    }

    public static GameInputManager Get()
    {
        return Instance;
    }
}
