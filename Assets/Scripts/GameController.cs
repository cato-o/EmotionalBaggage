using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    Scene currentScene;
    string currentSceneName;
    bool firstRun = true;
    public bool isEndless = false;

    [SerializeField] private Button storyMode;
    [SerializeField] private Button endlessMode;
    [SerializeField] private Button creditMode;
    public int distance = 0;
    public float time;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private int runDist = 1000;
    [SerializeField] private UnityEvent<int> scoreUpdateEvent;

    [SerializeField] private TextMeshProUGUI congratsText;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Debug.Log("first run: " + firstRun);
        currentScene = SceneManager.GetActiveScene();
        currentSceneName = currentScene.name;

        if (currentSceneName == "EndScene")
        {
            Invoke("PlayEnding", 3);
        }
        if (GameObject.Find("Timer") != null)
        {
            timerText = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        if (currentSceneName == "GameScene")
        {
            timerText.text = "Time: " + Time.timeSinceLevelLoad.ToString("F2");
            // add distance score, if isEndless, distance counts up. otherwise counts down
        }
        
        if (currentSceneName == "GameScene" && !isEndless && distance < 0)
        {
            Debug.Log(distance + "switched to ending");
            LoadEnding();
        }
    }

    public void LoadStory()
    {
        isEndless = false;
        SceneManager.LoadScene("IntroScene");
    }

    public void LoadEndless()
    {
        Debug.Log("load endless");
        isEndless = true;
        SceneManager.LoadScene("GameScene");
    }

    void LoadEnding() 
    {
        SceneManager.LoadScene("EndScene");
    }

    public void LoadCredits()
    {
        Debug.Log("load credits");
        // add credits logic here
    }

    public void UpdateDistance(int newDist)
    {
        if (currentSceneName == "TutorialScene")
        {
            return;
        }
        else if (isEndless)
        {
            distance = newDist;
        }
        else
        {
            distance = runDist - newDist;
        }

        scoreUpdateEvent.Invoke(distance);
        // Debug.Log("updated distance: " + distance);
    }

    void PlayEnding()
    {
        congratsText.text = "Congrats! \n You made your flight in time!";
    }

    // public void RestartGame() {
    //     //UI text says game over
    //     Debug.Log("game over");
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    // }
    IEnumerator Delay(int waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }

}
