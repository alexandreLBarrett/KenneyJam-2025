using UnityEngine;
using UnityEngine.InputSystem;
public class Joystick : MonoBehaviour
{
    public float maxDistanceToOrigin = 1;
    private Camera mainCamera;
    private Plane dragPlane;
    private Vector3 offset;
    private Vector3 initialPosition;
    private bool isDragging = false;

    private void Awake()
    {
        mainCamera = Camera.main;
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryStartDrag();
        }
        else if (Mouse.current.leftButton.isPressed && isDragging)
        {
            ContinueDrag();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
        {
            isDragging = false;
            transform.position = initialPosition;
        }
    }

    private void TryStartDrag()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && hit.transform == transform)
        {
            dragPlane = new Plane(Vector3.up, transform.position);
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
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        float enter;
        if (dragPlane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 positionUnclamped = hitPoint + offset;
            Vector3 linearOffset = positionUnclamped - initialPosition;
            transform.position = Mathf.Min(linearOffset.magnitude, maxDistanceToOrigin) * linearOffset.normalized + initialPosition;
        }
    }
}