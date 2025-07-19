using System;
using KenneyJam.Game.PlayerCar;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GarageDragOrigin : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public CarModule.Type type;

    private bool isDraggable = true;

    private TMP_Text text;
    private Image image;
    private Vector3 initialTransform;
    private ModularCar car;

    public void Start()
    {
        car = FindAnyObjectByType<ModularCar>();
        image = GetComponent<Image>();
        text = GetComponentInChildren<TMP_Text>();
        text.text += "\n(" + car.GetPurchaseCost(type).ToString() + "$)";
    }

    void Update()
    {
        if (CarSceneManager.Instance.playerCurrency < car.GetPurchaseCost(type))
        {
            image.color = Color.red;
            isDraggable = false;
        }
        else
        {
            image.color = Color.white;
            isDraggable = true;
        }
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
        if (!isDraggable)
        {
            return;
        }

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
