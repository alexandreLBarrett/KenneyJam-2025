using TMPro;
using UnityEngine;

public class GarageUIManager : MonoBehaviour
{
    public TMP_Text moneyText;
    private int currentMoney;

    void Start()
    {
        currentMoney = CarSceneManager.Instance.playerCurrency;
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentMoney != CarSceneManager.Instance.playerCurrency)
        {
            UpdateText();
        }
    }

    private void UpdateText()
    {
        moneyText.SetText("Money: " + CarSceneManager.Instance.playerCurrency);
    }
}
