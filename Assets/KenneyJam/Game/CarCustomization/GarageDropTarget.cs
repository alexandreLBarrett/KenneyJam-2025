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
        car = ModularCar.FindCarInScene();
        anchor = car.GetAnchorForSlot(targetSlot);
        
        upgradeButton.onClick.AddListener(Upgrade);
        
        CarModule module = car.GetModuleInSlot(targetSlot);
        upgradeButton.enabled = module && module.level == CarModule.Level.LVL1;
        upgradeButton.gameObject.SetActive(upgradeButton.enabled);
    }

    private void Update()
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(anchor.transform.position);
        transform.position = screenPoint;
    }

    // Upgrades the module to lvl2
    public void Upgrade()
    {
        CarModule module = car.GetModuleInSlot(targetSlot);
        if (!module || module.level == CarModule.Level.LVL2) return;
        
        // TODO: Apply cost

        var desc = new CarSlotDescription
        {
            type = module.GetModuleType(),
            level = CarModule.Level.LVL2,
            moduleSlot = targetSlot,
        };
        
        car.SetCarModule(desc);
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
        
        var desc = new CarSlotDescription
        {
            type = origin.type,
            level = CarModule.Level.LVL1,
            moduleSlot = targetSlot,
        };
        
        car.SetCarModule(desc);
        
        upgradeButton.gameObject.SetActive(true);
        upgradeButton.enabled = true;
    }
}
