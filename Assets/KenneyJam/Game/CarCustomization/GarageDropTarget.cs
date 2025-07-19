using KenneyJam.Game.PlayerCar;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GarageDropTarget : MonoBehaviour, IDropHandler
{
    public CarModuleSlot targetSlot;
    public Button upgradeButton;
    public Button destroyButton;

    private ModularCar car;

    public void Start()
    {
        car = FindAnyObjectByType<ModularCar>();
        CarModule module = car.GetModuleInSlot(targetSlot);
        
        upgradeButton.onClick.AddListener(Upgrade);
        upgradeButton.enabled = module && module.level == CarModule.Level.LVL1;
        upgradeButton.gameObject.SetActive(upgradeButton.enabled);

        destroyButton.onClick.AddListener(Destroy);
        destroyButton.enabled = module != null;
        destroyButton.gameObject.SetActive(destroyButton.enabled);
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

        int upgradeCost = car.GetUpgradeCost(module.GetModuleType());
        if (CarSceneManager.Instance.playerCurrency < upgradeCost)
        {
            // Too broke, do nothing.
            // TODO: UI indicator
            return;
        }

        CarSceneManager.Instance.playerCurrency -= upgradeCost;

        car.SetCarModule(targetSlot, new CarSlotData{ level = CarModule.Level.LVL2, type = module.GetModuleType() });

        upgradeButton.enabled = false;
        upgradeButton.gameObject.SetActive(upgradeButton.enabled);
    }

    // Destroy the module, don't sell because albin said so :(
    public void Destroy()
    {
        car.DestroyCarModule(targetSlot);

        upgradeButton.enabled = false;
        upgradeButton.gameObject.SetActive(upgradeButton.enabled);

        destroyButton.enabled = false;
        destroyButton.gameObject.SetActive(destroyButton.enabled);
    }

    public void OnDrop(PointerEventData eventData)
    {
        GarageDragOrigin origin = eventData.pointerDrag.GetComponent<GarageDragOrigin>();
        if (!origin) return;
        
        CarModule module = car.GetModuleInSlot(targetSlot);
        if (module && module.GetModuleType() == origin.type && module.level == CarModule.Level.LVL1)
        {
            // Module is identical, do nothing
            return;
        }

        int purchaseCost = car.GetPurchaseCost(origin.type);
        if (CarSceneManager.Instance.playerCurrency < purchaseCost)
        {
            // Too broke, do nothing.
            // TODO: UI indicator
            return;
        }

        CarSceneManager.Instance.playerCurrency -= purchaseCost;

        car.SetCarModule(targetSlot, new CarSlotData{ level = CarModule.Level.LVL1, type = origin.type });
        
        upgradeButton.enabled = true;
        upgradeButton.gameObject.SetActive(upgradeButton.enabled);

        destroyButton.enabled = true;
        destroyButton.gameObject.SetActive(destroyButton.enabled);
    }
}
