using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{

    [SerializeField]
    private GameObject moveInfo;
    // private TextMeshProUGUI moveText;
    [SerializeField]
    private GameObject turnInfo;
    // private TextMeshProUGUI turnText;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collided with: " + other.tag);
        if (other.tag == "Move")
        {
            moveInfo.SetActive(true);
            // moveText.enabled = true;
        }
        
        if (other.tag == "Turn")
        {
            turnInfo.SetActive(true);
            // turnText.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("left collision with: " + other.tag);
        if (other.tag == "Move")
        {
            moveInfo.SetActive(false);
            // moveText.enabled = false;
        }

        if (other.tag == "Turn")
        {
            turnInfo.SetActive(false);
            // turnText.enabled = false;
        }
    }
}
