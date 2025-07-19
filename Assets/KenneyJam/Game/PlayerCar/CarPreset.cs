using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KenneyJam.Game.PlayerCar
{
    [CreateAssetMenu(fileName = "New CarDataDefaults", menuName = "Scriptable Objects/CarDataDefaults")]
    public class CarDataPreset : ScriptableObject
    {
        public List<CarSlotDescription> moduleDesc;
        public CarFrame frame;

        public Dictionary<CarModuleSlot, CarSlotData> GetModuleSlotData()
        {
            return moduleDesc.ToDictionary(
                def => def.moduleSlot, 
                def => new CarSlotData { level = def.level, type = def.type }
            );
        }
    }
}