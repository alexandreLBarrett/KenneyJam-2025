namespace KenneyJam.Game.PlayerCar.Modules
{
    public class ShotgunModule : CarModule
    {
        public override float Cooldown => 1;

        public override Type GetModuleType()
        {
            return Type.Shotgun;
        }

        public override void Activate()
        {
            
        }

        public override bool CanHitAnyone()
        {
            return false;
        }
    }
}