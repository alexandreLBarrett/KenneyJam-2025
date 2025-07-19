using UnityEngine;

namespace KenneyJam.Game.PlayerCar.Modules
{
    public class SniperModule : CarModule
    {
        public Transform muzzle;
        public GameObject laserVFXPrefab;
        public float damage = 1;

        public override Type GetModuleType()
        {
            return Type.Sniper;
        }

        public override void Activate()
        {
            Transform muzzleTransform = muzzle.transform;
            RaycastHit hit;
            if (Physics.Raycast(muzzleTransform.position, muzzleTransform.forward, out hit, 10000.0f))
            {
                Debug.Log("An hit was registered on " + hit.collider.gameObject.name + ".");
                if (hit.collider.CompareTag("Car"))
                {
                    hit.collider.GetComponentInParent<CarController>().InflictDamage(gameObject.GetComponentInParent<CarController>(), damage);
                }
            }
            RailgunLaser laser = Instantiate(laserVFXPrefab, muzzleTransform).GetComponent<RailgunLaser>();
            laser.SetupVFXPosition(hit.collider != null ? hit.point : muzzleTransform.position + muzzleTransform.forward * 10.0f);
        }
    }
}