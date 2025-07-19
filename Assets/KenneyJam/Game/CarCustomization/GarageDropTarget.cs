using System;
using KenneyJam.Game.PlayerCar;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GarageDropTarget : MonoBehaviour, IDropHandler
{
    private ModularCar car;
    private GameObject anchor;
    public CarModuleSlot targetSlot;

    public Button upgradeButton;

    public void Start()
    {
        car = FindAnyObjectByType<ModularCar>();
        
        upgradeButton.onClick.AddListener(Upgrade);
        
        CarModule module = car.GetModuleInSlot(targetSlot);
        upgradeButton.enabled = module && module.level == CarModule.Level.LVL1;
        upgradeButton.gameObject.SetActive(upgradeButton.enabled);
    }

    private void Update()
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(car.GetAnchorForSlot(targetSlot).transform.position);
        transform.position = screenPoint;
    }

    // Upgrades the module to lvl2
    public void Upgrade()
    {
        CarModule module = car.GetModuleInSlot(targetSlot);
        if (!module || module.level == CarModule.Level.LVL2) return;
        
        // TODO: Apply cost

        
        car.SetCarModule(targetSlot, new CarSlotData{ level = CarModule.Level.LVL2, type = module.GetModuleType() });
        upgradeButton.gameObject.SetActive(false);
        upgradeButton.enabled = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GarageDragOrigin origin = eventData.pointerDrag.GetComponent<GarageDragOrigin>();
        if (!origin) return;
        
        CarModule module = car.GetModuleInSlot(targetSlot);
        if (module && module.GetModuleType() == origin.type && module.level == CarModule.Level.LVL1)
        {
            // Module is identical do nothing
            return;
        }
        
        // TODO: Apply cost
        
        car.SetCarModule(targetSlot, new CarSlotData{ level = CarModule.Level.LVL1, type = origin.type });
        
        upgradeButton.gameObject.SetActive(true);
        upgradeButton.enabled = true;
    }
}
