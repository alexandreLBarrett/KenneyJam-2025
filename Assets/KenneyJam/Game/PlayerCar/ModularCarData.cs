using System.Collections.Generic;
using KenneyJam.Game.PlayerCar;
using UnityEngine;

[CreateAssetMenu(fileName = "ModularCarData", menuName = "Scriptable Objects/ModularCarData")]
public class ModularCarData : ScriptableObject
{
    public List<CarSlotDescription> partPrefabs;
}
