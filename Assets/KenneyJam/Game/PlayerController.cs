using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarController : MonoBehaviour
{
    private Gamepad gamepad;
    private InputAction moveAction;
    private CarController carController;

    void Awake()
    {
        gamepad = FindFirstObjectByType<Gamepad>();
        moveAction = InputSystem.actions.FindAction("Move");
        carController = GetComponentInChildren<CarController>();
    }

    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        if (gamepad != null)
        {
            moveValue.x += gamepad.JoystickX;
            moveValue.y += gamepad.JoystickY;
        }
        carController.UpdateMovement(moveValue.y, moveValue.x);
    }
}
