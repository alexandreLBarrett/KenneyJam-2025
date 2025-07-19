using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

public class CarController : MonoBehaviour
{
    public CarStats stats;

    private Rigidbody rb;
    private float currentSpeed;
    private float currentHealth;

    public UnityEvent<Transform /* carTransform */> onCarDeath;
    public UnityEvent<float /* damage */, CarController /* car */> onDamageTaken;
    public UnityEvent<float/*health*/, float/*maxHealth*/> onHealthChanged;

    private AudioSource engineAudioSource;
    private float lerpedEngineVolume = .05f;

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

        engineAudioSource = SoundManager.Instance.CreatePermanentAudioSource(SoundManager.Instance.soundBank.engineSound);
        currentHealth = stats.maxHealth;
    }

    private void InitializePhysicsWithStats()
    {
        //rb.automaticCenterOfMass = true;
        rb.mass = stats.mass;
        rb.linearDamping = stats.linearDamping;
        rb.angularDamping = stats.angularDamping;
    }
    float Smoothstep(float edge0, float edge1, float x)
    {
        // Scale, and clamp x to 0..1 range
        x = Mathf.Clamp01((x - edge0) / (edge1 - edge0));

        return x * x * (3.0f - 2.0f * x);
    }

    private void FixedUpdate()
    {
        lerpedEngineVolume = Mathf.Lerp(lerpedEngineVolume, Mathf.Clamp01(
            Mathf.Max(Mathf.Abs(currentSpeed) / stats.maxSpeed * stats.engineVolumeLinearVelocityFactor, rb.angularVelocity.magnitude * stats.engineVolumeAngularVelocityFactor)),
            stats.engineGlobalVolue);
        engineAudioSource.volume = lerpedEngineVolume;
        engineAudioSource.pitch = 1 + lerpedEngineVolume * .3f - .15f;
    }

    private void OnDestroy()
    {
        SoundManager.Instance.FadeOutPermanentAudioSource(engineAudioSource);
    }

    public void UpdateMovement(float engineInput, float steeringInput)
    {
        currentSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);
        
        // Accelerate (when either turning or when moving forwards)
        if (currentSpeed < stats.maxSpeed && (engineInput > 0.001f || Mathf.Abs(steeringInput) > 0.1f))
        {
            rb.AddForce(Mathf.Max(Mathf.Abs(engineInput), Mathf.Abs(steeringInput)) * Mathf.Sign(engineInput) * stats.acceleration * transform.forward, ForceMode.Acceleration);
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
        if (Mathf.Abs(steeringInput) > 0.1f)
        {
            float turnForce = Mathf.Sign(steeringInput) * stats.turningMotorCurve.Evaluate(engineInput) * stats.turnTorque;

            // Stop the car from spinning out (reduce turn force when moving slowly).
            float speedTurnMask = Smoothstep(stats.turnSmoothingEdge1, stats.turnSmoothingEdge2, Mathf.Abs(currentSpeed) / stats.maxSpeed);
            turnForce *= speedTurnMask;

            // Clamp the turning speed to the max.
            turnForce = Mathf.Sign(steeringInput) * Mathf.Clamp(Mathf.Abs(turnForce), 0f, stats.maxTurnSpeed);
            turnForce *= Mathf.Sign(currentSpeed);

            rb.AddTorque(0, turnForce, 0, ForceMode.Force);
        }
    }

    public void InflictDamage(float damageValue)
    {
        currentHealth -= damageValue;
        onDamageTaken.Invoke(currentHealth, this);
        if (currentHealth < 0)
        {
            onCarDeath.Invoke(transform);
            onCarDeath.RemoveAllListeners();
            Destroy(transform.root.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // TODO play sound
        //Debug.Log(collision.gameObject.name + " " + collision.impulse + " " + collision.relativeVelocity);
    }
}
