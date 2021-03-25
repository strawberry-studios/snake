using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public delegate void SoundMethod(bool parameter1, bool parameter2);

public class GameOverManager : MonoBehaviour
{
    /// <summary>
    /// The current mode with which the snake is controlled.
    /// </summary>
    private ControlsMode currentControlsMode;
    public GameObject pauseButton, gameOverCanvas;
    public Text gameOverScore, gameOverText;
    public Text score;
    private int currentScore, scoreAsPermille, sqauresOfGamingArea;
    public GameObject snakeHead;
    /// <summary>
    /// game objects which are needed when 'buttons' are used for controlling the snake
    /// </summary>
    public GameObject directionControllingButtons;
    /// <summary>
    /// game objects which are needed when 'swipes' are used for controlling the snake
    /// </summary>
    public GameObject swipeControllers;
    /// <summary>
    /// Object which has scripts attached to it that are used for controlling the direction.
    /// </summary>
    public GameObject directionController;
    /// <summary>
    /// The sound controller
    /// </summary>
    GameObject soundController;
    /// <summary>
    /// Delegate which references the playGameOverSoundMethod.
    /// </summary>
    SoundMethod playGameOverSoundDelegate;
    /// <summary>
    /// Delegate which references the vibrateOnLoseMethod.
    /// </summary>
    SoundMethod vibrateOnLoseDelegate;
    /// <summary>
    /// Whether the sound effects are enabled or not.
    /// </summary>
    bool soundOn;
    /// <summary>
    /// Whether the vibrations should be enabled or not.
    /// </summary>
    bool vibrationOn;

    //private int currentHighscoreAsPermille;

    // Start is called before the first frame update
    void Start()
    {
        soundController = GameObject.FindGameObjectWithTag("SoundController");
        soundOn = DataSaver.Instance.SoundOn;
        playGameOverSoundDelegate = PlayGameOverSound;
        vibrateOnLoseDelegate = Vibrate;
        gameOverCanvas.GetComponent<Canvas>().enabled = false;
        sqauresOfGamingArea = GetComponent<CreateWorld>().GetColumns() * GetComponent<CreateWorld>().GetRows();
        currentScore = 1;
        SetScoreText(currentScore);
        score.GetComponent<Text>().enabled = false;
        //currentHighscoreAsPermille = DataSaver.Instance.GetHighscore();
        //PopUpAdsManager.Instance.SetUpMonetization();
        ToggleControllersEnabled();
    }

    private void Update()
    {

    }

    /// <summary>
    /// Enables the score UI. This method is called when the player starts the game by swiping/pressing a button.
    /// </summary>
    public void EnableScoreUIs()
    {
        score.GetComponent<Text>().enabled = true;
    }

    /// <summary>
    /// Loads the current controls mode.
    /// Toggles the controllers which are needed active and the others inactive. (Depending on the current 'controls mode')
    /// </summary>
    void ToggleControllersEnabled()
    {
        currentControlsMode = DataSaver.Instance.ControlsModeActivated;
        switch(currentControlsMode)
        {
            case ControlsMode.buttonsOnly:
                directionControllingButtons.SetActive(true);
                swipeControllers.SetActive(false);
                directionController.GetComponent<SetDirectionManager>().keypadButtons.ToggleKeypadControllersActive(false);
                break;
            case ControlsMode.swipeOnly:
                directionControllingButtons.SetActive(false);
                swipeControllers.SetActive(true);
                directionController.GetComponent<SetDirectionManager>().keypadButtons.ToggleKeypadControllersActive(false);
                StartCoroutine(IncreaseSize(swipeControllers.GetComponentInChildren<Text>()));
                break;
            case ControlsMode.keypad:
                directionControllingButtons.SetActive(false);
                swipeControllers.SetActive(false);
                directionController.GetComponent<SetDirectionManager>().keypadButtons.ToggleKeypadControllersActive(true);
                break;
            case ControlsMode.buttonsAndSwipe:
                directionControllingButtons.SetActive(true);
                swipeControllers.SetActive(false);
                directionController.GetComponent<SetDirectionManager>().keypadButtons.ToggleKeypadControllersActive(false);
                break;
        }
    }

    /// <summary>
    /// Toggles the swipe controllers (the 'swipe to start' text) (in)active.
    /// </summary>
    /// <param name="newState">The new activity state of the swipes controllers.</param>
    public void ToggleSwipeControllersActive(bool newState)
    {
        swipeControllers.SetActive(newState);
    }

