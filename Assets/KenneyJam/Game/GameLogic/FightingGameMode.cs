using System.Collections.Generic;
using KenneyJam.Game.PlayerCar;
using UnityEngine;

public class FightingGameMode : MonoBehaviour
{
    public GameObject[] spawnPoints;
    public GameObject PlayerPrefab;
    
    public GameObject AIPrefab;

    public CarDataPreset[] aiPresets;
    public CarDataPreset[] bossPresets;

    private bool matchEnded = false;
    private int remainingOponents;
    private TournamentData.Match match;
        
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

    private void Awake()
    {
        match = CarSceneManager.Instance.GetCurrentMatch();
    }

    void Start()
    {
        StartPrematch();
    }

    void StartMatch()
    {
        if (match.playerCount == 2)
        {
            SpawnPlayer(spawnPoints[0]);
            SpawnBot(spawnPoints[2], true);
        }
        else
        {
            Shuffle(spawnPoints);

            SpawnPlayer(spawnPoints[0]);
            for (int i = 1; i < Mathf.Min(match.playerCount, spawnPoints.Length); ++i)
            {
                SpawnBot(spawnPoints[i], false);
                remainingOponents++;
            }
        }
    }

    void StartPrematch()
    {
        // TODO: display info about the game (UI to show the rewards and the enemies with their names)

        // Then start the match after countdown:
        StartMatch();
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

    void SpawnBot(GameObject gameObject, bool finalRound)
    {
        GameObject aiCar = Instantiate(AIPrefab);
        aiCar.transform.position = gameObject.transform.position;
        aiCar.transform.rotation = gameObject.transform.rotation;

        ModularCar modularCar = aiCar.GetComponentInChildren<ModularCar>();
        CarDataPreset[] presets = finalRound ? bossPresets : aiPresets; 
        modularCar.preset = presets[Random.Range(0, presets.Length)];

        CarController controller = aiCar.GetComponentInChildren<CarController>();
        controller.onCarDeath.AddListener(t =>
        {
            if (matchEnded) return;
            remainingOponents--;
            if (remainingOponents <= 0)
            {
                CarSceneManager.Instance.TriggerGameWon(match.currencyReward);
                matchEnded = true;
            }
        });
    }
}
