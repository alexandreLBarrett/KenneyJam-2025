using System;
using KenneyJam.Game.PlayerCar;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FrameSelectionButton : MonoBehaviour
{
    public CarFrame frame;
    private ModularCar car;

    public void Start()
    {
        car = FindAnyObjectByType<ModularCar>();
    }

    public void OnClick()
    {
        car.SetCarFrame(frame, true);
    }
}