using UnityEngine;

namespace KenneyJam.Game.PlayerCar.Modules
{
    public class SniperModule : CarModule
    {
        public Transform muzzle;
        public GameObject laserVFXPrefab;

        public override Type GetModuleType()
        {
            return Type.Sniper;
        }

        public override void Activate()
        {
            Transform muzzleTransform = muzzle.transform;
            RailgunLaser laser = Instantiate(laserVFXPrefab, muzzleTransform).GetComponent<RailgunLaser>();
            if (Physics.Raycast(muzzleTransform.position, muzzleTransform.forward, out RaycastHit hit, 10000.0f))
            {
                Debug.Log("An hit was registered on " + hit.collider.gameObject.name + ".");
                if (hit.collider.CompareTag("Car"))
                {
                    Debug.Log("An actual hit was registered on " + hit.collider.gameObject.name + "!");
                }
            }
        }
    }
}