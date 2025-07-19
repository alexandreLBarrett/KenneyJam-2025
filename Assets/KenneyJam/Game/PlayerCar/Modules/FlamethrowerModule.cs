namespace KenneyJam.Game.PlayerCar.Modules
{
    public class FlamethrowerModule : CarModule
    {
        public override float Cooldown => 1;

        public override Type GetModuleType()
        {
            return Type.Flamethrower;
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