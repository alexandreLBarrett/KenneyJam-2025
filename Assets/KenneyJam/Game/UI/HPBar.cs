using System;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    private CarController controller;
    public GameObject sliderBar;
    public float offset;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponentInParent<CarController>();
        controller.onHealthChanged.AddListener((currentHP, maxHP) =>
        {
            sliderBar.transform.localScale = new Vector3(Math.Max(currentHP / maxHP, 0), 1, 1);    
        });
    }

    void Update()
    {
        transform.position = controller.transform.position + Vector3.up * offset;
    }
}
