using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Tutorial : MonoBehaviour
{

    [SerializeField]
    private GameObject moveInfo;
    [SerializeField]
    private GameObject turnInfo;
    [SerializeField]
    private GameObject jumpInfo;
    [SerializeField]
    private GameObject slideInfo;
    [SerializeField]
    private GameObject tutorialEnd;

    private float sceneWaitTime= 0.1f;


    private Dictionary<string, GameObject> instructions = new Dictionary<string,GameObject>();

    void Start()
    {
        instructions.Add("Move", moveInfo);
        instructions.Add("Turn", turnInfo);
        instructions.Add("Jump", jumpInfo);
        instructions.Add("Slide", slideInfo);
        instructions.Add("TutorialEnd", tutorialEnd);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;
        Debug.Log("collided with: " + other.tag);
        if(instructions.ContainsKey(other.tag))
        {
            // Debug.Log(other.tag);
            instructions[other.tag].SetActive(true);
        }
        else if (other.tag == "SwitchScene")
        {
            StartCoroutine(StartGame());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(instructions.ContainsKey(other.tag))
        {
            instructions[other.tag].SetActive(false);
        }
    }

    IEnumerator StartGame()
    {
        Debug.Log("tutorial scene starting");
        yield return new WaitForSeconds(sceneWaitTime);
        SceneManager.LoadScene("GameScene");
    }
}
