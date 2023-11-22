using UnityEngine;
using Cinemachine;

public class CamerSwitcherManager : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook finishLineCamera;

    private void Start()
    {
        CarGameManager.Get().OnStateChanged += CamerSwitcherManager_OnStateChanged;
    }

    private void CamerSwitcherManager_OnStateChanged(object sender, CarGameManager.OnStateChangedEventArgs e)
    {
        if (e.state == CarGameManager.State.GameOver)
        {
            //change priority of cameras to make switch between them.
            SwitchCamera();
        }
        else if (e.state == CarGameManager.State.CountDownToStart || e.state == CarGameManager.State.GamePlaying)
        {
            ResetCameraPriority();
        }
    }

    private void ResetCameraPriority()
    {
        finishLineCamera.Priority = 0;
    }
    
    private void SwitchCamera()
    {
        finishLineCamera.Priority = 20;
    }
}
