using System.Collections.Generic;
using KenneyJam.Game.PlayerCar;
using UnityEngine;

public class FightingGameMode : MonoBehaviour
{
    public GameObject[] spawnPoints;
    public GameObject PlayerPrefab;
    
    public GameObject AIPrefab;

    public CarDataPreset[] aiPresets;

    private bool matchEnded = false;
    private int remainingOponents;
        
    public static void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
    
    void Start()
    {
        if (spawnPoints.Length == 0)
            return;
        
        Shuffle(spawnPoints);

        SpawnPlayer(spawnPoints[0]);
        for (int i = 1; i < spawnPoints.Length; ++i)
        {
            SpawnBot(spawnPoints[i]);
            remainingOponents++;
        }
    }

    void SpawnPlayer(GameObject gameObject)
    {
        GameObject playerCar = Instantiate(PlayerPrefab);
        playerCar.transform.position = gameObject.transform.position;
        playerCar.transform.rotation = gameObject.transform.rotation;

        CarController controller = playerCar.GetComponentInChildren<CarController>();
        controller.onCarDeath.AddListener(t => {
            if (matchEnded) return;
            CarSceneManager.Instance.TriggerGameLost();
            matchEnded = true;
        });

        GameUIManager uiManager = FindAnyObjectByType<GameUIManager>();
        uiManager.BindToController(controller);
    }
    
    void SpawnBot(GameObject gameObject)
    {
        GameObject aiCar = Instantiate(AIPrefab);
        aiCar.transform.position = gameObject.transform.position;
        aiCar.transform.rotation = gameObject.transform.rotation;

        ModularCar modularCar = aiCar.GetComponentInChildren<ModularCar>();
        modularCar.preset = aiPresets[Random.Range(0, aiPresets.Length)];

        CarController controller = aiCar.GetComponentInChildren<CarController>();
        controller.onCarDeath.AddListener(t => {
            if (matchEnded) return;
            remainingOponents--;
            if (remainingOponents <= 0)
            {
                CarSceneManager.Instance.TriggerGameWon();
                matchEnded = true;
            }
        });
    }
}
