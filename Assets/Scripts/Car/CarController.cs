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

    [SerializeField] private Transform carIcon;

    [Header("Recover Car depending on current Waypoint index")]
    [SerializeField] private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private Vector3 lastPosition;

    private bool isDrifting = false;
    private const float angularVelocityThreshold = 0.001f;

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
    }

    private void Start()
    {
        lastPosition = transform.position;

        if (!GetComponent<AICarController>().enabled)
        {
            carEngineSound.spatialBlend = 0;
        }

        GameInputManager.Get().OnBrakeAction += CarController_OnBrakeAction;
    }

    private void CarController_OnBrakeAction(object sender, System.EventArgs e)
    {
        ApplyHandbrake(1);
    }

    private void FixedUpdate()
    {
        if (CarGameManager.Get().IsGamePlaying())
        {
            CalculateCarSpeed();
            CalculateLocalVelocityX();
            CalculateLocalVelocityZ();

            float steeringAngle = maxSteeringAngle * GameInputManager.Get().GetMovementHorizontal();
            motorInput = GameInputManager.Get().GetMovementVertical();

            //// Apply steering angle to the front wheels
            wheelColliders[0].steerAngle = steeringAngle;
            wheelColliders[1].steerAngle = steeringAngle;

            // Apply motor force to all wheels
            ApplyMotorForce();
            PlayEngineSound();

            bool isBrake = GameInputManager.Get().IsBraking();

            if (!isBrake)
            {
                ReleaseHandbrake();
            }

            //Car lose traction and apply drifting effects.
            if (IsDrifting())
            {
                ShowTrails();
                ShowDrifittingEffect();
                SetWheelStiffness(1.2f); // Reduce wheel stiffness for oversteer

                //Apply brakes If the car lose controlling to avoid strange behaviours
                if (localVelocityZ > 1)
                {
                    ApplyHandbrake(0.01f);
                }
            }
            else
            {
                HideTrails();
                HideDrifittingEffect();
                SetWheelStiffness(1.6f); // Reset wheel stiffness to default
            }

            if (IsPerfectDrifting())
            {
                OnCarDrifted?.Invoke(this, EventArgs.Empty);
            }

            // Update wheel meshes to match the wheel colliders
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
        if (currentWaypointIndex > 0)
        {
            currentWaypointIndex--;
            transform.position = waypoints[currentWaypointIndex].position;
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
        transform.rotation = waypoints[currentWaypointIndex - 1].rotation;
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