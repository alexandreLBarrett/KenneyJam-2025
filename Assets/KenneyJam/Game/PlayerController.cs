using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarController : MonoBehaviour
{
    private InputAction moveAction;
    private CarController carController;

    void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        carController = GetComponent<CarController>();
    }

    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        carController.UpdateMovement(moveValue.y, moveValue.x);
    }
}
