using KenneyJam.Game.PlayerCar;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarController : MonoBehaviour
{
    private Gamepad gamepad;
    private InputAction moveAction;
    private List<InputAction> activateModuleActions;
    private CarController carController;
    private ModularCar modularCar;

    void Awake()
    {
        gamepad = FindFirstObjectByType<Gamepad>();
        moveAction = InputSystem.actions.FindAction("Move");
        carController = GetComponentInChildren<CarController>();
        modularCar = GetComponentInChildren<ModularCar>();
        activateModuleActions = new();
        int i = 1;
        foreach (CarModuleSlot slot in Enum.GetValues(typeof(CarModuleSlot)))
        {
            activateModuleActions.Add(InputSystem.actions.FindAction("ActivateModule" + i++));
        }
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

        if (activateModuleActions[0].triggered)
        {
            modularCar.ActivateModule(0);
        }
    }
}
