using UnityEngine;
using System;

public class CarController : Car
{
    public event EventHandler<OnPlayerWinEventArgs> OnPlayerWin;
    public class OnPlayerWinEventArgs : EventArgs
    {
        public bool isPlayerWon = false;
    }

    public event EventHandler OnCarDrifted;

    [Header("Car Effects")]
    [SerializeField] private TrailRenderer[] trails;
    [SerializeField] private ParticleSystem[] drifittingEffects;
    [SerializeField] private GameObject[] nitroEffects;

    [SerializeField] private Transform carIcon;

    [Header("Recover Car depending on current Waypoint index")]
    [SerializeField] private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private Vector3 lastPosition;

    private bool isDrifting = false;
    private const float angularVelocityThreshold = 0.001f;
    private float tempCarMass;
    private bool isNitroReady;

    private void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = bodyMassCenter;

        carEngineSound = GetComponent<AudioSource>();

        // Configure wheel colliders for suspension
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            ConfigureWheelCollider(wheelColliders[i]);
        }

        HideTrails();
        HideDrifittingEffect();
        HideNitroEffect();
    }

    private void Start()
    {
        tempCarMass = carRigidbody.mass;
        lastPosition = transform.position;

        if (!GetComponent<AICarController>().enabled)
        {
            carEngineSound.spatialBlend = 0;
        }

        GameInputManager.Get().OnBrakeAction += CarController_OnBrakeAction;
        GameInputManager.Get().OnNitroActionReady += CarController_OnNitroAction;
        GameInputManager.Get().OnNitroActionCanceled += CarController_OnNitroActionCanceled;
    }

    private void CarController_OnNitroActionCanceled(object sender, EventArgs e)
    {
        isNitroReady = false;
        carRigidbody.mass = tempCarMass;
    }

    private void CarController_OnNitroAction(object sender, EventArgs e)
    {
        if (CarGameManager.Get().IsGamePlaying())
        {
            isNitroReady = true;
            ApplyNitroBoost();
        }
    }

    private void CarController_OnBrakeAction(object sender, System.EventArgs e)
    {
        ApplyHandbrake(0.6f);
    }

    private void FixedUpdate()
    {
        if (CarGameManager.Get().IsGamePlaying())
        {
            CalculateCarSpeed();
            CalculateLocalVelocityX();

            float steeringAngle = maxSteeringAngle * GameInputManager.Get().GetMovementHorizontal();

            //// Apply steering angle to the front wheels
            wheelColliders[0].steerAngle = steeringAngle;
            wheelColliders[1].steerAngle = steeringAngle;

           

            PlayEngineSound();

            if(motorInput == -1 && carSpeed > 100)
            {
                ApplyHandbrake(1);
            }
            else
            {
                ApplyMotorForce();
                ReleaseHandbrake();
            }

            if (isNitroReady && NitroUI.Get().HasNitro())
            {
                motorInput = 1;
            }
            else
            {
                motorInput = GameInputManager.Get().GetMovementVertical();
                SoundManager.Get().StopNitroSound();
                HideNitroEffect();
            }

            //bool isBrake = GameInputManager.Get().IsBraking();

            //if (!isBrake)
            //{
            //    ReleaseHandbrake();
            //}

            if (IsDrifting())
            {
                ShowTrails();
                ShowDrifittingEffect();
                SetWheelStiffness(1.8f); // Increase stiffness a little bit for nice drift and also avoid over control 
            }
            else
            {
                HideTrails();
                HideDrifittingEffect();
                SetWheelStiffness(1.9f); // Reset wheel stiffness to default
            }

            if (IsPerfectDrifting())
            {
                OnCarDrifted?.Invoke(this, EventArgs.Empty);
            }

            UpdateWheelMeshes();

            HandleCarFlipping();
        }
    }

    public float GetCarSpeed() => carSpeed;

    public Transform GetCarIcon() => carIcon;

    public void StopCarAudio() => carEngineSound.Stop();

    public void SetMotorInput(float motorInput) => this.motorInput = motorInput;

    public void Recover()
    {
        // this value to ensure that the car position up the track before that value there was a problem 
        // sometimes the cars recoverd under the track and this value solved the problem.
        const float carRecoverYOffset = 2f;

        if (currentWaypointIndex > 0)
        {
            currentWaypointIndex--;
            var waypointTransform = waypoints[currentWaypointIndex].position;
            transform.position = new Vector3(waypointTransform.x, waypointTransform.y + carRecoverYOffset, waypointTransform.z);
        }
        else
        {
            // If there's no previous checkpoint, move the car to its starting position
            transform.position = lastPosition;
        }

        //set velocity = 0 because in many times the car when get out of the track it flies and when player press recover
        //it continue flying so i set velocity to 0 to sure the car stop.
        carRigidbody.velocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;
        transform.rotation = waypoints[currentWaypointIndex].rotation;
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Waypoint"))
        {
            if (currentWaypointIndex > waypoints.Length) currentWaypointIndex = 0;
            else currentWaypointIndex++;

            // Save the current position as the last position
            lastPosition = transform.position;
        }
        else if (other.CompareTag("FinishLine") && !isWrongDirection)
        {
            CarGameManager.Get().SetGameOver(true);
            StartCoroutine(TriggerSlowMotion());
            carEngineSound.Stop();
            OnPlayerWin?.Invoke(this, new OnPlayerWinEventArgs { isPlayerWon = true });
        }
    }

    private void ApplyNitroBoost()
    {
        carRigidbody.mass = 300;

        ShowNitroEffect();
        SoundManager.Get().PlayNitroSound();
    }

    private void HideTrails()
    {
        foreach (TrailRenderer item in trails)
        {
            item.emitting = false;
        }
    }

    private void ShowTrails()
    {
        foreach (TrailRenderer item in trails)
        {
            item.emitting = true;
        }
    }

    private void ShowNitroEffect()
    {
        foreach (GameObject particle in nitroEffects)
        {
            particle.SetActive(true);
        }
    }

    private void HideNitroEffect()
    {
        foreach (GameObject particle in nitroEffects)
        {
            particle.SetActive(false);
        }
    }

    private void ShowDrifittingEffect()
    {
        foreach (ParticleSystem particle in drifittingEffects)
        {
            particle.Play();
        }
    }

    private void HideDrifittingEffect()
    {
        foreach (ParticleSystem particle in drifittingEffects)
        {
            particle.Stop();
        }
    }

    private bool IsPerfectDrifting()
    {
        if (Mathf.Abs(localVelocityX) > 30 && carSpeed > 90 && !isDrifting)
        {
            isDrifting = true;
            return true;
        }
        else if (Mathf.Abs(carRigidbody.angularVelocity.magnitude) < angularVelocityThreshold)
        {
            isDrifting = false;
        }

        return false;
    }
}