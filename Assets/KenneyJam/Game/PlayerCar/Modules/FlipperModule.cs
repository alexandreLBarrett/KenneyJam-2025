using System.Collections.Generic;
using UnityEngine;

namespace KenneyJam.Game.PlayerCar.Modules
{
    public class FlipperModule : CarModule
    {
        public float flipForce = 10000;
        public float flipDamage = 1;
        public float cooldown = 2;
        public AudioClip flipSound;

        private float currentCooldown = 0;

        private Animator animator;
        private BoxCollider boxCollider;

        public override float Cooldown => cooldown;

        void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (currentCooldown > 0)
            {
                currentCooldown -= Time.deltaTime;
            }
        }

        public override Type GetModuleType()
        {
            return Type.Flipper;
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
            foreach(Collider col in cols)
            {
                // Has Car tag, hasn't already been found and is not us.
                if (col.gameObject.CompareTag("Car") && !foundCars.Contains(col.gameObject.name) && col.transform.root.gameObject.name != transform.root.gameObject.name)
                {
                    col.GetComponentInParent<Rigidbody>().AddForce(transform.rotation * new Vector3(0, flipForce, 0), ForceMode.Impulse);
                    col.GetComponentInParent<CarController>().InflictDamage(gameObject.GetComponentInParent<CarController>(), flipDamage);
                    foundCars.Add(col.gameObject.name);
                }
            }
            
            // Effects
            animator.SetTrigger("Flip");
            SoundManager.Instance.PlayInstantSound(flipSound);
            
            currentCooldown = cooldown;
        }

        public override bool CanHitAnyone()
        {
            Collider[] cols = Physics.OverlapBox(
                transform.TransformPoint(boxCollider.center), // Convert local center to world space
                Vector3.Scale(boxCollider.size / 2.0f, transform.lossyScale), // Half extents
                transform.rotation // Use the transform's rotation, not parent's
            );
            foreach (Collider col in cols)
            {
                // Has Car tag, hasn't already been found and is not us.
                if (col.gameObject.CompareTag("Car") && col.transform.root.gameObject.name != transform.root.gameObject.name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}