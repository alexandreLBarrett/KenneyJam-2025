using System;
using System.Collections.Generic;
using UnityEngine;

namespace KenneyJam.Game.PlayerCar.Modules
{
    public class ShotgunModule : CarModule
    {
        public float cooldown = 2;
        public override float Cooldown => cooldown;

        public float knockBackScale = 1;
        public float damage;
        public GameObject muzzle;
        public AudioClip soundEffect;
        public GameObject VFX;

        private float currentCooldown = 0;

        public override Type GetModuleType()
        {
            return Type.Shotgun;
        }

        private void Update()
        {
            if (currentCooldown > 0)
            {
                currentCooldown -= Time.deltaTime;
            }
        }

        public override void Activate()
        {
            if (currentCooldown > 0)
            {
                return;
            }
            
            SoundManager.Instance.PlayInstantSound(soundEffect);
            var currentController = gameObject.GetComponentInParent<CarController>();

            if (VFX)
            {
                Instantiate(VFX, muzzle.transform);
            }
            
            foreach (var car in collidingCars)
            {
                car.Key.InflictDamage(currentController, damage);

                Vector3 knockbackDir = car.Key.gameObject.transform.position - muzzle.transform.position;
                knockbackDir.Normalize();
                car.Key.gameObject.GetComponentInParent<Rigidbody>().AddForce(knockbackDir * knockBackScale, ForceMode.Impulse);
            }

            // Self-knockback
            var rbs = GetComponentsInParent<Rigidbody>();
            var thisRB = GetComponentInParent<Rigidbody>();
            foreach(var rb in rbs)
            {
                if (rb != thisRB)
                {
                    rb.AddForce(knockBackScale * muzzle.transform.forward, ForceMode.Impulse);
                }
            }

            currentCooldown = cooldown;
        }

        public override bool CanHitAnyone()
        {
            return collidingCars.Count > 0;
        }

        private Dictionary<CarController, Collider> collidingCars = new();

        private void OnTriggerEnter(Collider other)
        {
            CarController car = other.gameObject.GetComponentInParent<CarController>();
            CarController myCar = GetComponentInParent<CarController>();
            if (car && other.gameObject.CompareTag("Car") && car != myCar)
            {
                collidingCars[car] = other;
            }
        }

        public void OnTriggerExit(Collider other)
        {
            CarController car = other.gameObject.GetComponentInParent<CarController>();
            CarController myCar = GetComponentInParent<CarController>();
            if (car && other.gameObject.CompareTag("Car") && car != myCar)
            {
                collidingCars.Remove(car);
            }
        }
    }
}