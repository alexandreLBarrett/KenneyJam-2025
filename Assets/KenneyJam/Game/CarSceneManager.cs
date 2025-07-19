using KenneyJam.Game.PlayerCar;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarSceneManager : MonoBehaviour
{
    public string menuScene = "MainMenuScene";
    public string garageScene = "GarageScene";
    public string gameScene = "GameScene";
    public float transitionDuration = .8f;

    public int PlayerLives = 3;
    public int GamesWon = 0;

    public static CarSceneManager Instance { get {
        CarSceneManager existing = FindAnyObjectByType<CarSceneManager>();
        if (existing) return existing;

        GameObject instance = Instantiate(new GameObject());
        DontDestroyOnLoad(instance);
        return instance.AddComponent<CarSceneManager>();
    } }

    public void StartGame()
    {
        Destroy(PersistentCarData.GetPersistentCarData());
        GamesWon = 0;
        PlayerLives = 3;
        LoadGame();
    }

    public void TriggerGameLost()
    {
        PlayerLives--;
        if (PlayerLives <= 0)
        {
            SoundManager.Instance.PlayInstantSound(SoundManager.Instance.soundBank.GameLost);
            StartCoroutine(StartLevelTransition(menuScene, 2));
        }
        else
        {
            SoundManager.Instance.PlayInstantSound(SoundManager.Instance.soundBank.MatchLost);
            StartCoroutine(StartLevelTransition(gameScene, 2));
        }
    }

    public void TriggerGameWon()
    {
        StartCoroutine(StartLevelTransition(garageScene, 2f));
    }

    public void LoadGarage()
    {
        StartCoroutine(StartLevelTransition(garageScene));
    }
    
    public void LoadGame()
    {
        StartCoroutine(StartLevelTransition(gameScene));
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

}
