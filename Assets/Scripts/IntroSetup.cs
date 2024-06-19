using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSetup : MonoBehaviour
{

    [SerializeField]
    private float introWaitTime= 21;
    bool firstLoad = true;
    // void Start()
    // {
    // }

    void Update() {
        // Debug.Log("Intro scene started:" + SceneManager.GetActiveScene().name);

        if(SceneManager.GetActiveScene().name == "IntroScene" && firstLoad)
        {
            firstLoad = false;
            Debug.Log("at intro scene");
            StartCoroutine(IntroWait());
        }
    }

    IEnumerator IntroWait()
    {
        Debug.Log("intro scene loaded");
        yield return new WaitForSeconds(introWaitTime);
        SceneManager.LoadScene("TutorialScene");
    }
}