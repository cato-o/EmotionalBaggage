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
    [SerializeField] private Button creditScene;
    
    public int distance = 0;
    public float time;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private int runDist = 500;
    [SerializeField] private UnityEvent<int> scoreUpdateEvent;

    [SerializeField] private TextMeshProUGUI congratsText;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Debug.Log("first run: " + firstRun);
    }

    void Update()
    {
        currentScene = SceneManager.GetActiveScene();
        currentSceneName = currentScene.name;
        
        if (currentSceneName == "GameScene")
        {
            distanceText = GameObject.Find("Distance").GetComponent<TextMeshProUGUI>();
            timerText = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
            timerText.text = string.Format("{0:0}:{1:00}", Mathf.FloorToInt(Time.timeSinceLevelLoad / 60F), Mathf.FloorToInt(Time.timeSinceLevelLoad - (Mathf.FloorToInt(Time.timeSinceLevelLoad / 60F)) * 60));
        }
        
        if (currentSceneName == "GameScene" && !isEndless && distance < 0)
        {
            Debug.Log(distance + "switched to ending");
            LoadEnding();
        }

        if (currentSceneName == "EndScene")
        {
            Invoke("PlayEnding", 3);
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

        UpdateDistanceText(distance);
        // scoreUpdateEvent.Invoke(distance);
        // Debug.Log("updated distance: " + distance);
    }

    public void UpdateDistanceText(int distance)
    {
        distanceText.text = "Distance: " + distance.ToString() + "m";
    }
    void PlayEnding()
    {
        congratsText.text = "Congrats! \n You made your flight in time!";
    }

    IEnumerator Delay(int waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("quitting");
    }

}
