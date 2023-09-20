using UnityEngine;
using System;

public class AICarController : Car
{
    [Header("Rays Specifications")]
    [Range(15, 35)]
    [SerializeField] private float rayDistance = 20;

    [Range(25, 45)]
    [SerializeField] private float rayAngle = 25;
    private readonly float rayOffset = 1f;

    [Header("Car direction depending on current Waypoint")]
    [SerializeField] private Transform[] waypoints;
    private int currentWaypointIndex = 0;

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
    }

    private void FixedUpdate()
    {
        if (CarGameManager.Get().IsGamePlaying())
        {
            CalculateCarSpeed();
            CalculateLocalVelocityX();
            CalculateLocalVelocityZ();

            // Apply motor force to all wheels
            ApplyMotorForce();
            PlayEngineSound();

            ChangeDirectionToWaypoint();
            DetectCars();

            // Update wheel meshes to match the wheel colliders
            UpdateWheelMeshes();

            HandleCarFlipping();
        }
    }

    private void DetectCars()
    {
        // Get the car's position and rotation.
        Vector3 carPosition = transform.position;
        Quaternion carRotation = transform.rotation;

        // Perform the raycasts.
        bool detectFront = Physics.Raycast(carPosition + Vector3.up, carRotation * Vector3.forward, out RaycastHit raycastHit, rayDistance);
        bool detectFrontLeft = Physics.Raycast(carPosition + Vector3.up + Vector3.left, carRotation * Vector3.forward, out raycastHit, rayDistance);
        bool detectFrontRight = Physics.Raycast(carPosition + Vector3.up + Vector3.right, carRotation * Vector3.forward, out raycastHit, rayDistance);
        bool detectRight = Physics.Raycast(carPosition + Vector3.up + transform.rotation * Vector3.right * rayOffset, Quaternion.Euler(0, rayAngle, 0) * transform.rotation * Vector3.forward, out raycastHit, rayDistance);
        bool detectLeft = Physics.Raycast(carPosition + Vector3.up + transform.rotation * Vector3.left * rayOffset, Quaternion.Euler(0, -rayAngle, 0) * transform.rotation * Vector3.forward, out raycastHit, rayDistance);

        // You Can get ride of this function (DrawCarRays) it's just for drawing the rays.
        DrawCarRays(carPosition, raycastHit, detectFront, detectFrontLeft, detectFrontRight, detectRight, detectLeft);

        // Check the raycast results and take appropriate action.
        PerformActions(raycastHit, detectFront, detectFrontLeft, detectFrontRight, detectRight, detectLeft);
    }

    private void PerformActions(RaycastHit raycastHit, bool detectFront, bool detectFrontLeft, bool detectFrontRight, bool detectRight, bool detectLeft)
    {
        if ((detectFront || detectFrontLeft) && detectLeft && !detectRight && !detectFrontRight)
        {
            TurnRight();
            ReleaseHandbrake();

            if (carSpeed < minimumCarSpeed)
            {
                motorInput = 1;
            }
        }
        else if (detectLeft && !detectRight && !detectFront && !detectFrontLeft && !detectFrontRight)
        {
            TurnRight();
            ReleaseHandbrake();

            if (carSpeed < minimumCarSpeed)
            {
                motorInput = 1;
            }

            if (IsDrifting())
            {
                SetWheelStiffness(1.5f);

                //Apply brakes If the car lose controlling to avoid strange behaviours
                if (localVelocityZ > 1)
                {
                    ApplyHandbrake(0.1f);
                }
                else
                {
                    ReleaseHandbrake();
                }
            }
        }
        else if ((detectFront || detectFrontRight) && detectRight && !detectLeft && !detectFrontLeft)
        {
            TurnLeft();
            ReleaseHandbrake();

            if (carSpeed < minimumCarSpeed)
            {
                motorInput = 1;
            }
        }
        else if (detectRight && !detectLeft && !detectFront && !detectFrontLeft && !detectFrontRight)
        {
            TurnLeft();
            ReleaseHandbrake();

            if (carSpeed < minimumCarSpeed)
            {
                motorInput = 1;
            }

            if (IsDrifting())
            {
                SetWheelStiffness(1.5f);

                //Apply brakes If the car lose controlling to avoid strange behaviours
                if (localVelocityZ > 1)
                {
                    ApplyHandbrake(0.1f);
                }
                else
                {
                    ReleaseHandbrake();
                }
            }
        }
        else if ((detectFront && detectFrontLeft && detectFrontRight) && ((!detectLeft || !detectRight) || (detectRight || detectLeft)))
        {
            if (raycastHit.transform != null)
            {
                if (raycastHit.transform.CompareTag("Obstacle"))
                {
                    motorInput = -1;
                }
                else if (raycastHit.transform.CompareTag("Car"))
                {
                    //may hit another Obstacle like car infront.
                    ApplyHandbrake(20f);
                }
                else
                {
                    motorInput = 1;
                }
            }
        }
        else
        {
            if (carSpeed < minimumCarSpeed)
            {
                motorInput = 1;
            }

            // Don't Rotate. Move froward. 
            DriveForward();
            ReleaseHandbrake();
            SetWheelStiffness(1.8f);
        }
    }

    protected void TurnRight()
    {
        wheelColliders[0].steerAngle = maxSteeringAngle;
        wheelColliders[1].steerAngle = maxSteeringAngle;
    }

    protected void TurnLeft()
    {
        wheelColliders[0].steerAngle = -maxSteeringAngle;
        wheelColliders[1].steerAngle = -maxSteeringAngle;
    }

    protected void DriveForward()
    {
        wheelColliders[0].steerAngle = 0;
        wheelColliders[1].steerAngle = 0;
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("FinishLine") && !isWrongDirection)
        {
            CarGameManager.Get().SetGameOver(true);
            StartCoroutine(TriggerSlowMotion());
        }
        else if (other.CompareTag("Waypoint"))
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
            }
        }
    }

    private void ChangeDirectionToWaypoint()
    {
        Vector3 targetWaypointPosition = waypoints[currentWaypointIndex].position;

        //Steer towards the current waypoint
        float rotationSpeed = 2f;
        Vector3 direction = (targetWaypointPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
    }

    private void DrawCarRays(Vector3 carPosition, RaycastHit raycastHit, bool detectFront, bool detectFrontLeft, bool detectFrontRight, bool detectRight, bool detectLeft)
    {
        // Front raycast
        if (detectFront)
        {
            Debug.DrawRay(carPosition + Vector3.up, transform.TransformDirection(Vector3.forward) * raycastHit.distance, Color.red);
        }
        else
        {
            Debug.DrawRay(carPosition + Vector3.up, transform.TransformDirection(Vector3.forward) * rayDistance, Color.green);
        }

        //Front Left raycast
        if (detectFrontLeft)
        {
            Debug.DrawRay(carPosition + Vector3.up + Vector3.left, transform.TransformDirection(Vector3.forward) * raycastHit.distance, Color.red);
        }
        else
        {
            Debug.DrawRay(carPosition + Vector3.up + Vector3.left, transform.TransformDirection(Vector3.forward) * rayDistance, Color.green);
        }

        // Front Right raycast
        if (detectFrontRight)
        {
            Debug.DrawRay(carPosition + Vector3.up + Vector3.right, transform.TransformDirection(Vector3.forward) * raycastHit.distance, Color.red);
        }
        else
        {
            Debug.DrawRay(carPosition + Vector3.up + Vector3.right, transform.TransformDirection(Vector3.forward) * rayDistance, Color.green);
        }

        // Right raycast
        if (detectRight)
        {
            Debug.DrawRay(carPosition + Vector3.up + transform.rotation * Vector3.right * rayOffset, Quaternion.Euler(0, rayAngle, 0) * transform.rotation * Vector3.forward * rayDistance, Color.red);
        }
        else
        {
            Debug.DrawRay(carPosition + Vector3.up + transform.rotation * Vector3.right * rayOffset, Quaternion.Euler(0, rayAngle, 0) * transform.rotation * Vector3.forward * rayDistance, Color.green);
        }

        // Left raycast
        if (detectLeft)
        {
            Debug.DrawRay(carPosition + Vector3.up + transform.rotation * Vector3.left * rayOffset, Quaternion.Euler(0, -rayAngle, 0) * transform.rotation * Vector3.forward * rayDistance, Color.red);
        }
        else
        {
            Debug.DrawRay(carPosition + Vector3.up + transform.rotation * Vector3.left * rayOffset, Quaternion.Euler(0, -rayAngle, 0) * transform.rotation * Vector3.forward * rayDistance, Color.green);
        }
    }
}
