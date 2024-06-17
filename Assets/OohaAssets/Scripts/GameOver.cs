using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class GameOver : MonoBehaviour
{
    [SerializeField]
    private GameObject gameOverCanvas;

    [SerializeField] private TextMeshProUGUI scoreText;

    private int score = 0;

    public void StopGame(int score) {
        gameOverCanvas.SetActive(true);
        this.score = score;
        scoreText.text = "Distance: " +score.ToString() + "m";
    }
    public void AddXP(int score) {

    }

    public void ReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
