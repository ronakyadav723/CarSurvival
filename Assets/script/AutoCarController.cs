using UnityEngine;

public class AutoCarController : MonoBehaviour
{
    public Transform target;
    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;

    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;

    
    public float maxMotorTorque = 1500f;
    public float maxSteeringAngle = 30f;
    public float maxSpeed = 110f;
    public float downforce = 100f;


    public GameObject collisionEffectPrefab;

    Rigidbody rb;
    bool isDead;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -1f, 0);
        rb.constraints = RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        if (isDead || GameManager.instance.playerIsDead) return;

        Drive();
        Steer();
        UpdateWheels();
        LimitSpeed();
        ApplyDownforce();
    }

    void Drive()
    {
        frontLeftWheelCollider.motorTorque = maxMotorTorque;
        frontRightWheelCollider.motorTorque = maxMotorTorque;
        rearLeftWheelCollider.motorTorque = maxMotorTorque;
        rearRightWheelCollider.motorTorque = maxMotorTorque;
    }

    void Steer()
    {
        if (!target) return;

        Vector3 localTarget = transform.InverseTransformPoint(target.position);
        float steer = (localTarget.x / localTarget.magnitude) * maxSteeringAngle;

        frontLeftWheelCollider.steerAngle = steer;
        frontRightWheelCollider.steerAngle = steer;
    }

    void UpdateWheels()
    {
        UpdateWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    void UpdateWheel(WheelCollider col, Transform tr)
    {
        col.GetWorldPose(out Vector3 pos, out Quaternion rot);
        tr.SetPositionAndRotation(pos, rot);
    }

    void LimitSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity= rb.linearVelocity.normalized * maxSpeed;
    }

    void ApplyDownforce()
    {
        rb.AddForce(-transform.up * downforce * rb.linearVelocity.magnitude);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            GameManager.instance.TakeDamage();
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (collisionEffectPrefab)
        {
            Destroy(
                Instantiate(collisionEffectPrefab, transform.position, Quaternion.identity),
                2f
            );
        }

        GameManager.instance.NotifyPoliceDestroyed(this);
        Destroy(gameObject);
    }
}
