using UnityEngine;

public class AutoCarController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Wheels")]
    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;

    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;

    [Header("Car Settings")]
    public float maxMotorTorque = 1500f;
    public float maxSteeringAngle = 30f;
    public float maxSpeed = 110f;
    public float downforce = 100f;

    [Header("Effects")]
    public GameObject collisionEffectPrefab;

    private Rigidbody rb;
    private bool isDead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -1f, 0);
    }

    private void FixedUpdate()
    {
        if (isDead || GameManager.instance.playerIsDead)
            return;

        Drive();
        Steer();
        UpdateWheelVisuals();
        LimitSpeed();
        ApplyDownforce();
    }

    private void Drive()
    {
        frontLeftWheelCollider.motorTorque = maxMotorTorque;
        frontRightWheelCollider.motorTorque = maxMotorTorque;
    }

    private void Steer()
    {
        if (target == null) return;

        Vector3 localTarget = transform.InverseTransformPoint(target.position);
        float steer = (localTarget.x / localTarget.magnitude) * maxSteeringAngle;

        frontLeftWheelCollider.steerAngle = steer;
        frontRightWheelCollider.steerAngle = steer;
    }

    private void UpdateWheelVisuals()
    {
        UpdateWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateWheel(WheelCollider col, Transform tr)
    {
        col.GetWorldPose(out Vector3 pos, out Quaternion rot);
        tr.SetPositionAndRotation(pos, rot);
    }

    private void LimitSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    private void ApplyDownforce()
    {
        rb.AddForce(-transform.up * downforce * rb.linearVelocity.magnitude);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            GameManager.instance.TakeDamage();
            HandleDeath(spawnNewCar: true);
        }
    }

    private void HandleDeath(bool spawnNewCar)
    {
        isDead = true;

        if (collisionEffectPrefab != null)
        {
            GameObject fx = Instantiate(
                collisionEffectPrefab,
                transform.position,
                Quaternion.identity
            );
            Destroy(fx, 2f);
        }

        if (spawnNewCar)
            GameManager.instance.SpawnCars();

        // FULL shutdown
        enabled = false;
        rb.isKinematic = true;

        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = false;

        Destroy(gameObject);
    }
}
