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
    private int waypointIndex = 0;

    private Vector3 lastPosition;

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

        lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (CarGameManager.Get().IsGamePlaying())
        {
            CalculateCarSpeed();
            CalculateLocalVelocityX();

            // Apply motor force to all wheels
            ApplyMotorForce();
            PlayEngineSound();

            ChangeDirectionToWaypoint();
            DetectObstacles();

            // Update wheel meshes to match the wheel colliders
            UpdateWheelMeshes();

            HandleCarFlipping();
        }
    }

    private void DetectObstacles()
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
        //DrawCarRays(carPosition, raycastHit, detectFront, detectFrontLeft, detectFrontRight, detectRight, detectLeft);

        // Check the raycast results and take appropriate action.
        PerformActions(raycastHit, detectFront, detectFrontLeft, detectFrontRight, detectRight, detectLeft);
    }

    private void PerformActions(RaycastHit raycastHit, bool detectFront, bool detectFrontLeft, bool detectFrontRight, bool detectRight, bool detectLeft)
    {
        if ((detectFront || detectFrontLeft) && detectLeft && !detectRight && !detectFrontRight)
        {
            TurnRight();
            //ReleaseHandbrake();

            //if (carSpeed < minimumCarSpeed)
            //{
            //    motorInput = 1;
            //    ReleaseHandbrake();
            //}
        }
        else if (detectLeft && !detectRight && !detectFront && !detectFrontLeft && !detectFrontRight)
        {
            TurnRight();
            //ReleaseHandbrake();

            //if (carSpeed < minimumCarSpeed)
            //{
            //    motorInput = 1;
            //    ReleaseHandbrake();
            //}

            //if (IsDrifting())
            //{
            //    SetWheelStiffness(1.8f);

            //    //Apply brakes If the car lose controlling to avoid strange behaviours
            //    if (localVelocityZ > 1)
            //    {
            //        ApplyHandbrake(1f);
            //    }
            //    else
            //    {
            //        ReleaseHandbrake();
            //    }
            //}
        }
        else if ((detectFront || detectFrontRight) && detectRight && !detectLeft && !detectFrontLeft)
        {
            TurnLeft();
            //ReleaseHandbrake();

            //if (carSpeed < minimumCarSpeed)
            //{
            //    motorInput = 1;
            //}
        }
        else if (detectRight && !detectLeft && !detectFront && !detectFrontLeft && !detectFrontRight)
        {
            TurnLeft();

            //if (carSpeed < minimumCarSpeed)
            //{
            //    motorInput = 1;
            //    ReleaseHandbrake();
            //}

            //if (IsDrifting())
            //{
            //    SetWheelStiffness(1.8f);

            //    //Apply brakes If the car lose controlling to avoid strange behaviours
            //    if (localVelocityZ > 1)
            //    {
            //        ApplyHandbrake(0.4f);
            //    }
            //    else
            //    {
            //        ReleaseHandbrake();
            //    }
            //}
        }
        else if ((detectFront && detectFrontLeft && detectFrontRight) && ((!detectLeft || !detectRight) || (detectRight || detectLeft)))
        {
            if (raycastHit.transform != null)
            {
                //if (raycastHit.transform.CompareTag("Obstacle"))
                //{
                //    ApplyHandbrake(1f);
                //}
                if (raycastHit.transform.CompareTag("Car"))
                {
                    //may hit another Obstacle like car infront.
                    ApplyHandbrake(0.2f);
                }
                //else
                //{
                //    ReleaseHandbrake();
                //}
            }
        }
        else
        {
            if (carSpeed < minimumCarSpeed)
            {
                motorInput = 1;
                ReleaseHandbrake();
            }

            // Don't Rotate. Move froward. 
            DriveForward();
            SetWheelStiffness(1.9f);
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

            waypointIndex++;
        }
        else if (other.CompareTag("Obstacle"))
        {
            if (GetComponent<CarController>().enabled == false)
            {
                ApplyHandbrake(1f);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            Recover();
        }
    }

    public void Recover()
    {
        // this value to ensure that the car position up the track before that value there was a problem 
        // sometimes the cars recoverd under the track and this value solved the problem.
        const float carRecoverYOffset = 2f;

        if (waypointIndex > 0)
        {
            waypointIndex--;
            var waypointTransform = waypoints[waypointIndex].position;
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
        transform.rotation = waypoints[waypointIndex].rotation;
    }

    private void ChangeDirectionToWaypoint()
    {
        Vector3 targetWaypointPosition = waypoints[currentWaypointIndex].position;

        //Steer towards the current waypoint
        float rotationSpeed = 0.8f;
        Vector3 direction = (targetWaypointPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
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
