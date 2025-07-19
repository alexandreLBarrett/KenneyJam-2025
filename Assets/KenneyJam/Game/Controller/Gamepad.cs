using KenneyJam.Game.PlayerCar;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gamepad : MonoBehaviour
{
    public UnityEvent<int> OnButtonPressed;
    public UnityEvent<float, float> OnJoystickMoved = new UnityEvent<float, float>();

    private float _JoystickX = 0, _JoystickY = 0;
    public float JoystickX { get => _JoystickX; }
    public float JoystickY { get => _JoystickY; }

    Gamepad()
    {
        OnButtonPressed = new();
        OnJoystickMoved.AddListener(OnJosystickMovedCallback);
    }

    void OnJosystickMovedCallback(float x, float y)
    {
        _JoystickX = x;
        _JoystickY = y;
    }
}
