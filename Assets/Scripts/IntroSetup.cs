using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSetup : MonoBehaviour
{

    [SerializeField]
    private float introWaitTime= 60;

    void Start()
    {
        StartCoroutine(IntroWait());
    }

    IEnumerator IntroWait()
    {
        yield return new WaitForSeconds(introWaitTime);
        SceneManager.LoadScene("TutorialScene");
    }
}