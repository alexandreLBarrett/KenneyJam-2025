using KenneyJam.Game.PlayerCar;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarSceneManager : MonoBehaviour
{
    public string menuScene = "MainMenuScene";
    public string defeatScene = "DefeatScene";
    public string garageScene = "GarageScene";
    public string winScene = "VictoryScene";
    public string gameScene = "MainScene";
    public float transitionDuration = .8f;

    public int playerLives;
    public int playerCurrency;
    public int currentMatch;

    public TournamentData tournamentData;
    
    public static CarSceneManager Instance { get {
            return instance;
    } }

    private static CarSceneManager instance;

    private void Awake()
    {
        if (instance != null && gameObject != instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Default init, only used for when we run a scene without passing through the typical flow (eg: when running the garage independantly).
        // Don't remove pls :-)
        currentMatch = 0;
        playerLives = tournamentData.startingLives;
        playerCurrency = tournamentData.startingCurrency;
    }

    public static void StaticStartGame()
    {
        Instance.StartGame();
    }

    public void StartGame()
    {
        Destroy(PersistentCarData.GetPersistentCarData());
        currentMatch = 0;
        playerLives = tournamentData.startingLives;
        playerCurrency = tournamentData.startingCurrency;
        LoadGarage();
    }

    public TournamentData.Match GetCurrentMatch()
    {
        return tournamentData.matches[currentMatch];
    }

    public void TriggerGameLost()
    {
        playerLives--;
        if (playerLives <= 0)
        {
            SoundManager.Instance.PlayInstantSound(SoundManager.Instance.soundBank.GameLost);
            StartCoroutine(StartLevelTransition(defeatScene, 2));
        }
        else
        {
            SoundManager.Instance.PlayInstantSound(SoundManager.Instance.soundBank.MatchLost);
            StartCoroutine(StartLevelTransition(gameScene, 2));
        }
    }

    public void TriggerGameWon(int currencyReward)
    {
        playerCurrency += currencyReward;
        currentMatch++;
        SoundManager.Instance.PlayInstantSound(SoundManager.Instance.soundBank.MatchWon);
        if (currentMatch < tournamentData.matches.Count)
        {
            StartCoroutine(StartLevelTransition(garageScene, 2f));
        }
        else
        {
            currentMatch = 0;
            StartCoroutine(StartLevelTransition(winScene, 2f));
        }
    }

    public void LoadGarage()
    {
        StartCoroutine(StartLevelTransition(garageScene));
    }
    
    public void LoadGame()
    {
        StartCoroutine(StartLevelTransition(gameScene));
    }
    
    public void LoadMainMenu()
    {
        StartCoroutine(StartLevelTransition(menuScene));
    }
    
    public static void LoadMainMenuStatic()
    {
        instance.LoadMainMenu();
    }

    private void StartCoroutine(IEnumerator c)
    {
        FindAnyObjectByType<MonoBehaviour>().StartCoroutine(c);
    }

    private IEnumerator StartLevelTransition(string toScene, float delay=0)
    {
        yield return new WaitForSeconds(delay);

        var transitionPanel = GameObject.FindGameObjectsWithTag("MenuTransition");
        if (transitionPanel.Length == 1)
        {
            var animator = transitionPanel[0].GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("StartFadeOut");
                yield return new WaitForSeconds(transitionDuration);
            }
        }
        SceneManager.LoadScene(toScene);
    }

    public void OnValidate()
    {
        if (tournamentData == null)
        {
            Debug.LogWarning("Tournament data has not been set for the CarSceneManager!");
        }
    }
}
