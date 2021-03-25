using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class HighScoresController : MonoBehaviour
{
    public GameObject[] highScoresHolders;
    public Dropdown difficultyDropdown;

    // Start is called before the first frame update
    void Start()
    {
        LoadCurrentDifficulty();
        LoadHighScoresTable(HighScores.Instance.DifficultyLevel);
    }

    /// <summary>
    /// Loads the current difficulty level from an external file and sets the right value for the dropdown.
    /// </summary>
    void LoadCurrentDifficulty()
    {
        Difficulty currentDifficulty = HighScores.Instance.DifficultyLevel;
        switch(currentDifficulty)
        {
            case Difficulty.VeryEasy:
                difficultyDropdown.value = 0;
                break;
            case Difficulty.Easy:
                difficultyDropdown.value = 1;
                break;
            case Difficulty.Medium:
                difficultyDropdown.value = 2;
                break;
            case Difficulty.Hard:
                difficultyDropdown.value = 3;
                break;
            case Difficulty.VeryHard:
                difficultyDropdown.value = 4;
                break;
            case Difficulty.Ultimate:
                difficultyDropdown.value = 5;
                break;
            default: //can never occur, but is added to avoid a compilation error
                difficultyDropdown.value = 0;
                break;
        }
    }

    /// <summary>
    /// Loads the highScores table with the passed 'difficulty' from an external file. The difficulties are: 0 = very easy, 1 = easy, 
    /// 2 = medium, 3 = hard, 4 = very hard.
    /// </summary>
    /// <param name="index">The difficulty index as int.</param>
    public void LoadHighScoresTable(int index)
    {
        switch (index)
        { case 0:
                LoadActualTable(HighScores.Instance.GetHighScores(Difficulty.VeryEasy));
                break;
            case 1:
                LoadActualTable(HighScores.Instance.GetHighScores(Difficulty.Easy));
                break;
            case 2:
                LoadActualTable(HighScores.Instance.GetHighScores(Difficulty.Medium));
                break;
            case 3:
                LoadActualTable(HighScores.Instance.GetHighScores(Difficulty.Hard));
                break;
            case 4:
                LoadActualTable(HighScores.Instance.GetHighScores(Difficulty.VeryHard));
                break;
            case 5:
                LoadActualTable(HighScores.Instance.GetHighScores(Difficulty.Ultimate));
                break;
        }
    }

    /// <summary>
    /// Loads the highScores table with the passed 'difficulty' from an external file. 
    /// </summary>
    /// <param name="difficulty">The difficulty as 'Difficulty'.</param>
    public void LoadHighScoresTable(Difficulty difficulty)
    {
        LoadActualTable(HighScores.Instance.GetHighScores(difficulty));
    }

    /// <summary>
    /// Assigns the passed scores to the high-score-text-objects in the scene.
    /// </summary>
    /// <param name="scores">The scores as an int array.</param>
    void LoadActualTable(int[] scores)
    {
        int highScoresLength = highScoresHolders.Length;
        int scoresLength = scores.Length;
        int iterationMax = highScoresLength > scoresLength ? highScoresLength : scoresLength;
        for (int i = 0; i < iterationMax; i++)
        {
            highScoresHolders[i].GetComponent<Text>().text = ScoreConverter.ConvertPermilleScoreToPercentage(scores[i]);
            //print(ScoreConverter.ConvertPermilleScoreToPercentage(scores[i]));
        }
    }
}
