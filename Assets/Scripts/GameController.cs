using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartGame() {
        //UI text says game over
        Debug.Log("game over");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
