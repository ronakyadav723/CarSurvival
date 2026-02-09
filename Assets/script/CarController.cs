using TMPro;
using UnityEngine;

public class CarController : MonoBehaviour
{
  
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
    public float maxSpeed = 100f;
    public float downforce = 100f;

    public TMP_Text speedText;

    public float leftrightvalue;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -1f, 0);

        rb.constraints = RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (GameManager.instance.playerIsDead) return;

        speedText.text = "Speed :"+rb.linearVelocity.magnitude.ToString("##");
    }

    void FixedUpdate()
    {
        if (GameManager.instance.playerIsDead) return;

        Drive();
        Steer();
        UpdateWheelVisuals();
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
        float steer = maxSteeringAngle * leftrightvalue;
        frontLeftWheelCollider.steerAngle = steer;
        frontRightWheelCollider.steerAngle = steer;
    }

    void UpdateWheelVisuals()
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
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    void ApplyDownforce()
    {
        rb.AddForce(-transform.up * downforce * rb.linearVelocity.magnitude);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Life"))
        {
            if (GameManager.instance.currentLives < GameManager.instance.maxLives)
            {
                GameManager.instance.currentLives++;
                GameManager.instance.UpdateLivesUI();
            }

            Destroy(other.gameObject);
        }
    }
}
