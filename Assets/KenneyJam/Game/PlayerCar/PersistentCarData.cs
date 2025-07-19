using System.Collections.Generic;
using UnityEngine;

namespace KenneyJam.Game.PlayerCar
{
    public struct CarSlotData
    {
        public CarModule.Level level;
        public CarModule.Type type;
    }

    public class PersistentCarData : MonoBehaviour
    {
        public Dictionary<CarModuleSlot, CarSlotData> moduleSlotData = new();
        public CarFrame carFrame;

        public void SaveModuleStates(Dictionary<CarModuleSlot, CarModule> inModuleDescriptions)
        {
            moduleSlotData.Clear();
            foreach (var desc in inModuleDescriptions)
            {
                moduleSlotData.Add(desc.Key, new CarSlotData
                {
                    level = desc.Value.level, 
                    type = desc.Value.GetModuleType()
                });    
            }
        }

        public static PersistentCarData GetPersistentCarData()
        {
            PersistentCarData modularCars = FindAnyObjectByType<PersistentCarData>();
            if (modularCars) return modularCars;
        
            GameObject instance = Instantiate(new GameObject());
            DontDestroyOnLoad(instance);
            return instance.AddComponent<PersistentCarData>();
        }
    }
}