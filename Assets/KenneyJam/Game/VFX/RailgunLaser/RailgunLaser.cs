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
    private ParticleSystem ps;

    void Awake()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
        ps = GetComponent<ParticleSystem>();
        if (ps) 
        {
            ps.startLifetime = totalLifespan;
        }
    }

    void Start()
    {
        lineRenderer.useWorldSpace = true;
        lineRenderer.SetPositions(new[] { transform.position, transform.position + transform.forward * 10.0f });
        // This component is spawned with a parent for its initial position. Now that that is determined, we detach.
        transform.SetParent(null);
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
            // Laser is done firing, die.
            Destroy(gameObject);
            return;
        }

        // [0, totalLifespan] to [0, 1]
        float currentLifespan = currentTime / totalLifespan;

        // Add build up / fade out to the laser.
        float startMask = Smoothstep(0, startRamp, currentLifespan);
        float endMask = Smoothstep(1, endRamp, currentLifespan);
        lineRenderer.widthMultiplier = startMask * endMask;
    }
}
