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
        pauseButton.GetComponent<Canvas>().enabled = true;
    }
    
    /// <summary>
    /// The game is stopped and the pause menu opened.
    /// </summary>
    public void StopGame()
    {
        Time.timeScale = 0.0f;
        pauseMenu.GetComponent<Canvas>().enabled = true;
        pauseButton.GetComponent<Canvas>().enabled = false;
    }

    /// <summary>
    /// The game is continued and the pause menu closed.
    /// </summary>
    public void Resume()
    {
        Time.timeScale = 1.0f;
        pauseMenu.GetComponent<Canvas>().enabled = false;
        pauseButton.GetComponent<Canvas>().enabled = true;
    }

    /// <summary>
    /// The level gets restarted.
    /// </summary>
    public void RestartLevel()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Game");
    }
}
