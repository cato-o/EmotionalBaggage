using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public GameObject pauseMenuCanvas; 

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            Pause();
        }
        else if (isPaused && Input.anyKeyDown)
        {
            ResumeGame();
        }
    }

    void Pause()
    {
        pauseMenuCanvas.SetActive(true); // Activate the pause menu canvas
        Time.timeScale = 0f; // Pause the game
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuCanvas.SetActive(false); // Deactivate the pause menu canvas
        Time.timeScale = 1f; // Resume the game
        isPaused = false;
    }
}

