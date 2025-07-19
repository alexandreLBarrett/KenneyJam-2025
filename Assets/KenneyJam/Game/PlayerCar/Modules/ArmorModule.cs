namespace KenneyJam.Game.PlayerCar.Modules
{
    public class ArmorModule : CarModule
    {
        public float armorValue = 0.3f;

        public override float Cooldown => 1;

        public override Type GetModuleType()
        {
            return Type.Armor;
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