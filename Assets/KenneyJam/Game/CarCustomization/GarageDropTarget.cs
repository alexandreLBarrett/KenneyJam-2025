using System;
using KenneyJam.Game.PlayerCar;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GarageDropTarget : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public CarModuleSlot targetSlot;
    public Button upgradeButton;
    public Button destroyButton;
    public TMP_Text moduleName;
    public Image dropTarget;

    private ModularCar car;
    private TMP_Text upgradeText;

    private string initialUpgradeText;

    public void Start()
    {
        upgradeText = GetComponentInChildren<TMP_Text>();
        initialUpgradeText = upgradeText.text;

        car = FindAnyObjectByType<ModularCar>();
        CarModule module = car.GetModuleInSlot(targetSlot);
        
        upgradeButton.onClick.AddListener(Upgrade);
        upgradeButton.enabled = module && module.level == CarModule.Level.LVL1;
        upgradeButton.gameObject.SetActive(upgradeButton.enabled);
        if (module && module.level == CarModule.Level.LVL1)
        {
            upgradeText.text = initialUpgradeText + " (" + car.GetUpgradeCost(module.GetModuleType()).ToString() + "$)";
        }

        destroyButton.onClick.AddListener(Destroy);
        destroyButton.enabled = module != null;
        destroyButton.gameObject.SetActive(destroyButton.enabled);

        dropTarget = GetComponent<Image>();
        dropTarget.color = Color.grey;
    }

    void Update()
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(car.GetAnchorForSlot(targetSlot).transform.position);
        transform.position = screenPoint;

        CarModule module = car.GetModuleInSlot(targetSlot);
        if (module)
        {
            UpdateName(module.level, module.GetModuleType());
        }
        else
        {
            moduleName.text = "";
        }
        
        if (!module || module.level == CarModule.Level.LVL2)
        {
            return;
        }
        
        if (CarSceneManager.Instance.playerCurrency < car.GetUpgradeCost(module.GetModuleType()))
        {
            upgradeButton.image.color = Color.red;
        }
        else
        {
            upgradeButton.image.color = Color.mediumSeaGreen;
        }
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

    void UpdateName(CarModule.Level level, CarModule.Type type)
    {
        string lvlLabel = level.ToString();
        string moduleLabel = type.ToString();
        moduleName.text = moduleLabel + "\n" + lvlLabel;
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
        upgradeText.text = initialUpgradeText + " (" + car.GetUpgradeCost(origin.type).ToString() + "$)";

        destroyButton.enabled = true;
        destroyButton.gameObject.SetActive(destroyButton.enabled);
        
        dropTarget.color = Color.grey;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.dragging)
        {
            dropTarget.color = Color.chartreuse;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.dragging)
        {
            dropTarget.color = Color.grey;
        }
    }
}
