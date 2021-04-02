using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.UI;

public class PauseGameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject pauseButton;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.GetComponent<Canvas>().enabled = false;
        pauseButton.SetActive(false); //.GetComponent<Canvas>().enabled = true;
    }
    
    /// <summary>
    /// The game is stopped and the pause menu opened.
    /// </summary>
    public void StopGame()
    {
        Time.timeScale = 0.0f;
        pauseMenu.GetComponent<Canvas>().enabled = true;
        pauseButton.SetActive(false); // GetComponent<Canvas>().enabled = false;
    }

    /// <summary>
    /// The game is continued and the pause menu closed.
    /// </summary>
    public void Resume()
    {
        Time.timeScale = 1.0f;
        pauseMenu.GetComponent<Canvas>().enabled = false;
        pauseButton.SetActive(true); //.GetComponent<Canvas>().enabled = true;
    }

    /// <summary>
    /// The level gets restarted. Before that an new high score is saved if one was set.
    /// </summary>
    public void RestartLevel()
    {
        Time.timeScale = 1.0f;
        if(SnakeHeadController.GameStarted)
            GetComponent<GameOverManager>().SetNewHighscore();
        FullVersion.Instance.ShowAdCounter += GetComponent<GameOverManager>().GetScore();
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// The menu scene is loaded. Before that a new high score is added to the high scores list if one was set.
    /// </summary>
    public void BackToMenu()
    {
        Time.timeScale = 1.0f;
        if(SnakeHeadController.GameStarted)
            GetComponent<GameOverManager>().SetNewHighscore();
        FullVersion.Instance.ShowAdCounter += GetComponent<GameOverManager>().GetScore();
        SceneManager.LoadScene("Menu");
    }
}
