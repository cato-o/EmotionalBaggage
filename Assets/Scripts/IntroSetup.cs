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
        if(SceneManager.GetActiveScene().name == "IntroScene")
        {
            StartCoroutine(IntroWait());
        }
    }

    IEnumerator IntroWait()
    {
        yield return new WaitForSeconds(introWaitTime);
        SceneManager.LoadScene("TutorialScene");
    }
}