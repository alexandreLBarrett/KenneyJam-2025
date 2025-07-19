using System;
using System.Collections.Generic;
using UnityEngine;
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
    public class CarPartPair
    {
        public CarModuleSlot moduleSlot;
        public GameObject gameObject;
    }
    
    public class ModularCar : MonoBehaviour
    {
        public ModularCarData data;
        public GameObject carFramePrefab;

        private Dictionary<CarModuleSlot, CarModule> modules = new();
        
        private void Start()
        {
            Debug.Assert(data);

            GameObject car = Instantiate(carFramePrefab, transform);
            
            CarFrame frame = car.GetComponent<CarFrame>();
            Debug.Assert(frame != null);

            SpawnModules(frame.anchors);
        }
        
        private void SpawnCarPart(CarModuleSlot moduleSlot, GameObject anchor)
        {
            int index = data.partPrefabs.FindIndex(x => x.moduleSlot == moduleSlot);
            if (index == -1) return;
            
            GameObject moduleInstance = Instantiate(data.partPrefabs[index].gameObject, anchor.transform);
            CarModule module = moduleInstance.GetComponent<CarModule>();
            if (!module) return;

            modules.Add(moduleSlot, module);
        }
        
        private void SpawnModules(List<CarPartPair> anchors)
        {
            foreach (var anchorPair in anchors)
            {
                SpawnCarPart(anchorPair.moduleSlot, anchorPair.gameObject);
            }
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