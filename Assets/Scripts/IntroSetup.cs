using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSetup : MonoBehaviour
{

    [SerializeField]
    private float introWaitTime= 21;

    void Start()
    {
        Debug.Log("Intro scene started:" + SceneManager.GetActiveScene().name);
        if(SceneManager.GetActiveScene().name == "StartScene")
        {
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