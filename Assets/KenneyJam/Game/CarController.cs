using UnityEngine;

public class CarController : MonoBehaviour
{
    public CarStats stats;

    private Rigidbody rb;
    private float currentSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Fallback to default
        if (stats == null)
        {
            stats = ScriptableObject.CreateInstance<CarStats>();
            Debug.LogWarning("CarController has no CarStats assigned! Falling back to the default.");
        }
    }

    void Start()
    {
        InitializePhysicsWithStats();
    }

    private void InitializePhysicsWithStats()
    {
        rb.automaticCenterOfMass = true;
        rb.mass = stats.mass;
        rb.linearDamping = stats.linearDamping;
        rb.angularDamping = stats.angularDamping;
    }

    public void UpdateMovement(float engineInput, float steeringInput)
    {
        currentSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);
        
        // Accelerate
        if (currentSpeed < stats.maxSpeed && engineInput > 0.001f)
        {
            rb.AddForce(engineInput * stats.acceleration * transform.forward, ForceMode.Acceleration);
        }
        // Brake
        else if (currentSpeed > 0 && engineInput < -0.001f)
        {
            rb.AddForce(engineInput * stats.brakeForce * transform.forward, ForceMode.Acceleration);
        }
        // Reverse
        else if (engineInput < -0.001f)
        {
            rb.AddForce(engineInput * stats.reverseForce * transform.forward, ForceMode.Acceleration);
        }

        // Steering
        if (Mathf.Abs(currentSpeed) > 0.001f && Mathf.Abs(steeringInput) > 0.1f)
        {
            float turnForce = steeringInput * stats.turnTorque;
            float speedFactor = Mathf.Clamp01(1f - (Mathf.Abs(currentSpeed) / stats.maxSpeed * 0.5f));
            turnForce *= speedFactor;

            // Stop turning too fast.
            turnForce = Mathf.Sign(steeringInput) * Mathf.Clamp(Mathf.Abs(turnForce), 0f, stats.maxTurnSpeed);
            turnForce *= Mathf.Sign(currentSpeed);

            rb.AddTorque(0, turnForce, 0, ForceMode.Force);
        }
    }
}