    /// <summary>
    /// The gameOverMenu is opened and the pause Button disabled.
    /// </summary>
    /// <param name="won">Whether the game was won or not.</param>
    public void GameOver(bool won)
    {
        pauseButton.GetComponent<Canvas>().enabled = false;
        PlayerData currentData = DataSaver.Instance.RetrievePlayerDataFromFile();
        vibrationOn = currentData.VibrationsOn;
        soundOn = currentData.SoundOn;

        PlayGameOverSound(soundOn, won);
        Vibrate(vibrationOn, won);

        gameOverCanvas.GetComponent<Canvas>().enabled = true;
        SetNewHighscore();
        gameOverScore.text = "Score: " + ScoreConverter.ConvertPermilleScoreToPercentage(scoreAsPermille);
        gameOverText.text = won ? "YOU WIN!" : "GAME OVER!";
        Time.timeScale = 0;
    }

    /// <summary>
    /// Plays a game over sound if the passed parameter is true. 
    /// </summary>
    /// <param name="won">Whether the game was won or not.</param>
    /// <param name="playSound">Whether the sound should even be played or not.</param>
    void PlayGameOverSound(bool playSound, bool won)
    {
        if(playSound)
        {
            if (won)
                soundController.GetComponent<SoundController>().PlayGameWonSound();
            else
                soundController.GetComponent<SoundController>().PlayGameOverSound();
        }
    }

    /// <summary>
    /// Makes the device vibrate if vibrationOn is set to true. The length of the played sound is 0.8s if lost, elsewhise 1 second.
    /// </summary>
    /// <param name="vibrationOn">Whether the vibration is on.</param>
    /// <param name="won">Whether the game was won.</param>
    void Vibrate(bool vibrationOn, bool won)
    {
        if (vibrationOn)
        {
            if (!won)
                Vibration.Vibrate(800);
            else
                Vibration.Vibrate(1000);
        }
    }

    /// <summary>
    /// Sets the current score.
    /// </summary>
    /// <param name="newScore">New score as int to pass</param>
    public void SetScore(int newScore)
    {
        currentScore = newScore;
        SetScoreText(currentScore);
    }

    /// <summary>
    /// Adds the passed score to the current score.
    /// </summary>
    /// <param name="extraScore">Additional score as int to pass</param>
    public void AddToScore(int extraScore)
    {
        currentScore += extraScore;
        SetScoreText(currentScore);
    }

    /// <summary>
    /// Returns the current score.
    /// </summary>
    /// <returns>Returns the score as int</returns>
    public int GetScore()
    {
        return currentScore;
    }

    /// <summary>
    /// Displays the passed scoring value.
    /// </summary>
    /// <param name="scoring">scoring as int to pass</param>
    private void SetScoreText(int scoring)
    {
        scoreAsPermille = Mathf.RoundToInt(scoring*1000 / sqauresOfGamingArea);
        score.text = ScoreConverter.ConvertPermilleScoreToPercentage(scoreAsPermille);
    }

    /// <summary>
    /// If a new highscore is reached it is saved to an external file by calling SaveNewHighscore().
    /// </summary>
    public void SetNewHighscore()
    {
        HighScores.Instance.SetNewHighScore(scoreAsPermille);
        //if (scoreAsPermille > currentHighscoreAsPermille)
          //  DataSaver.Instance.SaveNewHighscore(scoreAsPermille);
    }

    /// <summary>
    /// Restarts the game.
    /// </summary>
    public void RestartLevel()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Game");
    }

    //methods for animating the 'Swipe to start' - statement:

    /// <summary>
    /// Gradually increases the size of the 'swipe to start' text. If the maximum font size is reached, it starts decreasing again.
    /// The animation ends when the parent of the text is set inactive.
    /// </summary>
    /// <param name="thisText">The text componenent holding the text which is to be animated.</param>
    /// <returns></returns>
    IEnumerator IncreaseSize(Text thisText)
    {
        while(thisText.fontSize < 75)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            thisText.fontSize++;
        }
        if(thisText.transform.parent.gameObject.activeInHierarchy)
            StartCoroutine(DecreaseSize(thisText));
    }

    /// <summary>
    /// Gradually decreases the size of the 'swipe to start' text. If the minimum font size is reached, it starts increasing again.
    /// The animation ends when the parent of the text is set inactive.
    /// </summary>
    /// <param name="thisText">The text componenent holding the text which is to be animated.</param>
    /// <returns></returns>
    IEnumerator DecreaseSize(Text thisText)
    {
        while(thisText.fontSize > 65)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            thisText.fontSize--;
        }
        if (thisText.transform.parent.gameObject.activeInHierarchy)
            StartCoroutine(IncreaseSize(thisText));
    }
}
