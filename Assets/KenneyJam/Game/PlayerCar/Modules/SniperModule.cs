﻿using UnityEngine;

namespace KenneyJam.Game.PlayerCar.Modules
{
    public class SniperModule : CarModule
    {
        public Transform muzzle;
        public GameObject laserVFXPrefab;
        public float damage = 1;
        public float cooldown = 1f;
        public AudioClip sfx;
        public float volume = .2f;
        public float knockBackScale = 6500f;

        public override float Cooldown => cooldown;

        private float currentCooldown = 0;

        public override Type GetModuleType()
        {
            return Type.Sniper;
        }

        public override void Activate()
        {
            if (currentCooldown > 0)
            {
                return;
            }
            SoundManager.Instance.PlayInstantSound(sfx, volume);
            currentCooldown = cooldown;
            Transform muzzleTransform = muzzle.transform;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, muzzleTransform.forward, out hit, 10000.0f))
            {
                if (hit.collider.CompareTag("Car"))
                {
                    hit.collider.GetComponentInParent<CarController>().InflictDamage(gameObject.GetComponentInParent<CarController>(), damage);
                }
            }

            // Self-knockback
            GetComponentInParent<Rigidbody>().AddForce(knockBackScale * -muzzle.transform.forward, ForceMode.Impulse);

            RailgunLaser laser = Instantiate(laserVFXPrefab, muzzleTransform).GetComponent<RailgunLaser>();
            laser.SetupVFXPosition(hit.collider != null ? hit.point : muzzleTransform.position + muzzleTransform.forward * 10.0f);
        }

        private void Update()
        {
            if (currentCooldown > 0)
            {
                currentCooldown -= Time.deltaTime;
            }
        }

        public override bool CanHitAnyone()
        {
            Transform muzzleTransform = muzzle.transform;
            RaycastHit hit;
            if (Physics.Raycast(muzzleTransform.position, muzzleTransform.forward, out hit, 10000.0f))
            {
                if (hit.collider.CompareTag("Car"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}