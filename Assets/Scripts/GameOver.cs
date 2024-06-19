using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class GameOver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private int score = 0;

    public void StopGame(int score) {
        this.score = score;
        scoreText.text = "Distance: " +score.ToString() + "m";

        Time.timeScale = 0f;
    }

    public void ReloadScene() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }
}
