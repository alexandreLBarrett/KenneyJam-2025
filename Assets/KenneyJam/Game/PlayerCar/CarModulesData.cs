using System;
using System.Linq;
using KenneyJam.Game.PlayerCar;
using UnityEngine;

[CreateAssetMenu(fileName = "CarModulesData", menuName = "Scriptable Objects/CarModulesData")]
public class CarModulesData : ScriptableObject
{
    [Serializable]
    public class ModuleInfo
    {
        public CarModule.Type type;
        public CarModule lvl1Module;
        public CarModule lvl2Module;
        public int purchaseCost;
        public int upgradeCost;
    }

    public ModuleInfo[] moduleTypes;

    public ModuleInfo GetModuleInfo(CarModule.Type type)
    {
        return moduleTypes.First(x => x.type == type);
    }
}
