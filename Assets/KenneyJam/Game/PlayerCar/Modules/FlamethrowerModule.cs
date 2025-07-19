using System.Collections.Generic;
using UnityEngine;

namespace KenneyJam.Game.PlayerCar.Modules
{
    public class FlamethrowerModule : CarModule
    {
        public float cooldown = 4f;
        public float activationDuration = 4f;
        public float tickDamage = 0.01f;
        public Transform muzzle;

        public GameObject flamesPrefab;
        public float flamesSFXVolume = 0.6f;
        public AudioClip flamesSFXLooped;

        private BoxCollider boxCollider;

        private float currentCooldown = 0;
        private float currentDuration = 0;
        private GameObject currentFlamesVFX;
        private AudioSource currentFlamesSFX;

        public override float Cooldown => cooldown;

        public override Type GetModuleType()
        {
            return Type.Flamethrower;
        }

        void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
        }

        void Update()
        {
            if (currentCooldown > 0)
            {
                currentCooldown -= Time.deltaTime;
            }

            if (currentDuration > 0)
            {
                currentDuration -= Time.deltaTime;
                if (currentDuration <= 0)
                {
                    StopFlames();
                    currentCooldown = cooldown;
                }
                else
                {
                    TickFlames();
                }
            }
        }

        void StartFlames()
        {
            currentDuration = activationDuration;
            currentFlamesVFX = Instantiate(flamesPrefab, muzzle);
            currentFlamesSFX = SoundManager.Instance.CreatePermanentAudioSource(flamesSFXLooped, flamesSFXVolume);
        }

        void TickFlames()
        {
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
                    col.GetComponentInParent<CarController>().InflictDamage(gameObject.GetComponentInParent<CarController>(), tickDamage * Time.deltaTime, false);
                    Debug.Log("Dealt " + tickDamage * Time.deltaTime + " damage to " + col.transform.root.gameObject.name);
                    foundCars.Add(col.gameObject.name);
                }
            }
        }

        void StopFlames()
        {
            Destroy(currentFlamesVFX);
            SoundManager.Instance.FadeOutPermanentAudioSource(currentFlamesSFX);
        }

        public override void Activate()
        {
            if (currentCooldown > 0 || currentDuration > 0)
            {
                return;
            }

            StartFlames();
        }

        public override bool CanHitAnyone()
        {
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
                    return true;
                }
            }
            return false;
        }
    }
}