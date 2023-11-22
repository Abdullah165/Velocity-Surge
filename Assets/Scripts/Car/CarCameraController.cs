using UnityEngine;
using Cinemachine;
using System.Collections;

public class CarCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook CarCamera;

    [SerializeField] private float oldRadius = -10;
    [SerializeField] private float newRadius = -17;

    private const float transitionSpeed = 2.0f;

    private float transitionStartTime;
    private bool isTransitionReady = false;

    private Coroutine nitroCameraCoroutine;
    private Coroutine normalCameraCoroutine;

    void Start()
    {
        CarCamera.m_Orbits[0].m_Radius = newRadius;
        GameInputManager.Get().OnNitroActionReady += CarCameraController_OnNitroAction;
        GameInputManager.Get().OnNitroActionCanceled += CarCameraController_OnNitroActionCanceled;
    }

    private void CarCameraController_OnNitroActionCanceled(object sender, System.EventArgs e)
    {
        if (CarGameManager.Get().IsGamePlaying())
        {
            transitionStartTime = Time.time;
            isTransitionReady = false;

            StopNitroCameraCoroutine();
            StartNormalCameraCoroutine();
        }
    }

    private void CarCameraController_OnNitroAction(object sender, System.EventArgs e)
    {
        if (CarGameManager.Get().IsGamePlaying())
        {
            transitionStartTime = Time.time;
            isTransitionReady = true;

            StartNitroCameraCoroutine();
            StopNormalCameraCoroutine();
        }
    }

    IEnumerator NitroCameraPosition()
    {
        while (isTransitionReady && CarGameManager.Get().IsGamePlaying())
        {
            float t = (Time.time - transitionStartTime) * transitionSpeed;
            CarCamera.m_Orbits[0].m_Radius = Mathf.Lerp(oldRadius, newRadius, t);

            yield return null;
        }
    }

    IEnumerator NormalCameraPosition()
    {
        while (!isTransitionReady && CarGameManager.Get().IsGamePlaying())
        {
            float t = (Time.time - transitionStartTime) * transitionSpeed;
            CarCamera.m_Orbits[0].m_Radius = Mathf.Lerp(newRadius, oldRadius, t);

            yield return null;
        }
    }

    private void StartNitroCameraCoroutine()
    {
        if (nitroCameraCoroutine == null)
        {
            nitroCameraCoroutine = StartCoroutine(NitroCameraPosition());
        }
    }

    private void StopNitroCameraCoroutine()
    {
        if (nitroCameraCoroutine != null)
        {
            StopCoroutine(nitroCameraCoroutine);
            nitroCameraCoroutine = null;
        }
    }

    private void StartNormalCameraCoroutine()
    {
        if (normalCameraCoroutine == null)
        {
            normalCameraCoroutine = StartCoroutine(NormalCameraPosition());
        }
    }

    private void StopNormalCameraCoroutine()
    {
        if (normalCameraCoroutine != null)
        {
            StopCoroutine(normalCameraCoroutine);
            normalCameraCoroutine = null;
        }
    }
}

