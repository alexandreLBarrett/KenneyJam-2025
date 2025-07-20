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
        foreach (CarModuleSlot slot in Enum.GetValues(typeof(CarModuleSlot)))
        {
            activateModuleActions.Add(InputSystem.actions.FindAction("ActivateModule" + ((int)slot + 1)));
        }
    }

    private void Start()
    {
        gamepad.OnButtonPressed.AddListener(OnButtonPressed);
    }

    private void Update()
    {
        for (int i = 0; i < activateModuleActions.Count; ++i)
        {
            if (activateModuleActions[i].triggered)
            {
                OnButtonPressed(i);
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();

        Vector3 flatForward = Camera.main.transform.forward;
        flatForward.y = 0;
        flatForward.Normalize();
        Vector3 flatRight = Camera.main.transform.right;
        flatRight.y = 0;
        flatRight.Normalize();

        Vector3 worldDir = flatForward * gamepad.JoystickY + flatRight * gamepad.JoystickX;
        
        float dx = Vector3.Dot(carController.transform.right, worldDir);
        float dy = Vector3.Dot(carController.transform.forward, worldDir);
        float steering = Mathf.Clamp(dx / 1f, -1, +1) * .8f;
        float engine = dy;
        engine = Mathf.Min(Mathf.Sqrt(1 - steering * steering), Mathf.Abs(engine)) * Mathf.Sign(engine);
        if (engine < 0) steering *= -1;
        moveValue.y += engine;
        moveValue.x += steering;

        //if (gamepad != null)
        //{
        //    moveValue.x += gamepad.JoystickX;
        //    moveValue.y += gamepad.JoystickY;
        //}
        carController.UpdateMovement(moveValue.y, moveValue.x);
    }

    void OnButtonPressed(int buttonIndex)
    {
        if (buttonIndex == 5)
            carController.Panick();
        else
            modularCar.ActivateModule((CarModuleSlot) buttonIndex);
    }
}
