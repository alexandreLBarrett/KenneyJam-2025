using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GarageUIManager : MonoBehaviour
{
    public TMP_Text moneyText;
    public Button weaponsButton;
    public Button upgradesButton;
    public GameObject weaponsSelectionPanel;
    public GameObject upgradesSelectionPanel;

    enum MenuSelected
    {
        Weapons = 0,
        Upgrades = 1,
    }

    private MenuSelected menuSelected;

    private int currentMoney;

    void Start()
    {
        ChangeMenuSelection(MenuSelected.Weapons);
        weaponsButton.onClick.AddListener(() => ChangeMenuSelection(MenuSelected.Weapons));
        upgradesButton.onClick.AddListener(() => ChangeMenuSelection(MenuSelected.Upgrades));
        currentMoney = CarSceneManager.Instance.playerCurrency;
        UpdateText();
        
        gameObject.GetComponent<Canvas>().enabled = false;
        StartCoroutine(ShowMenu());
    }

    private IEnumerator ShowMenu()
    {
        yield return new WaitForSeconds(1);
        gameObject.GetComponent<Canvas>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentMoney != CarSceneManager.Instance.playerCurrency)
        {
            UpdateText();
        }
    }

    private void ChangeMenuSelection(MenuSelected newSelection)
    {
        menuSelected = newSelection;
        weaponsSelectionPanel.SetActive(menuSelected == MenuSelected.Weapons);
        upgradesSelectionPanel.SetActive(menuSelected == MenuSelected.Upgrades);
        weaponsButton.image.color = menuSelected == MenuSelected.Weapons ? Color.mediumSeaGreen : Color.white;
        upgradesButton.image.color = menuSelected == MenuSelected.Upgrades ? Color.mediumSeaGreen : Color.white;
    }

    private void UpdateText()
    {
        moneyText.SetText("Money: " + CarSceneManager.Instance.playerCurrency + "$");
    }

    public void Play()
    {
        gameObject.SetActive(false);
        CarSceneManager.Instance.LoadGame();
    }
}
