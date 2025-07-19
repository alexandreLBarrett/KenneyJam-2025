using System.Collections.Generic;
using UnityEngine;

namespace KenneyJam.Game.PlayerCar
{
    public class CarFrame : MonoBehaviour
    {
        public List<CarPartPair> anchors;

        private void Start()
        {
            // Set tag for collision purposes
            GetComponentInChildren<Collider>().gameObject.tag = "Car";
        }
    }
}