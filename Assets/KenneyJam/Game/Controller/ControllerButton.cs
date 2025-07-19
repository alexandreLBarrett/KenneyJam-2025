using UnityEngine;
using UnityEngine.EventSystems;

public class ControllerButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Tooltip("The GameObject that will be hidden on press and shown on release.")]
    public GameObject objectToToggle;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (objectToToggle != null)
        {
            objectToToggle.SetActive(false);
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