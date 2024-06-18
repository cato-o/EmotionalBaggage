using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    Scene currentScene;
    string currentSceneName;
    bool firstRun = true;

    void Start()
    {
        Debug.Log("first run: " + firstRun);
        currentScene = SceneManager.GetActiveScene();
        currentSceneName = currentScene.name;
        if (currentSceneName == "IntroScene")
        {
            // run tutorialsetup
            firstRun = false;
        }
    }

    public void RestartGame() {
        //UI text says game over
        Debug.Log("game over");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
