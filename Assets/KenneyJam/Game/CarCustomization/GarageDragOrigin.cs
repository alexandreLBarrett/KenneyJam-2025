using System;
using KenneyJam.Game.PlayerCar;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GarageDragOrigin : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public CarModule.Type type;

    private Image image;
    private Vector3 initialTransform;

    public void Start()
    {
        image = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        initialTransform = transform.position;
        if (image)
        {
            image.raycastTarget = false;
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        transform.Translate(eventData.delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = initialTransform;
        if (image)
        {
            image.raycastTarget = true;
        }
    }
}
