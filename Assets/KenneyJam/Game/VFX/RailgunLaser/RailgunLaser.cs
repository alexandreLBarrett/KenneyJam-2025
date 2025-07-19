using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class RailgunLaser : MonoBehaviour
{
    public float totalLifespan = 0.6f;
    public float startRamp = 0.1f;
    public float endRamp = 0.7f;
    private float currentTime = 0;

    private LineRenderer lineRenderer;
    private ParticleSystem particleSystem;

    void Awake()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
        particleSystem = GetComponent<ParticleSystem>();
        particleSystem.startLifetime = totalLifespan;
    }

    void Start()
    {
    }

    float Smoothstep(float edge0, float edge1, float x)
    {
        // Scale, and clamp x to 0..1 range
        x = Mathf.Clamp01((x - edge0) / (edge1 - edge0));

        return x * x * (3.0f - 2.0f * x);
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > totalLifespan)
        {
            // Laser done firing, kill.
            //Destroy(gameObject);
            //return;
            currentTime = 0;
        }

        // [0, totalLifespan] to [0, 1]
        float currentLifespan = currentTime / totalLifespan;

        // Add build up / fade out to the laser.
        float startMask = Smoothstep(0, startRamp, currentLifespan);
        float endMask = Smoothstep(1, endRamp, currentLifespan);
        lineRenderer.widthMultiplier = startMask * endMask;
    }
}
