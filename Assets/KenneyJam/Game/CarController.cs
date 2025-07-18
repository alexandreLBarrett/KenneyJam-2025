using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public CarStats stats;

    private Rigidbody rb;
    private InputAction moveAction;
    private float currentSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");

        // Fallback to default
        if (stats == null)
        {
            stats = ScriptableObject.CreateInstance<CarStats>();
            Debug.LogWarning("CarController has no CarStats assigned! Falling back to the default.");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializePhysicsWithStats();
    }

    private void InitializePhysicsWithStats()
    {
        rb.automaticCenterOfMass = true;
        rb.mass = stats.mass;
        rb.linearDamping = stats.linearDamping;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();

        currentSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);
        
        // Accelerate
        if (currentSpeed < stats.maxSpeed && moveValue.y > 0.001f)
        {
            rb.AddForce(moveValue.y * stats.acceleration * transform.forward, ForceMode.Acceleration);
        }
        // Brake
        else if (currentSpeed > 0 && moveValue.y < -0.001f)
        {
            rb.AddForce(moveValue.y * stats.brakeForce * transform.forward, ForceMode.Acceleration);
        }
        // Reverse
        else if (moveValue.y < -0.001f)
        {
            rb.AddForce(moveValue.y * stats.reverseForce * transform.forward, ForceMode.Acceleration);
        }

        // Steering
        if (currentSpeed > 0.001f && Mathf.Abs(moveValue.x) > 0.1f)
        {
            float turnForce = moveValue.x * stats.turnTorque;
            float speedFactor = Mathf.Clamp01(1f - (Mathf.Abs(currentSpeed) / stats.maxSpeed * 0.5f));
            turnForce *= speedFactor;

            // Stop turning too fast.
            turnForce = Mathf.Sign(moveValue.x) * Mathf.Clamp(Mathf.Abs(turnForce), 0f, stats.maxTurnSpeed);

            rb.AddTorque(0, turnForce, 0, ForceMode.Force);
        }
    }
}
