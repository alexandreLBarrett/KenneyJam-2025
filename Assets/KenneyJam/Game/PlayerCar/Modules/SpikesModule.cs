using KenneyJam.Game.PlayerCar;
using UnityEngine;

public class SpikesModule : CarModule
{
    public float cooldown = 1f;
    public float baseDamage = 0.5f;
    public float speedScaling = 4f;
    public AudioClip spikeSound;

    private float currentCooldown = 0;
    private Rigidbody rb;
    private CarController car;

    public override Type GetModuleType()
    {
        return Type.Spikes;
    }

    public override void Activate()
    {
    }

    void Awake()
    {
        rb = GetComponentInParent<Rigidbody>();
        car = GetComponentInParent<CarController>();
    }

    void Update()
    {
        if (currentCooldown > 0)
        { 
            currentCooldown -= Time.deltaTime;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (currentCooldown > 0)
        {
            return;
        }

        if (other.CompareTag("Car") && other.transform.root.gameObject.name != transform.root.gameObject.name)
        {
            float damage = baseDamage;
            float speed = rb.linearVelocity.magnitude;
            damage += speedScaling * speed / car.stats.maxSpeed;
            other.GetComponentInParent<CarController>().InflictDamage(gameObject.GetComponentInParent<CarController>(), damage);

            SoundManager.Instance.PlayInstantSound(spikeSound);
            currentCooldown = cooldown;
        }
    }
}
