using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject healthPoints;
    [SerializeField]
    private TextMeshProUGUI victoriesText;
    [SerializeField]
    private CarController controller;

    void Start()
    {
        if (controller != null)
        {
            controller.onHealthChanged.AddListener(OnHealthChangedCallback);
        }
    }

    void Update()
    {
    }

    void OnHealthChangedCallback(float health, float maxHealth)
    {
        int active = Mathf.Clamp((int)(Mathf.Ceil(health) / maxHealth), 0, healthPoints.transform.childCount);
        for (int i = 0; i < healthPoints.transform.childCount; i++)
        {
            healthPoints.transform.GetChild(i).gameObject.SetActive(i < active);
        }
    }
}
