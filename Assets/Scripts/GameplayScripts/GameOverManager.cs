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
    public Text gameOverScore;
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
        if(!score.GetComponent<Text>().enabled && snakeHead.GetComponent<SnakeHeadController>().GetDirection() != SnakeHeadController.DIRECTION.none)
        {
            score.GetComponent<Text>().enabled = true;
        }
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
                break;
            case ControlsMode.swipeOnly:
                directionControllingButtons.SetActive(false);
                swipeControllers.SetActive(true);
                StartCoroutine(IncreaseSize(swipeControllers.GetComponentInChildren<Text>()));
                break;
            case ControlsMode.buttonsAndSwipe:
                directionControllingButtons.SetActive(true);
                swipeControllers.SetActive(false);
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

        //ads are shown if they weren't removed (by purchasing the full version) 
        //conditions: since the last ad was shown at least 40 blocks were collected, the ad is loaded, 
        //the game wasn't won (collecting 100% of all blocks)
        if (FullVersion.Instance.IsFullVersionUnlocked == FullVersionUnlocked.notUnlocked)
        {
            if (soundOn || vibrationOn)
                ShowAds(won, playGameOverSoundDelegate, vibrateOnLoseDelegate);
            else
                ShowAds(won);
        }
        else
        {
            PlayGameOverSound(soundOn, won);
            Vibration.Vibrate(1000);
        }

        gameOverCanvas.GetComponent<Canvas>().enabled = true;
        SetNewHighscore();
        gameOverScore.text = "Score: " + ScoreConverter.ConvertPermilleScoreToPercentage(scoreAsPermille);
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
    /// Makes the device vibrate if vibrationOn is set to true and if the game wasn't won.
    /// </summary>
    /// <param name="vibrationOn">Whether the vibration is on.</param>
    /// <param name="won">Whether the game was won.</param>
    void Vibrate(bool vibrationOn, bool won)
    {
        if (!won)
            Vibration.Vibrate(800);
    }

    /// <summary>
    /// Shows either no ad, a banner ad, an interstitial video or a non-skippable video depending on the score of the last game.
    /// </summary>
    /// <param name="gameWon">Whether the game was won or not.</param>
    private void ShowAds(bool gameWon)
    {
        FullVersion.Instance.ShowAdCounter += currentScore;
        print(FullVersion.Instance.ShowAdCounter);
        AdManager.Instance.ShowVideoAdOnLose(gameWon);
    }

    /// <summary>
    /// Shows either no ad, a banner ad, an interstitial video or a non-skippable video depending on the score of the last game.
    /// If no ad is shown (i.e. because the game was won or if no ad should be shown yet), a game over sound is played.
    /// </summary>
    /// <param name="gameWon">Whether the game was won or not.</param>
    /// <param name="playGameOverSound">A method which plays a game over sound if no ad will be shown.</param>
    /// ///<param name="playVibration">A method which makes the device vibrate if no ad is shown.</param>
    private void ShowAds(bool gameWon, SoundMethod playGameOverSound, SoundMethod playVibration)
    {
        FullVersion.Instance.ShowAdCounter += currentScore;
        //print(FullVersion.Instance.ShowAdCounter);
        AdManager.Instance.ShowVideoAdOnLose(gameWon, playGameOverSound, playVibration);
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
