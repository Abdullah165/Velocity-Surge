using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour
{
    [SerializeField] protected Transform[] wheelMeshes;
    [SerializeField] protected WheelCollider[] wheelColliders;
    [SerializeField] protected float maxSteeringAngle = 30f;
    [SerializeField] protected float motorForce = 800f;
    [SerializeField] protected float brakeForce = 2000f;

    [SerializeField] protected Vector3 bodyMassCenter;
    [SerializeField] protected float suspensionDistance = 0.2f;

    protected Rigidbody carRigidbody;
    protected float localVelocityX;
    protected float localVelocityZ;
    protected float carSpeed;
    protected const float minimumCarSpeed = 70f;
    protected float motorInput = 1;

    protected AudioSource carEngineSound;
    protected const float minPitch = 0.1f;
    protected const float maxPitch = 0.9f;
    protected const float slowMotionScale = 0.3f;

    protected bool isWrongDirection;

    protected void ApplyMotorForce()
    {
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            wheelColliders[i].motorTorque = motorInput * motorForce;
        }
    }

    protected void PlayEngineSound()
    {
        float engineSoundPitch = minPitch + (Mathf.Abs(carRigidbody.velocity.magnitude) / 60f);
        carEngineSound.pitch = engineSoundPitch;

        if (carSpeed < 1)
        {
            carEngineSound.Play();
        }
    }

    protected void CalculateCarSpeed()
    {
        //determine the car speed in m/s if you want to change it to Km/h use this formula : kilometers per hour = meters per second × 3.6
        carSpeed = carRigidbody.velocity.magnitude;
    }

    protected void CalculateLocalVelocityX()
    {
        localVelocityX = transform.InverseTransformDirection(carRigidbody.velocity).x;
    }

    protected void CalculateLocalVelocityZ()
    {
        localVelocityZ = transform.InverseTransformDirection(carRigidbody.velocity).z;
    }

    protected void ConfigureWheelCollider(WheelCollider collider)
    {
        collider.suspensionDistance = suspensionDistance;
    }

    protected void UpdateWheelMeshes()
    {
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            wheelColliders[i].GetWorldPose(out Vector3 position, out Quaternion rotation);
            wheelMeshes[i].SetPositionAndRotation(position, rotation);
        }
    }

    protected void HandleCarFlipping()
    {
        if (carSpeed <= 1)
        {
            if ((transform.eulerAngles.z <= 0 && transform.eulerAngles.z <= -80) || transform.eulerAngles.z >= 80)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
            }
        }
    }

    protected bool IsDrifting()
    {
        if (Mathf.Abs(localVelocityX) > 8 && carSpeed > minimumCarSpeed)
        {
            return true;
        }
        return false;
    }

    protected void SetWheelStiffness(float stiffness)
    {
        for (int wheelIndex = 2; wheelIndex < wheelColliders.Length; wheelIndex++)
        {
            // Get the default WheelFrictionCurve from the Wheel Collider
            WheelFrictionCurve frictionCurve = wheelColliders[wheelIndex].sidewaysFriction;

            // Modify the stiffness value
            frictionCurve.stiffness = stiffness;

            // Apply the modified WheelFrictionCurve back to the Wheel Collider
            wheelColliders[wheelIndex].sidewaysFriction = frictionCurve;
        }
    }

    protected void ApplyHandbrake(float strength)
    {
        foreach (WheelCollider wheel in wheelColliders)
        {
            wheel.brakeTorque = brakeForce * strength;
        }
    }

    protected void ReleaseHandbrake()
    {
        foreach (WheelCollider wheel in wheelColliders)
        {
            wheel.brakeTorque = 0;
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WrongDirection"))
        {
            if (GetComponent<AICarController>().enabled == true)
            {
                gameObject.SetActive(false);
            }
            isWrongDirection = true;
        }
    }

    protected IEnumerator TriggerSlowMotion()
    {
        Time.timeScale = slowMotionScale;
        yield return new WaitForSeconds(0.3f);
        Time.timeScale = 1;
        GameOverUI.Get().Show();
    }
}
