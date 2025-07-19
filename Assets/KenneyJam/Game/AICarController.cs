using KenneyJam.Game.PlayerCar;
using System.Linq;
using UnityEngine;

public class AICarController : MonoBehaviour
{
    private CarController controller;
    private ModularCar modularCar;

    public float steeringControl = 1f;
    public float engineControl = 2f;
    public float gameRingRadius = 3f;
    public float maxSteering = .7f;

    enum Mood
    {
        Fleeing,
        Ramming,
        Skeeing,
        Picking,
        _Count,
        AvoidingEdge,
    }
    
    private Vector3 pickingTarget;
    private Mood currentMood = Mood.Skeeing;
    private float nextMoodChange = 0;

    private Vector3 _Debug;

    private float[] moduleCooldowns = new float[5];
    private float[] moduleRawCooldown = new float[5];

    private bool isTeabaging = Random.Range(0,2)==0;

    void Start()
    {
        controller = GetComponent<CarController>();
        modularCar = GetComponent<ModularCar>();
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = currentMood switch {
            Mood.Fleeing => Color.yellow,
            Mood.Ramming => Color.orange,
            Mood.Skeeing => Color.red,
            Mood.Picking => Color.green,
            Mood.AvoidingEdge => Color.blue,
        };
        Gizmos.DrawSphere(transform.position + Vector3.up, .1f);
        if (currentMood == Mood.Picking)
            Gizmos.DrawLine(transform.position, pickingTarget);
    }

    void FixedUpdate()
    {
        if (controller.currentHealth <= 0) return;

        CarController? target = GetTarget();
        if (target == null)
        {
            if (isTeabaging)
                controller.UpdateMovement(0, 1);
            return;
        }
        var offset = target.transform.position - transform.position;

        nextMoodChange -= Time.fixedDeltaTime;
        if (nextMoodChange <= 0)
        {
            nextMoodChange = Random.Range(1f, 5f);
            currentMood = (Mood)Random.Range(0, (int)Mood._Count);
            if (currentMood == Mood.Picking)
            {
                pickingTarget = CarController.RandomVector3();
                pickingTarget.y = 0;
                pickingTarget *= gameRingRadius * .9f;
            }
        }

        if (Vector3.Distance(transform.position, Vector3.zero) > gameRingRadius)
        {
            currentMood = Mood.AvoidingEdge;
            nextMoodChange = Random.Range(.5f, 1f);
        }

        switch (currentMood)
        {
            case Mood.Fleeing:
                {
                    float dx = Vector3.Dot(transform.right, offset);
                    float dy = Vector3.Dot(transform.forward, offset);
                    float steering = -Mathf.Clamp(dx / steeringControl, -1, +1);
                    float engine = dy < .3f ? -1f : 1f;
                    engine = Mathf.Min(Mathf.Sqrt(1 - steering * steering), Mathf.Abs(engine)) * Mathf.Sign(engine);
                    if (engine < 0) steering *= -1;
                    controller.UpdateMovement(engine, steering);
                    _Debug = new(dx, engine, steering);
                    break;
                }
            case Mood.Picking:
                offset = pickingTarget - transform.position;
                if (offset.magnitude < .1f)
                {
                    while (currentMood == Mood.Picking)
                        currentMood = (Mood)Random.Range(0, (int)Mood._Count);
                }
                goto case Mood.Ramming;
            case Mood.Ramming:
                {
                    float dx = Vector3.Dot(transform.right, offset);
                    float dy = Vector3.Dot(transform.forward, offset);
                    float steering = Mathf.Clamp(dx / steeringControl, -1, +1) * maxSteering;
                    float engine = dy > 0 ? 1f : -1f;
                    engine = Mathf.Min(Mathf.Sqrt(1 - steering * steering), Mathf.Abs(engine)) * Mathf.Sign(engine);
                    if (engine < 0) steering *= -1;
                    controller.UpdateMovement(engine, steering);
                    _Debug = new(dx, engine, steering);
                    break;
                }
            case Mood.Skeeing:
                {
                    float dx = Vector3.Dot(transform.right, offset);
                    float dy = Vector3.Dot(transform.forward, offset);
                    float steering = Mathf.Clamp(dx / steeringControl, -1, +1);
                    float engine = offset.magnitude - engineControl;
                    engine = Mathf.Min(Mathf.Sqrt(1 - steering * steering), Mathf.Abs(engine)) * Mathf.Sign(engine);
                    if (engine < 0) steering *= -1;
                    controller.UpdateMovement(engine, steering);
                    _Debug = new(dx, engine, steering);
                    break;
                }
            case Mood.AvoidingEdge:
                {
                    offset = transform.position;
                    offset.y = 0;
                    offset = offset.normalized * gameRingRadius;
                    float dx = Vector3.Dot(transform.right, offset);
                    float dy = Vector3.Dot(transform.forward, offset);
                    float steering = -Mathf.Clamp(dx / steeringControl, -1, +1);
                    float engine = dy > 0 ? -1f : 1f;
                    steering *= Mathf.Sign(engine);
                    Vector2 v = new(steering, engine);
                    v.Normalize();
                    controller.UpdateMovement(v.y, v.x);
                    _Debug = new(dx, dy, steering);
                    break;
                }
        }

        for (CarModuleSlot slot = 0; slot < (CarModuleSlot)5; slot++)
        {
            CarModule module = modularCar.GetModuleInSlot(slot);
            if (module == null) continue;
            if (moduleRawCooldown[(int)slot] > 0)
            {
                moduleRawCooldown[(int)slot] -= Time.deltaTime;
                continue;
            }

            float cooldown = moduleCooldowns[(int)slot];
            if (cooldown < 0)
            {
                if (module.CanHitAnyone())
                    moduleCooldowns[(int)slot] = Random.Range(.1f, .25f);
                continue;
            }
            cooldown -= Time.fixedDeltaTime;
            moduleCooldowns[(int)slot] = cooldown;
            if (cooldown < 0)
            {
                module.Activate();
                moduleRawCooldown[(int)slot] = module.Cooldown;
            }
        }
    }

    CarController? GetTarget()
    {
        CarController[] controllers = FindObjectsByType<CarController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        try { 
        return controllers.Where(c => c != controller && c.currentHealth > 0)
            .OrderBy(c => Vector3.Distance(c.transform.position, transform.position))
            .First();
        }
        catch (System.InvalidOperationException)
        {
            return null;
        }
    }
}
