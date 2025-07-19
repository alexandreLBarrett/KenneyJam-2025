using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New CarSceneManager", menuName = "Scriptable Objects/CarSceneManager")]
public class CarSceneManager : ScriptableObject
{
    public string garageScene;
    public string gameScene;
    
    public void LoadGarage()
    {
        SceneManager.LoadScene(garageScene);
    }
    
    public void LoadGame()
    {
        SceneManager.LoadScene(gameScene);
    }
}
