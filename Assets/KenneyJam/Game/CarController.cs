using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using KenneyJam.Game.PlayerCar;

public class CarController : MonoBehaviour
{
    public CarStats stats;

    private Rigidbody rb;
    private float currentSpeed;
    public float currentHealth;

    public UnityEvent<Transform /* carTransform */> onCarDeath;
    public UnityEvent<float /* damage */, CarController /* damageDealer */, bool /* shouldPlaySound */> onDamageTaken;
    public UnityEvent<float/*health*/, float/*maxHealth*/> onHealthChanged;

    private AudioSource engineAudioSource;
    private float lerpedEngineVolume = .05f;
    private ModularCar modularCar;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        modularCar = GetComponent<ModularCar>();

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

        currentHealth = stats.maxHealth;

        onDamageTaken.AddListener((damage, damageDealer, playHitSound) => {
            if (currentHealth <= 0)
            {
                SoundManager.Instance.PlayInstantSound(SoundManager.Instance.soundBank.CarBreak);
                rb.AddForce(((transform.position - damageDealer.transform.position).normalized + Vector3.up) * 50000, ForceMode.Impulse);
            }
            else if (playHitSound)
            {
                SoundManager.Instance.PlayInstantSound(SoundManager.Instance.soundBank.CarDamageTaken, .3f);
            }
        });
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
        if (engineAudioSource == null)
        {
            if (Time.timeSinceLevelLoad > .8)
                engineAudioSource = SoundManager.Instance.CreatePermanentAudioSource(SoundManager.Instance.soundBank.engineSound);
            else
                return;
        }
        lerpedEngineVolume = Mathf.Lerp(lerpedEngineVolume, Mathf.Clamp01(
            Mathf.Max(Mathf.Abs(currentSpeed) / stats.maxSpeed * stats.engineVolumeLinearVelocityFactor, rb.angularVelocity.magnitude * stats.engineVolumeAngularVelocityFactor)),
            stats.engineVolumeLerp);
        engineAudioSource.volume = lerpedEngineVolume * stats.engineGlobalVolume;
        engineAudioSource.pitch = 1 + lerpedEngineVolume * .3f - .15f;
    }

    private void OnDestroy()
    {
        SoundManager.Instance.FadeOutPermanentAudioSource(engineAudioSource);
    }

    public void UpdateMovement(float engineInput, float steeringInput)
    {
        // OverlapBox below the car to check if wheels are still on the ground.
        Collider[] cols = Physics.OverlapBox(transform.position + new Vector3(0.07f / 8.0f, 0, 0.07f / 8.0f), new(0.07f, 0.01f, 0.07f), transform.rotation);
        List<Collider> actualCols = new();
        foreach (Collider col in cols)
        {
            if (col.transform.root.gameObject != transform.root.gameObject)
            {
                actualCols.Add(col);
            }
        }
        if (actualCols.Count == 0)
        {
            return;
        }

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
        if (Mathf.Abs(steeringInput) > 0.05f)
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

    public void InflictDamage(CarController damageDealer, float damageValue, bool playHitSound = true)
    {
        currentHealth = currentHealth - Mathf.Max(damageValue - modularCar.GetArmorValue(), 0);
        onDamageTaken.Invoke(currentHealth, damageDealer, playHitSound);
        onHealthChanged.Invoke(currentHealth, stats.maxHealth);
        if (currentHealth <= 0)
        {
            onCarDeath.Invoke(transform);
            onCarDeath.RemoveAllListeners();
            onDamageTaken.RemoveAllListeners();
            onHealthChanged.RemoveAllListeners();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // TODO play sound
        //Debug.Log(collision.gameObject.name + " " + collision.impulse + " " + collision.relativeVelocity);
    }
}
