using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    Scene currentScene;
    string currentSceneName;
    bool firstRun = true;
    public bool isEndless = false;

    public int distance = 0;
    public float time;

    [SerializeField]
    private TextMeshProUGUI timerText;

    [SerializeField]
    private int runDist = 100;
    [SerializeField]
    private TextMeshProUGUI congratsText;
    [SerializeField]
    private UnityEvent<int> scoreUpdateEvent;

    void Awake()
    {
        // DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Debug.Log("first run: " + firstRun);
        currentScene = SceneManager.GetActiveScene();
        currentSceneName = currentScene.name;

        if (currentSceneName == "IntroScene")
        {
            // run tutorialsetup
            firstRun = false;
        }
        else if (currentSceneName == "EndScene")
        {
            Invoke("PlayEnding", 3);
        }
    }

    void Update()
    {
        if (currentSceneName == "GameScene")
        {
            timerText.text = "Time: " + Time.timeSinceLevelLoad.ToString("F2");
            // add distance score, if isEndless, distance counts up. otherwise counts down
        }
        
        if (distance >= runDist)
        {
            Debug.Log(distance + "switched to ending");
            SwitchToEnding();
        }
    }

    public void UpdateDistance(int newDist)
    {
        distance = newDist;
        scoreUpdateEvent.Invoke(distance);
        // Debug.Log("updated distance: " + distance);
    }
    void SwitchToEnding() 
    {
        SceneManager.LoadScene(3);
    }

    void PlayEnding()
    {
        congratsText.text = "Congrats! \n You made your flight in time!";
    }

    public void RestartGame() {
        //UI text says game over
        Debug.Log("game over");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    IEnumerator Delay(int waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }

}
