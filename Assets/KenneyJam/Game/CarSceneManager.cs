using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New CarSceneManager", menuName = "Scriptable Objects/CarSceneManager")]
public class CarSceneManager : ScriptableObject
{
    public string garageScene;
    public string gameScene;
    public float transitionDuration = .5f;

    public void LoadGarage()
    {
        FindAnyObjectByType<MonoBehaviour>().StartCoroutine(StartLevelTransition(garageScene));
    }
    
    public void LoadGame()
    {
        FindAnyObjectByType<MonoBehaviour>().StartCoroutine(StartLevelTransition(gameScene));
    }

    private IEnumerator StartLevelTransition(string toScene)
    {
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
