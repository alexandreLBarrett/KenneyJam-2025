using System.Collections.Generic;
using KenneyJam.Game.PlayerCar;
using UnityEngine;

public class FightingGameMode : MonoBehaviour
{
    public GameObject[] spawnPoints;
    public GameObject PlayerPrefab;
    
    public GameObject AIPrefab;

    public CarDataPreset[] aiPresets;
        
    public static void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
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
        }
    }

    void SpawnPlayer(GameObject gameObject)
    {
        GameObject playerCar = Instantiate(PlayerPrefab);
        playerCar.transform.position = gameObject.transform.position;
        playerCar.transform.rotation = gameObject.transform.rotation;
    }
    
    void SpawnBot(GameObject gameObject)
    {
        GameObject aiCar = Instantiate(AIPrefab);
        aiCar.transform.position = gameObject.transform.position;
        aiCar.transform.rotation = gameObject.transform.rotation;

        ModularCar modularCar = aiCar.GetComponentInChildren<ModularCar>();
        
        modularCar.preset = aiPresets[Random.Range(0, aiPresets.Length)];
    }
}
