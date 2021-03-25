using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class HighScores : Singleton<HighScores>
{
    /// <summary>
    /// Checks whether a passed score that was achieved is good enough for making it into the high scores list. If so, it is added to the 
    /// high scores and saved to an external file.
    /// </summary>
    /// <param name="score">The score which should be integrated into a high scores table if high enough.</param>
    public void SetNewHighScore(int score)
    {
        HighScoresData data = RetrieveHighScoresDataFromFile();
        data.MakeNewHighScoresEntry(score);
        SaveHighScoresDataToFile(data);
    }

    /// <summary>
    /// Loads a high scores array from an external file and returns it.
    /// </summary>
    /// <returns>Returns a high scores list as int array.</returns>
    /// <param name="difficultyLevel">The difficulty level of which the high scores array should be returned.</param>
    public int[] GetHighScores(Difficulty difficultyLevel)
    {
        HighScoresData data = RetrieveHighScoresDataFromFile();
        return data.GetHighScores(difficultyLevel);
    }

    /// <summary>
    /// Retrieves the current difficulty level (with which the player plays) from an external file.
    /// (The difficulty value depends on the settings: 'WorldBoundaries', 
    /// 'PlayerSpeed', 'DelayedSpawnings' and 'WorldSize'. For the implementation see the script 'DataSaver').
    /// </summary>
    public Difficulty DifficultyLevel
    {
        get
        {
            HighScoresData data = RetrieveHighScoresDataFromFile();
            return data.DifficultyLevel;
        }
        set
        {
            HighScoresData data = RetrieveHighScoresDataFromFile();
            data.DifficultyLevel = value;
            SaveHighScoresDataToFile(data);
        }
    }

    /// <summary>
    /// This method retrieves the highScores data (includes the current difficulty) from an external file where it is saved.
    /// </summary>
    /// <returns>It returns an object of the type SceneInteractionData which holds all of the high-scores-specific data.
    /// The high scores data is device specific, when the high scores data is copied/moved to another device it gets overridden.</returns>
    public HighScoresData RetrieveHighScoresDataFromFile()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/difficulty.dat"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/difficulty.dat", FileMode.Open);
            HighScoresData data = (HighScoresData)bf.Deserialize(file);
            file.Close();

            if (data.DeviceSpecificNumber == SystemInfo.deviceUniqueIdentifier)
                return data;
            else
            {
                FileStream newFile = File.Create(Application.persistentDataPath + "/difficulty.dat");
                HighScoresData toBeSaved = new HighScoresData();
                bf.Serialize(newFile, toBeSaved);
                file.Close();
                return toBeSaved;
            }
        }
        else
        {
            Debug.Log("High Scores data couldn't be retrieved from the external file. Check whether the persistentDataPath " +
                "addresses the right directory");

            FileStream file = File.Create(Application.persistentDataPath + "/difficulty.dat");
            HighScoresData newHighScoresData = new HighScoresData();

            bf.Serialize(file, newHighScoresData);
            file.Close();

            return newHighScoresData;
        }
    }

    /// <summary>
    /// This method saves high scores data and the current difficulty to an external persistent file.
    /// </summary>
    /// <param name="toBeSaved">The object of the type 'HighScoresData' (which holds the information 
    /// which should be saved) to pass.</param>
    public void SaveHighScoresDataToFile(HighScoresData toBeSaved)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/difficulty.dat");

        bf.Serialize(file, toBeSaved);
        file.Close();
    }
}

public enum Difficulty { VeryEasy, Easy, Medium, Hard, VeryHard, Ultimate };

[Serializable]
public class HighScoresData
{
    private int[] scoresVeryEasy, scoresEasy, scoresMedium, scoresHard, scoresVeryHard, scoresUltimate; //the int-arrays holding the high scores
    private String deviceSpecificNumber;

    /// <summary>
    /// A number which is unique for each device, makes sure that the high scores can't be copied to
    /// other devices
    /// </summary>
    public String DeviceSpecificNumber
    {
        get { return deviceSpecificNumber; }
    }

    /// <summary>
    /// The high scores arrays are instantiated and associated with the build number of the current device. When the game is loaded on
    /// another device and played, the previous high scores are overridden (deleted and newly instantiated) and associated with the build 
    /// number of the new device.
    /// </summary>
    public HighScoresData()
    {
        scoresVeryEasy = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        scoresEasy = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        scoresMedium = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        scoresHard = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        scoresVeryHard = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        scoresUltimate = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        deviceSpecificNumber = SystemInfo.deviceUniqueIdentifier;
    }

