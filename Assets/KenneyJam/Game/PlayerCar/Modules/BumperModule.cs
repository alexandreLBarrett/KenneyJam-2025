using System.Collections.Generic;
using UnityEngine;

namespace KenneyJam.Game.PlayerCar.Modules
{
    public class BumperModule : CarModule
    {
        public float force = 100;
        public float damage = 6;
        public float cooldown = 3;

        public AudioClip bumpSound;

        private float currentCooldown = 0;
        private BoxCollider boxCollider;
        private Animator animator;

        public override Type GetModuleType()
        {
            return Type.Bumper;
        }

        void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
            animator = GetComponent<Animator>();
        }

        void Update()
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

            // Detection logic
            List<string> foundCars = new();
            Collider[] cols = Physics.OverlapBox(
                transform.TransformPoint(boxCollider.center), // Convert local center to world space
                Vector3.Scale(boxCollider.size / 2.0f, transform.lossyScale), // Half extents
                transform.rotation // Use the transform's rotation, not parent's
            );
            foreach (Collider col in cols)
            {
                // Has Car tag, hasn't already been found and is not us.
                if (col.gameObject.CompareTag("Car") && !foundCars.Contains(col.gameObject.name) && col.transform.root.gameObject.name != transform.root.gameObject.name)
                {
                    col.GetComponentInParent<Rigidbody>().AddForce(transform.rotation * new Vector3(force, 0.05f, 0), ForceMode.Impulse);
                    col.GetComponentInParent<CarController>().InflictDamage(gameObject.GetComponentInParent<CarController>(), damage);
                    foundCars.Add(col.gameObject.name);
                }
            }

            // Effects
            animator.SetTrigger("Bump");
            SoundManager.Instance.PlayInstantSound(bumpSound);

            currentCooldown = cooldown;
        }
    }
}