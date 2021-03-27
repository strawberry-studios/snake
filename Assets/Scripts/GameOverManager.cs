using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class GameOverManager : MonoBehaviour
{
    public GameObject pauseButton, gameOverCanvas;
    public Text gameOverScore;
    public Text score;
    private int currentScore, scoreAsPercentage, sqauresOfGamingArea;
    public GameObject snakeHead;
    private int currentHighscoreAsPercentage;

    // Start is called before the first frame update
    void Start()
    {
        gameOverCanvas.GetComponent<Canvas>().enabled = false;
        sqauresOfGamingArea = GetComponent<CreateWorld>().GetColumns() * GetComponent<CreateWorld>().GetRows();
        currentScore = 1;
        SetScoreText(currentScore);
        score.GetComponent<Text>().enabled = false;
        currentHighscoreAsPercentage = DataSaver.Instance.GetHighscore();
        PopUpAdsManager.Instance.SetUpMonetization();
    }

    private void Update()
    {
        if(!score.GetComponent<Text>().enabled && snakeHead.GetComponent<SnakeHeadController>().GetDirection() != SnakeHeadController.DIRECTION.none)
        {
            score.GetComponent<Text>().enabled = true;
        }
    }

    /// <summary>
    /// The gameOverMenu is opened and the pause Button disabled.
    /// </summary>
    public void GameOver()
    {
        pauseButton.GetComponent<Canvas>().enabled = false;
        ShowAds();
        gameOverCanvas.GetComponent<Canvas>().enabled = true;
        gameOverScore.text = "Score: " + scoreAsPercentage + "%";
    }

    /// <summary>
    /// Shows either no ad, a banner ad, an interstitial video or a non-skippable video depending on the score of the last game.
    /// </summary>
    private void ShowAds()
    {
        if (scoreAsPercentage > 50)
            PopUpAdsManager.Instance.ShowAd("video");
        else if (scoreAsPercentage > 25)
            PopUpAdsManager.Instance.ShowAd("InterstitialVideo");
        //else if (scoreAsPercentage > 15)
          //  PopUpAdsManager.Instance.ShowAd("BannerAd");
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
        scoreAsPercentage = Mathf.RoundToInt(scoring*100 / sqauresOfGamingArea);
        score.text = "" + scoreAsPercentage + "%";
    }

    /// <summary>
    /// If a new highscore is reached it is saved to an external file by calling SaveNewHighscore().
    /// </summary>
    public void SetNewHighscore()
    {
        if(scoreAsPercentage > currentHighscoreAsPercentage)
            DataSaver.Instance.SaveNewHighscore(scoreAsPercentage);
    }
}
