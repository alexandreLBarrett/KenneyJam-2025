using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class Joystick : MonoBehaviour
{
    public float maxDistanceToOrigin = 1;
    private Camera mainCamera;
    private Plane dragPlane;
    private Vector3 offset;
    private Vector3 initialPosition;
    private bool isDragging = false;

    private Gamepad Gamepad;

    private void Awake()
    {
        mainCamera = Camera.main;
        initialPosition = transform.position;
    }

    private void Start()
    {
        Gamepad = GetComponentInParent<Gamepad>();
    }

    private Vector2 GetInputPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.phase.ReadValue() != TouchPhase.None)
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }

        if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();    
        }

        return Vector2.zero;
    }
    
    private void Update()
    {
        //bool pressStarted = false;
        bool isPressed = false;
        bool pressReleased = false;
        if (Touchscreen.current != null)
        {
            isPressed = Touchscreen.current.primaryTouch.phase.ReadValue() == TouchPhase.Moved;
            pressReleased = Touchscreen.current.primaryTouch.phase.ReadValue() == TouchPhase.Ended;
        }
        
        if (Mouse.current != null)
        {
            isPressed = isPressed || Mouse.current.leftButton.isPressed;
            pressReleased = pressReleased || Mouse.current.leftButton.wasReleasedThisFrame;
        }
        
        if (!isDragging && isPressed)
        {
            TryStartDrag();
        }
        else if (isPressed && isDragging)
        {
            ContinueDrag();
        }
        else if (pressReleased && isDragging)
        {
            isDragging = false;
            transform.position = initialPosition;
            Gamepad.OnJoystickMoved.Invoke(0, 0);
        }
    }
    
    private void TryStartDrag()
    {
        Ray ray = mainCamera.ScreenPointToRay(GetInputPosition());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && hit.transform == transform)
        {
            dragPlane = new Plane(transform.up, transform.position);
            float enter;
            if (dragPlane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                offset = transform.position - hitPoint;
                isDragging = true;
            }
        }
    }

    private void ContinueDrag()
    {
        Ray ray = mainCamera.ScreenPointToRay(GetInputPosition());
        float enter;
        if (dragPlane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 positionUnclamped = hitPoint + offset;
            Vector3 linearOffset = positionUnclamped - initialPosition;
            Vector3 finalOffset = Mathf.Min(linearOffset.magnitude, maxDistanceToOrigin) * linearOffset.normalized;
            transform.position = finalOffset + initialPosition;
            Gamepad.OnJoystickMoved.Invoke(Vector3.Dot(finalOffset, transform.forward) / maxDistanceToOrigin, -Vector3.Dot(finalOffset, transform.right) / maxDistanceToOrigin);
        }
    }
}