    /// <summary>
    /// Returns one of the high scores arrays depending on the passed difficulty level.
    /// </summary>
    /// <returns>Returns a high scores list as an int array.</returns>
    /// <param name="difficultyLevel">The difficulty level of which the high scores array should be returned.</param>
    public int[] GetHighScores(Difficulty difficultyLevel)
    {
        switch (difficultyLevel)
        {
            case Difficulty.VeryEasy:
                return scoresVeryEasy;
            case Difficulty.Easy:
                return scoresEasy;
            case Difficulty.Medium:
                return scoresMedium;
            case Difficulty.Hard:
                return scoresHard;
            case Difficulty.VeryHard:
                return scoresVeryHard;
            case Difficulty.Ultimate:
                return scoresUltimate;
            default: //never occurs, is only added to avoid a compilation error
                return new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        }
    }

    /// <summary>
    /// Checks whether a passed score that was achieved is good enough for making it into the high scores list.
    /// </summary>
    /// <param name="score">The new score that will be saved (if good enough).</param>
    public void MakeNewHighScoresEntry(int score)
    {
        int[] highScores;
        switch (DifficultyLevel)
        {
            case Difficulty.VeryEasy:
                highScores = scoresVeryEasy;
                break;
            case Difficulty.Easy:
                highScores = scoresEasy;
                break;
            case Difficulty.Medium:
                highScores = scoresMedium;
                break;
            case Difficulty.Hard:
                highScores = scoresHard;
                break;
            case Difficulty.VeryHard:
                highScores = scoresVeryHard;
                break;
            case Difficulty.Ultimate:
                highScores = scoresUltimate;
                break;
            default: //never occurs, is only added to avoid a compilation error
                highScores = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                break;
        }

        for (int i = 0; i < highScores.Length; i++)
        {
            if (score > highScores[i])
            {
                highScores = NewHighScoresEntry(score, i, highScores);
                break;
            }
        }

        switch (DifficultyLevel)
        {
            case Difficulty.VeryEasy:
                scoresVeryEasy = highScores;
                break;
            case Difficulty.Easy:
                scoresEasy = highScores;
                break;
            case Difficulty.Medium:
                scoresMedium = highScores;
                break;
            case Difficulty.Hard:
                scoresHard = highScores;
                break;
            case Difficulty.VeryHard:
                scoresVeryHard = highScores;
                break;
            case Difficulty.Ultimate:
                scoresUltimate = highScores;
                break;
        }
    }

    /// <summary>
    /// Integrates a passed value at a passed position into an array.
    /// </summary>
    /// <param name="score">The score which should be integrated into the array.</param>
    /// <param name="indexInTheTable">The index at which the score should be added.</param>
    /// <param name="highScoresArray">The array to which the changes are made (and which will be returned).</param>
    /// <returns>The new array is returned.</returns>
    int[] NewHighScoresEntry(int score, int indexInTheTable, int[] highScoresArray)
    {
        for (int i = highScoresArray.Length - 1; i > indexInTheTable; i--)
            highScoresArray[i] = highScoresArray[i - 1];
        highScoresArray[indexInTheTable] = score;
        return highScoresArray;
    }

    /// <summary>
    /// Holds the current difficulty level with which the player plays. (The difficulty value depends on the settings: 'WorldBoundaries', 
    /// 'PlayerSpeed', 'DelayedSpawnings' and 'WorldSize'. For the implementation see the script 'DataSaver')
    /// </summary>
    public Difficulty DifficultyLevel
    {
        get; set;
    }
}

public static class ScoreConverter
{
    /// <summary>
    /// Converts a passed permille score to a percentage which is rounded to one decimal digit. The percentage is returned as string.
    /// </summary>
    /// <param name="permilleScore">A score as permille score (as int).</param>
    /// <returns>Returns a string.</returns>
    public static string ConvertPermilleScoreToPercentage(int permilleScore)
    {
        int percentagScore = permilleScore / 10;
        string percentageText = permilleScore < 1000 ? "" + percentagScore + "." + Mathf.RoundToInt(permilleScore - percentagScore*10) + "%" : 
            "" + 100 + "." + 0 + "%";
        return percentageText;
    }
}
