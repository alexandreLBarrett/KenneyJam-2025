namespace KenneyJam.Game.PlayerCar.Modules
{
    public class ArmorModule : CarModule
    {
        public float armorValue = 0.3f;

        public override Type GetModuleType()
        {
            return Type.Armor;
        }

        public override void Activate()
        {
        }
    }
}