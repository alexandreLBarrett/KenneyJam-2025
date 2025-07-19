using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUIManager : MonoBehaviour
{
    public GameObject healthPoints;
    public TextMeshProUGUI victoriesText;
    public CarController controller;
    public GameObject livesRemaining1;
    public GameObject livesRemaining2;
    public GameObject livesRemaining3;

    void Start()
    {
    }

    public void BindToController(CarController controller)
    {
        this.controller = controller;
        controller.onHealthChanged.AddListener(OnHealthChangedCallback);
        livesRemaining1.SetActive(CarSceneManager.Instance.PlayerLives >= 1);
        livesRemaining2.SetActive(CarSceneManager.Instance.PlayerLives >= 2);
        livesRemaining3.SetActive(CarSceneManager.Instance.PlayerLives >= 3);
        victoriesText.text = CarSceneManager.Instance.GamesWon.ToString();
    }

    void OnHealthChangedCallback(float health, float maxHealth)
    {
        int active = Mathf.Clamp((int)(Mathf.Ceil(health) / maxHealth * healthPoints.transform.childCount), 0, healthPoints.transform.childCount);
        for (int i = 0; i < healthPoints.transform.childCount; i++)
        {
            healthPoints.transform.GetChild(i).gameObject.SetActive(i < active);
        }
    }
}
