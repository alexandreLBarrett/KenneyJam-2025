using UnityEngine;

namespace KenneyJam.Game.PlayerCar.Modules
{
    public class SniperModule : CarModule
    {
        public Transform muzzle;

        public override Type GetModuleType()
        {
            return Type.Sniper;
        }

        public override void Activate()
        {
            //Physics.Raycast()
        }
    }
}