using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseGameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject pauseButton;
    public GameObject timeUntilResumeText;
    public GameObject directionManager;
    public GameObject controllersDisabledPanel;

    /// <summary>
    /// Whether the game is paused or not.
    /// </summary>
    public bool GamePaused {get; set;}

    private void Awake()
    {
        GamePaused = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.GetComponent<Canvas>().enabled = false;
        controllersDisabledPanel.SetActive(false);
        pauseButton.SetActive(true);
        timeUntilResumeText.SetActive(false);
    }
    
    /// <summary>
    /// The game is stopped and the pause menu opened.
    /// </summary>
    public void StopGame()
    {
        Time.timeScale = 0.0f;
        GamePaused = true;
        directionManager.GetComponent<SetDirectionManager>().SetAwaitingSwipes(false);
        pauseMenu.GetComponent<Canvas>().enabled = true;
        pauseButton.SetActive(false);
        controllersDisabledPanel.SetActive(true);
        timeUntilResumeText.SetActive(false);
    }

    /// <summary>
    /// Starts a coroutine which resumes the game after the passed time.
    /// </summary>
    /// <param name="timeInSeconds">The time until the game is resumed.</param>
    public void ResumeGameIn(int timeInSeconds)
    {
        pauseMenu.GetComponent<Canvas>().enabled = false;
        timeUntilResumeText.SetActive(true);

        if (timeInSeconds > 0)
            StartCoroutine(ResumeGameInS(timeInSeconds));
        else
            Resume();
    }

    /// <summary>
    /// Coroutine which resumes the game after the passed time. 
    /// </summary>
    /// <param name="timeInSeconds"></param>The time until the game is resumed is displayed in the bottom right corner of the screen (instead of the pause button).</param>
    /// <returns></returns>
    IEnumerator ResumeGameInS(int timeInSeconds)
    {
        timeUntilResumeText.GetComponent<Text>().text = "" + timeInSeconds;
        float timeSinceStart = 0;

        while(timeSinceStart < 1)
        {
            timeSinceStart += Time.unscaledDeltaTime;
            yield return null;
        }

        timeInSeconds--;
        if (timeInSeconds == 0)
            Resume();
        else
            StartCoroutine(ResumeGameInS(timeInSeconds));
    }

    /// <summary>
    /// The game is continued and the pause menu closed.
    /// </summary>
    void Resume()
    {
        Time.timeScale = 1.0f;
        GamePaused = false;
        pauseButton.SetActive(true);
        timeUntilResumeText.SetActive(false);
        controllersDisabledPanel.SetActive(false);
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
