using UnityEngine;

namespace KenneyJam.Game.PlayerCar
{
    public abstract class CarModule : MonoBehaviour
    {
        public enum Type
        {
            Flamethrower,
            Shotgun,
            Sniper,
            Flipper,
            Bumper,
            Spikes,
            Armor,
        }

        public enum Level
        {
            LVL1,
            LVL2
        }
        
        public Level level;

        public abstract bool CanHitAnyone();
        public abstract Type GetModuleType();
        public abstract void Activate();

        public abstract float Cooldown { get; }
    }
}