using UnityEngine;
using TMPro;

public class ScoreUpdater : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;

    public void UpdateScore(int score){
        scoreText.text = ("Distance: " + score.ToString());
    }
}
