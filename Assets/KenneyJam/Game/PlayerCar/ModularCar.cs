using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;

namespace KenneyJam.Game.PlayerCar
{
    public enum CarModuleSlot
    {
        Front = 0,
        FrontLeft = 1,
        FrontRight = 2,
        BackLeft = 3,
        BackRight = 4,
    }
    
    [Serializable]
    public class CarSlotDescription
    {
        public CarModuleSlot moduleSlot;
        public CarModule.Type type;
        public CarModule.Level level;
    }
    
    public class ModularCar : MonoBehaviour
    {
        public static ModularCar FindCarInScene()
        {
            ModularCar[] modularCars = FindObjectsByType<ModularCar>(FindObjectsSortMode.None);
            return modularCars.Length == 0 ? null : modularCars[0];
        }
        
        [SerializeField]
        private ModularCarData defaults;

        public CarModulesData modulesDB;
        
        public CarFrame carFrame;

        // Runtime data on the car itself
        private Dictionary<CarModuleSlot, CarModule> modules = new();

        private void Start()
        {
            GameObject car = Instantiate(carFrame.gameObject, transform);
            
            SpawnModules();
        }
        
        private void SpawnCarPart(CarSlotDescription part)
        {
            var info = modulesDB.GetModuleInfo(part.type);

            CarModule moduleToAttach = part.level == CarModule.Level.LVL1 ? info.lvl1Module : info.lvl2Module;

            var slotAnchor = GetAnchorForSlot(part.moduleSlot);
            
            GameObject moduleInstance = Instantiate(moduleToAttach.gameObject, slotAnchor.transform);
            modules.Add(part.moduleSlot, moduleInstance.GetComponent<CarModule>());
        }
        
        private void SpawnModules()
        {
            if (!defaults) return;
            foreach (var desc in defaults.partPrefabs)
            {
                SpawnCarPart(desc);    
            }
        }

        public GameObject GetAnchorForSlot(CarModuleSlot slot)
        {
            return carFrame.anchors.Find(x => x.slot == slot).anchor;
        }

        public void SetCarModule(CarSlotDescription desc)
        {
            if (modules.TryGetValue(desc.moduleSlot, out var module))
            {
                Destroy(module.gameObject);
                modules.Remove(desc.moduleSlot);
            }
            
            SpawnCarPart(desc);
        }

        public CarModule GetModuleInSlot(CarModuleSlot slot)
        {
            return modules.GetValueOrDefault(slot);
        }

        public void ActivateModule(CarModuleSlot moduleSlot)
        {
            CarModule mod = modules[moduleSlot];
            if (mod != null)
            {
                mod.Activate();
            }
        }
    }
}