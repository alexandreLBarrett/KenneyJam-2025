using System;
using System.Collections.Generic;
using UnityEngine;

namespace KenneyJam.Game.PlayerCar
{
    public class CarFrame : MonoBehaviour
    {
        private void Start()
        {
            // Set tag for collision purposes
            GetComponentInChildren<Collider>().gameObject.tag = "Car";
        }
        
        [Serializable]
        public struct AnchorSlot
        {
            public CarModuleSlot slot;
            public GameObject anchor;
        }
        public List<AnchorSlot> anchors;
    }
}