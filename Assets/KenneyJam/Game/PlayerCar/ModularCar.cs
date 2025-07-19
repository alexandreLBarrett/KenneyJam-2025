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
        public CarDataPreset preset;

        public CarModulesData modulesDB;
        
        // Runtime data on the car itself
        private Dictionary<CarModuleSlot, CarModule> modules = new();
        private CarFrame carFrame;
        
        private CarFrame currentFrameInsance;
        private PersistentCarData persistentData;

        public bool isPlayer;

        private void Start()
        {
            bool isInitialized = false;
            if (isPlayer)
            {
                persistentData = FindAnyObjectByType<PersistentCarData>();
                if (persistentData)
                {
                    SetCarPreset(persistentData.carFrame, persistentData.moduleSlotData);
                    isInitialized = true;
                }
                else
                {
                    // Initialize persistent object
                    persistentData = PersistentCarData.GetPersistentCarData();
                }
            }
            
            if (!isInitialized && preset)
            {
                SetCarPreset(preset.frame, preset.GetModuleSlotData());
            }
        }
        
        private void SpawnCarPart(CarModuleSlot slot, CarSlotData slotData)
        {
            var info = modulesDB.GetModuleInfo(slotData.type);

            CarModule moduleToAttach = slotData.level == CarModule.Level.LVL1 ? info.lvl1Module : info.lvl2Module;

            var slotAnchor = GetAnchorForSlot(slot);
            
            GameObject moduleInstance = Instantiate(moduleToAttach.gameObject, slotAnchor.transform);
            modules.Add(slot, moduleInstance.GetComponent<CarModule>());
        }

        private void ResetModules()
        {
            foreach (var module in modules)
            {
                Destroy(module.Value.gameObject);
            }
            modules.Clear();
        }
        
        private void SpawnModules(Dictionary<CarModuleSlot, CarSlotData> inModules)
        {
            ResetModules();
            foreach (var desc in inModules)
            {
                SpawnCarPart(desc.Key, desc.Value);    
            }
        }

        public GameObject GetAnchorForSlot(CarModuleSlot slot)
        {
            return currentFrameInsance.anchors.Find(x => x.slot == slot).anchor;
        }

        public void SetCarModule(CarModuleSlot slot, CarSlotData data)
        {
            if (modules.TryGetValue(slot, out var module))
            {
                Destroy(module.gameObject);
                modules.Remove(slot);
            }
            
            SpawnCarPart(slot, new CarSlotData{ level = data.level, type = data.type });
        }

        public void SetCarPreset(CarFrame frame, Dictionary<CarModuleSlot, CarSlotData> modules)
        {
            SetCarFrame(frame, false);
            SpawnModules(modules);
        }
        
        public void SetCarFrame(CarFrame frame, bool respawnModules)
        {
            if (currentFrameInsance)
            {
                Destroy(currentFrameInsance.gameObject);    
            }

            carFrame = frame;
            var newFrame = Instantiate(carFrame.gameObject, transform);
            currentFrameInsance = newFrame.GetComponent<CarFrame>();

            if (respawnModules)
            {
                Dictionary<CarModuleSlot, CarSlotData> slots = modules.ToDictionary(
                    x => x.Key,
                    x => new CarSlotData { type = x.Value.GetModuleType(), level = x.Value.level }
                );
                SpawnModules(slots);    
            }
        }

        public CarModule GetModuleInSlot(CarModuleSlot slot)
        {
            return modules.GetValueOrDefault(slot);
        }

        public void ActivateModule(CarModuleSlot moduleSlot)
        {
            if (modules.TryGetValue(moduleSlot, out var mod))
            {
                mod.Activate();
            }
        }

        private void OnDestroy()
        {
            if (isPlayer)
            {
                persistentData.SaveModuleStates(modules);
                persistentData.carFrame = carFrame;
            }
        }
    }
}