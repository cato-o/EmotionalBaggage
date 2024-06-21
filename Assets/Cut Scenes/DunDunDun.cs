using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DunDunDun : MonoBehaviour
{
    public GameObject[] gameObjects;  // Array to hold the game objects
    public float[] activeTimes;       // Array to hold the activation times for each game object
    public GameObject finalGameObject; // The final game object to be activated

    // Ensure that the arrays are of the same length
    void Start()
    {
        if (gameObjects.Length != activeTimes.Length)
        {
            Debug.LogError("The lengths of gameObjects and activeTimes arrays must be the same.");
        }
    }

    // Call this method to start the activation process
    public void StartActivation()
    {
        StartCoroutine(ActivateObjects());
    }

    private IEnumerator ActivateObjects()
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].SetActive(true);  // Activate the current game object
            yield return new WaitForSeconds(activeTimes[i]);  // Wait for the specified time
            gameObjects[i].SetActive(false); // Deactivate the current game object
        }

        finalGameObject.SetActive(true); // Activate the final game object
    }
}