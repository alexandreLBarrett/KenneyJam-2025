using UnityEngine;
using UnityEngine.EventSystems;

public class ControllerButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Tooltip("The GameObject that will be hidden on press and shown on release.")]
    public GameObject objectToToggle;

    public int GamepadButton = 0;
    private Gamepad Gamepad;

    public void Start()
    {
        Gamepad = GetComponentInParent<Gamepad>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (objectToToggle != null)
        {
            objectToToggle.SetActive(false);
            Gamepad.OnButtonPressed[GamepadButton].Invoke();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (objectToToggle != null)
        {
            objectToToggle.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (objectToToggle != null)
        {
            objectToToggle.SetActive(true);
        }
    }
}