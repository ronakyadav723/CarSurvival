using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   
    public static bool isFirstScenePlayed = false;

    void Start()
    {
        
        if (isFirstScenePlayed)
        {
            SceneManager.UnloadSceneAsync(0);
            SceneManager.LoadSceneAsync(1);
        }
    }

   
    public void StartGame()
    {
        isFirstScenePlayed = true;
        SceneManager.LoadSceneAsync(1);
        SceneManager.UnloadSceneAsync(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
