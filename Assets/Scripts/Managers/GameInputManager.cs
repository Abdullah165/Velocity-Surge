using UnityEngine;
using System;

public class GameInputManager : MonoBehaviour
{
    private static GameInputManager Instance;

    private PlayerInputAction playerInputActions;

    public event EventHandler OnPauseAction;
    public event EventHandler OnBrakeAction;
    public event EventHandler OnNitroActionReady;
    public event EventHandler OnNitroActionCanceled;

    private void Awake()
    {
        Instance = this;

        playerInputActions = new PlayerInputAction();
        playerInputActions.Vehicle.Enable();

        playerInputActions.Vehicle.Pause.performed += Pause_performed;
        playerInputActions.Vehicle.Brake.performed += Brake_performed;
        playerInputActions.Vehicle.Nitro.performed += Nitro_performed;
        playerInputActions.Vehicle.Nitro.canceled += Nitro_canceled;
    }

    private void Nitro_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnNitroActionCanceled?.Invoke(this, EventArgs.Empty);
    }

    private void Nitro_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnNitroActionReady?.Invoke(this, EventArgs.Empty);
    }

    private void Brake_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnBrakeAction?.Invoke(this, EventArgs.Empty);
    }


    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (CarGameManager.Get().IsGamePlaying())
        {
            OnPauseAction?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnDestroy()
    {
        playerInputActions.Dispose();

        playerInputActions.Vehicle.Pause.performed -= Pause_performed;
        playerInputActions.Vehicle.Brake.performed -= Brake_performed;
        playerInputActions.Vehicle.Nitro.performed -= Nitro_performed;
        playerInputActions.Vehicle.Nitro.canceled -= Nitro_canceled;
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
