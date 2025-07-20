using System;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    private CarController controller;
    public GameObject sliderBar;
    public float offset;
    
    void Awake()
    {
        controller = GetComponentInParent<CarController>();
        if (sliderBar != null)
        {
            controller.onHealthChanged.AddListener((currentHP, maxHP) =>
            {
                sliderBar.transform.localScale = new Vector3(Math.Max(currentHP / maxHP, 0), 1, 1);    
            });
        }
    }

    void Update()
    {
        transform.position = controller.transform.position + Vector3.up * offset;
    }
}
