using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// This singleton provides methods/properties for getting or setting player progress data (was the game started for the first time, which custom colors were unlocked)
/// </summary>
public class PlayerProgress : Singleton<PlayerProgress>
{

    /// <summary>
    /// Get or set whether the game was loaded for the first time.
    /// </summary>
    public GameFirstLoaded IsGameFirstLoaded
    {
        get
        {
            PlayerProgressData data = RetrievePlayerProgressDataFromFile();
            return data.IsGameFirstLoaded;
        }
        set
        {
            PlayerProgressData data = RetrievePlayerProgressDataFromFile();
            data.IsGameFirstLoaded = value;
            SavePlayerProgressDataToFile(data);
        }
    }


    /// <summary>
    /// Stores the information of each customized color whether it was unlocked or is still locked. The information
    /// is stored in an external file.
    /// The index of the bool array identifies the color whose info is stored (1 = orange, 2 = dark green, 3 = faint red)
    /// </summary>
    public bool[] CustomColorUnlocked
    {
        get
        {
            PlayerProgressData data = RetrievePlayerProgressDataFromFile();
            return data.CustomColorUnlocked;
        }
        set
        {
            PlayerProgressData data = RetrievePlayerProgressDataFromFile();
            data.CustomColorUnlocked = value;
            SavePlayerProgressDataToFile(data);
        }
    }


    /// <summary>
    /// Checks if the player achieved something to unlock a custom color that is currently locked.
    /// If so, the info which colors were unlocked is persisted.
    /// <returns>Returns the lock states of all custom colors (true means a color was unlocked)</returns>
    /// </summary>
    public bool[] UnlockCustomColors()
    {
        HighScoresData data = HighScores.Instance.RetrieveHighScoresDataFromFile();
        bool[] lockedStates = CustomColorUnlocked;

        if (!lockedStates[0])
        {
            if (data.GetHighScores(Difficulty.Medium)[0] >= 400)
                lockedStates[0] = true;
        }

        if (!lockedStates[1])
        {
            if (data.GetHighScores(Difficulty.Ultimate)[0] >= 100)
                lockedStates[1] = true;
        }

        if (!lockedStates[2])
        {
            if (data.GetHighScores(Difficulty.VeryEasy)[0] >= 1000 || data.GetHighScores(Difficulty.Easy)[0] >= 1000 ||
                data.GetHighScores(Difficulty.Medium)[0] >= 1000 || data.GetHighScores(Difficulty.Hard)[0] >= 1000 ||
                data.GetHighScores(Difficulty.VeryHard)[0] >= 1000 || data.GetHighScores(Difficulty.Ultimate)[0] >= 1000)
                lockedStates[2] = true;
        }

        return CustomColorUnlocked = lockedStates;
    }


    /// <summary>
    /// This method retrieves the fullVersion data from an external file where it is saved.
    /// </summary>
    /// <returns>It returns an object of the type PlayerProgressData which holds all of the full-verion-specific data.</returns>
        public PlayerProgressData RetrievePlayerProgressDataFromFile()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/playerProgress.dat"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/playerProgress.dat", FileMode.Open);
            PlayerProgressData data = (PlayerProgressData)bf.Deserialize(file);
            file.Close();

            return data;
        }
        else
        {
            Debug.Log("Full version data couldn't be retrieved from the external file. Check whether the persistentDataPath " +
                "addresses the right directory");
            FileStream file = File.Create(Application.persistentDataPath + "/playerProgress.dat");
            PlayerProgressData newPlayerProgressData = new PlayerProgressData();

            bf.Serialize(file, newPlayerProgressData);
            file.Close();

            return newPlayerProgressData;
        }
    }

    /// <summary>
    /// This method saves full version data to an external persistent file.
    /// </summary>
    /// <param name="toBeSaved">The object of the type PlayerProgressData (which holds the information 
    /// which should be saved) to pass.</param>
    public void SavePlayerProgressDataToFile(PlayerProgressData toBeSaved)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerProgress.dat");

        bf.Serialize(file, toBeSaved);
        file.Close();
    }
}


public enum GameFirstLoaded { firstLoaded, notFirstLoaded };

[Serializable]
public class PlayerProgressData
{
    public PlayerProgressData()
    {
        IsGameFirstLoaded = GameFirstLoaded.firstLoaded;
        CustomColorUnlocked = new bool[] { false, false, false };
    }


    /// <summary>
    /// Get or set whether the game was loaded for the first time.
    /// </summary>
    public GameFirstLoaded IsGameFirstLoaded
    { get; set; }


    /// <summary>
    /// Only needed if IAPs are disabled. Stores the information of each customized color whether it was unlocked or is still locked.
    /// The index of the bool array identifies the color whose info is stored (1 = orange, 2 = dark green, 3 = faint red)
    /// </summary>
    public bool[] CustomColorUnlocked
    { get; set; }
}
