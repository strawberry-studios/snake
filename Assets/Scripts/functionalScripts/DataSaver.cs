using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class DataSaver : Singleton<DataSaver>
{
    /// <summary>
    /// Saves the scoreAsPercentage (if it is a new highscore) to an external file.
    /// </summary>
    /// <param name="score">score (as percentage) as int to pass.</param>
    public void SaveNewHighscore(int highscore)
    {
        PlayerData data = RetrievePlayerDataFromFile();
        data.SetHighscore(highscore);

        SavePlayerDataToFile(data);
    }

    /// <summary>
    /// Saves the new size of the world to an external file.
    /// </summary>
    /// <param name="newWorldSize">The new size of the world as int to pass.</param>
    public void SaveNewWorldSize(int newWorldSize)
    {
        PlayerData data = RetrievePlayerDataFromFile();
        data.SetWorldSize(newWorldSize);

        SavePlayerDataToFile(data);
    }

    /// <summary>
    /// Saves the speed of the player to an external file.
    /// </summary>
    /// <param name="newPlayerSpeed">The new speed of the player as int to pass.</param>
    public void SavePlayerSpeed(int newPlayerSpeed)
    {
        PlayerData data = RetrievePlayerDataFromFile();
        data.SetPlayerSpeed(newPlayerSpeed);

        SavePlayerDataToFile(data);
    }

    /// <summary>
    /// Saves the state of the world boundaries as bool (i.e. true or  false) to an external file.
    /// </summary>
    /// <param name="worldBoundariesOn">The new worldBoundaries state as bool to pass.</param>
    public void SaveWorldBoundariesToggleState(bool worldBoundariesOn)
    {
        PlayerData data = RetrievePlayerDataFromFile();
        data.ToggleWorldBoundaries(worldBoundariesOn);

        SavePlayerDataToFile(data);
    }

    /// <summary>
    /// Saves the showPixels state as bool (i.e. true or false) to an external file.
    /// If true the objects (i.e. the snake and the world) should be displayed pixeled.
    /// </summary>
    /// <param name="newShowPixelsState">The new showPixels state as bool to pass.</param>
    public void SetShowPixels(bool newShowPixelsState)
    {
        PlayerData data = RetrievePlayerDataFromFile();
        data.ShowPixels(newShowPixelsState);

        SavePlayerDataToFile(data);
    }

    /// <summary>
    /// Returns the highscore as percentage (datatype: int) from an external file
    /// </summary>
    public int GetHighscore()
    {
        PlayerData data = RetrievePlayerDataFromFile();
        if (data != null)
            return data.GetHighscore();
        else
        {
            Debug.Log("The highscore couldn't be retrieved from the external playerInfo file.");
            return 0;
        }
    }

    /// <summary>
    /// Returns the world's size as an int from an external file.
    /// </summary>
    public int GetWorldSize()
    {
        PlayerData data = RetrievePlayerDataFromFile();
        if (data != null)
            return data.GetWorldSize();
        else
        {
            Debug.Log("The world size couldn't be retrieved from the external playerInfo file.");
            return 6;
        }
    }

    /// <summary>
    /// Returns the speed of the player as an int from an external file.
    /// </summary>
    public int GetPlayerSpeed()
    {
        PlayerData data = RetrievePlayerDataFromFile();
        if (data != null)
            return data.GetPlayerSpeed();
        else
        {
            Debug.Log("The speed of the player couldn't be retrieved from the external playerInfo file.");
            return 0;
        }
    }

    /// <summary>
    /// Returns the current state of the world boundaries as bool from an external file.
    /// </summary>
    public bool GetWorldBoundariesState()
    {
        PlayerData data = RetrievePlayerDataFromFile();
        if (data != null)
            return data.GetWorldBoundariesState();
        else
        {
            Debug.Log("The worldBoundaries state couldn't be retrieved from the external playerInfo file.");
            return false;
        }
    }

    /// <summary>
    /// Returns the current state of the showPixels variable as bool from an external file.
    /// If true the objects (i.e. the snake and the world) should be displayed pixeled.
    /// </summary>
    public bool GetShowPixels()
    {
        PlayerData data = RetrievePlayerDataFromFile();
        if (data != null)
            return data.GetShowPixels();
        else
        {
            Debug.Log("The showPixels state couldn't be retrieved from the external playerInfo file.");
            return false;
        }
    }

    /// <summary>
    /// This method retrieves the playerData from an external file to which it is saved.
    /// </summary>
    /// <returns>It returns an object of the type PlayerData which holds all of the player-specific data.</returns>
    public PlayerData RetrievePlayerDataFromFile()
    {
        if (File.Exists(Application.persistentDataPath + "/playerinfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerinfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            return data;
        }
        else
        {
            Debug.Log("Data couldn't be retrieved from file. Check whether the persistentDataPath " +
                "addresses the right directory");
            return new PlayerData();
        }
    }

    /// <summary>
    /// This method saves player data to an external persistent file.
    /// </summary>
    /// <param name="toBeSaved">The object of the type PlayerData (which holds the information 
    /// which should be saved) to pass.</param>
    private void SavePlayerDataToFile(PlayerData toBeSaved)
    {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/playerinfo.dat");

            bf.Serialize(file, toBeSaved);
            file.Close();
    }
}

[Serializable]
public class PlayerData
{
    int highscore, worldSize, speed;
    bool worldBoundaries, showPixels;

    /// <summary>
    /// Assigns a new value to the highscore variable.
    /// </summary>
    /// <param name="passedNewHighscore">new highscore as int to pass</param>
    public void SetHighscore(int passedNewHighscore)
    {
        highscore = passedNewHighscore;
    }

    /// <summary>
    /// The new size of the world is set. (The value corresponds to the number of columns...perhaps).
    /// </summary>
    /// <param name="newWorldSize">The new size of the world as int to pass.</param>
    public void SetWorldSize(int newWorldSize)
    {
        worldSize = newWorldSize;
    }

    /// <summary>
    /// The speed of the player is set.
    /// </summary>
    /// <param name="newPlayerSpeed">The new speed of the player as int to pass.</param>
    public void SetPlayerSpeed(int newPlayerSpeed)
    {
        speed = newPlayerSpeed;
    }

    /// <summary>
    /// The world boundaries are toggled on or off.
    /// </summary>
    /// <param name="worldBoundariesOn">New state of the world boundaries as bool to pass</param>
    public void ToggleWorldBoundaries(bool worldBoundariesOn)
    {
        worldBoundaries = worldBoundariesOn;
    }

    /// <summary>
    /// Sets show pixels.
    /// </summary>
    /// <param name="showPixelsOn">New state as bool to pass.</param>
    public void ShowPixels(bool showPixelsOn)
    {
        showPixels = showPixelsOn;
    }

    /// <summary>
    /// Returns the current highscore held by a PlayerData object as int.
    /// </summary>
    public int GetHighscore()
    {
        return highscore;
    }

    /// <summary>
    /// Returns the world's size as an int.
    /// </summary>
    public int GetWorldSize()
    {
        if (worldSize != 0)
            return worldSize;
        else
            return 6;
    }

    /// <summary>
    /// Returns the speed of the player as an int.
    /// </summary>
    public int GetPlayerSpeed()
    {
        return speed;
    }

    /// <summary>
    /// Returns the current state of the world boundaries as bool.
    /// </summary>
    public bool GetWorldBoundariesState()
    {
        return worldBoundaries;
    }

    /// <summary>
    /// Returns the current state of showPixels as a bool.
    /// </summary>
    public bool GetShowPixels()
    {
        return showPixels;
    }
}
