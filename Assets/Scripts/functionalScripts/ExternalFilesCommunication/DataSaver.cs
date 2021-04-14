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
    /// The index of the current 'collectAppleSound' from/to an external file (ranging from 0 to 5, the sound clips are also numbered like that).
    /// </summary>
    public int currentCollectAppleSound
    {
        get
        {
            PlayerData data = RetrievePlayerDataFromFile();
            return data.currentCollectAppleSound;
        }
        set
        {
            PlayerData data = RetrievePlayerDataFromFile();
            data.currentCollectAppleSound = value;
            SavePlayerDataToFile(data);
        }
    }

    /// <summary>
    /// The scene from which the 'customize sounds' scene was opened at last. Retrieved from or saved to an external file.
    /// </summary>
    public LastEditedSound LastEditedSound {
        get
        {
            PlayerData data = RetrievePlayerDataFromFile();
            return data.LastEditedSound;
        }
        set
        {
            PlayerData data = RetrievePlayerDataFromFile();
            data.LastEditedSound = value;
            SavePlayerDataToFile(data);
        }
    }

    /// <summary>
    /// The customized colors (saved to or retrieved from an external file). 
    /// Index one means which color, index two the r,g,b,a components of the color from 0 to 255.
    /// </summary>
    public int[,] ColorsCustomized
    {
        get
        {
            PlayerData data = RetrievePlayerDataFromFile();
            return data.ColorsCustomized;
        }
        set
        {
            PlayerData data = RetrievePlayerDataFromFile();
            data.ColorsCustomized = value;
            SavePlayerDataToFile(data);
        }
    }

    /// <summary>
    /// The colors which were customized at last. (Saved to or retrieved from an external file). Either the snake or the collectable colors.
    /// </summary>
    public LastEditedColor LastCustomizedColor
    {
        get
        {
            PlayerData data = RetrievePlayerDataFromFile();
            return data.LastCustomizedColor;
        }
        set
        {
            PlayerData data = RetrievePlayerDataFromFile();
            data.LastCustomizedColor = value;
            SavePlayerDataToFile(data);
        }
    }

    /// <summary>
    /// Get or set whether the sound should be activated or not. The information is saved to or retrieved from an external file.
    /// </summary>
    public bool SoundOn
    {
        get
        {
            PlayerData data = RetrievePlayerDataFromFile();
            return data.SoundOn;
        }
        set
        {
            PlayerData data = RetrievePlayerDataFromFile();
            data.SoundOn = value;
            SavePlayerDataToFile(data);
        }
    }

    /// <summary>
    /// Get or set whether the vibrations should be activated or not. The information is saved to or retrieved from an external file.
    /// </summary>
    public bool VibrationsOn
    {
        get
        {
            PlayerData data = RetrievePlayerDataFromFile();
            return data.VibrationsOn;
        }
        set
        {
            PlayerData data = RetrievePlayerDataFromFile();
            data.VibrationsOn = value;
            SavePlayerDataToFile(data);
        }
    }

    /// <summary>
    /// Get or set the value of the speed slider in the 'difficulty' scene. It is saved to or retrieved from an external file. 
    /// Note: The actual player speed and the speed slider value are not the same. 
    /// Note 2: The new actual speed of the snake is determined depending on the new speedSlider and worldSize value.
    /// </summary>
    public int SpeedSliderValue
    { get
        {
            PlayerData data = RetrievePlayerDataFromFile();
            return data.SpeedSliderValue;
        }
      set
        {
            PlayerData data = RetrievePlayerDataFromFile();
            data.SpeedSliderValue = value;
            SavePlayerDataToFile(data);
        }
    }

    /// <summary>
    /// Get or set the current controls mode. The information is saved to or retrieved from an external file.
    /// </summary>
    public ControlsMode ControlsModeActivated
    {
        get
        {
            PlayerData data = RetrievePlayerDataFromFile();
            return data.ControlsModeActivated;
        }
        set
        {
            PlayerData data = RetrievePlayerDataFromFile();
            data.ControlsModeActivated = value;
            SavePlayerDataToFile(data);
        }
    }

    /// <summary>
    /// The sensitivity with which the swipes (to control the snake) are processed. Saved to/retrieved from an external file.
    /// </summary>
    public float SwipesSensitivity
    {
        get
        {
            PlayerData data = RetrievePlayerDataFromFile();
            return data.SwipesSensitivity;
        }
        set
        {
            PlayerData data = RetrievePlayerDataFromFile();
            data.SwipesSensitivity = value;
            SavePlayerDataToFile(data);
        }
    }

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
    /// The new speed of the snake is determined depending on the speedSlider and new worldSize value.
    /// </summary>
    /// <param name="newWorldSize">The new size of the world as int to pass.</param>
    public void SaveNewWorldSize(int newWorldSize)
    {
        PlayerData data = RetrievePlayerDataFromFile();
        data.SetWorldSize(newWorldSize);

        SavePlayerDataToFile(data);
    }

    ///// <summary>
    ///// Saves the speed of the player to an external file.
    ///// </summary>
    ///// <param name="newPlayerSpeed">The new speed of the player as int to pass.</param>
    //public void SavePlayerSpeed(int newPlayerSpeed)
    //{
    //    PlayerData data = RetrievePlayerDataFromFile();
    //    data.SetPlayerSpeed(newPlayerSpeed);

    //    SavePlayerDataToFile(data);
    //}

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
    /// Saves the state of the delayed spawnings as bool (i.e. true or  false) to an external file.
    /// </summary>
    /// <param name="delayedSpawningsOn">The new delayedSpawnings state as bool to pass.</param>
    public void SaveDelayedSpawningsToggleState(bool delayedSpawningsOn)
    {
        PlayerData data = RetrievePlayerDataFromFile();
        data.ToggleDelayedSpawnings(delayedSpawningsOn);

        SavePlayerDataToFile(data);
    }

    /// <summary>
    /// Saves the state of funMode as bool (i.e. true or  false) to an external file.
    /// </summary>
    /// <param name="delayedSpawningsOn">The new funMode state as bool to pass.</param>
    public void SaveFunModeToggleState(bool funModeOn)
    {
        PlayerData data = RetrievePlayerDataFromFile();
        data.ToggleFunMode(funModeOn);

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
    /// Saves the showGridLines state as bool (i.e. true or false) to an external file.
    /// If true the gridLines of the world shouldn't be visible.
    /// </summary>
    /// <param name="newShowGridLinesState">The new showGridLines state as bool to pass.</param>
    public void SetShowGridLines(bool newShowGridLinesState)
    {
        PlayerData data = RetrievePlayerDataFromFile();
        data.ShowGridLines(newShowGridLinesState);

        SavePlayerDataToFile(data);
    }

    /// <summary>
    /// Sets whether the head of the snake is marked or not. If marked, it is colored in a darker shade than the other blocks of the snake.
    /// </summary>
    /// <param name="newSnakeHeadMarked">New state whether the snake head should be marked or not (as bool).</param>
    public void SetSnakeHeadMarked(bool newSnakeHeadMarked)
    {
        PlayerData data = RetrievePlayerDataFromFile();
        data.ToggleSnakeHeadMarked(newSnakeHeadMarked);

        SavePlayerDataToFile(data);
    }

    /// <summary>
    /// Saves a passed color to an external file. The snake (in the game) and other objects in the menu will be displayed in this color.
    /// </summary>
    /// <param name="newColor">The new Color object to pass.</param>
    public void SetSnakeColor(Color newColor)
    {
        PlayerData data = RetrievePlayerDataFromFile();
        data.SetSnakeColor(newColor);

        SavePlayerDataToFile(data);
    }

    /// <summary>
    /// Saves a passed color to an external file. The snake (in the game) and other objects in the menu will be displayed in this color.
    /// </summary>
    /// <param name="r">The red component of the color.</param>
    /// <param name="g">The green component of the color.</param>
    /// <param name="b">The blue component of the color.</param>
    /// <param name="a">The alpha component of the color.</param>
    public void SetSnakeColor(int r, int g, int b, int a)
    {
        PlayerData data = RetrievePlayerDataFromFile();
        data.SetSnakeColor(r, g, b, a);

        SavePlayerDataToFile(data);
    }

    /// <summary>
    /// Saves a passed color to an external file. The collectables (in the game) and other objects in the menu will be displayed in this color.
    /// </summary>
    /// <param name="newColor">The new Color object to pass.</param>
    public void SetCollectablesColor(Color newColor)
    {
        PlayerData data = RetrievePlayerDataFromFile();
        data.SetCollectablesColor(newColor);

        SavePlayerDataToFile(data);
    }

    /// <summary>
    /// Saves a passed color to an external file. The collectables (in the game) and other objects in the menu will be displayed in this color.
    /// </summary>
    /// <param name="r">The red component of the color.</param>
    /// <param name="g">The green component of the color.</param>
    /// <param name="b">The blue component of the color.</param>
    /// <param name="a">The alpha component of the color.</param>
    public void SetCollectablesColor(int r, int g, int b, int a)
    {
        PlayerData data = RetrievePlayerDataFromFile();
        data.SetCollectablesColor(r, g, b, a);

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
    /// Returns the current state of delayed spawninfgs as a bool from an external file.
    /// </summary>
    public bool GetDelayedSpawningsState()
    {
        PlayerData data = RetrievePlayerDataFromFile();
        if (data != null)
            return data.GetDelayedSpawningsState();
        else
        {
            Debug.Log("The delayedSpawnings state couldn't be retrieved from the external playerInfo file.");
            return false;
        }
    }

    /// <summary>
    /// Returns the current state of delayed spawninfgs as a bool from an external file.
    /// </summary>
    public bool GetFunModeState()
    {
        PlayerData data = RetrievePlayerDataFromFile();
        if (data != null)
            return data.GetFunModeState();
        else
        {
            Debug.Log("The funMode state couldn't be retrieved from the external playerInfo file.");
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
    /// Returns the current state of the showGridLines variable as bool from an external file.
    /// If true the gridLines of the world should not be visible.
    /// </summary>
    public bool GetShowGridLines()
    {
        PlayerData data = RetrievePlayerDataFromFile();
        if (data != null)
            return data.GetShowGridLines();
        else
        {
            Debug.Log("The showGridLines state couldn't be retrieved from the external playerInfo file.");
            return true;
        }
    }

    /// <summary>
    /// Returns whether the head of the snake is marked or not. If marked, it is colored in a darker shade than the other blocks of the snake.
    /// </summary>
    public bool GetSnakeHeadMarked()
    {
        PlayerData data = RetrievePlayerDataFromFile();
        if (data != null)
            return data.GetSnakeHeadMarked();
        else
        {
            Debug.Log("The snakeHeadMarked state couldn't be retrieved from the external playerInfo file. By default the snake head is marked.");
            return true;
        }
    }

    /// <summary>
    /// Returns the current color of the snake. If it can't be retrieved, green is returned by default.
    /// </summary>
    /// <returns></returns>
    public int[] GetSnakeColor()
    {
        PlayerData data = RetrievePlayerDataFromFile();
        if (data != null)
            return data.GetSnakeColor();
        else
        {
            Debug.Log("The current color of the snake couldn't be retrieved from the external playerInfo file. It is set to green by default.");
            return Color.green.ConvertColorToIntArray();
        }
    }

    /// <summary>
    /// Returns a color that is similar to the color of the snake, yet which has a slightly different shade.
    /// Note: This method only works when the RGB values it uses (see the switch statement) correspond to the colors of the color-select-objects
    /// in the snake-color-skins-scene.
    /// </summary>
    /// <returns></returns>
    public int[] GetSnakeHeadColor()
    {
        PlayerData data = RetrievePlayerDataFromFile();
        if (data != null)
            return data.GetSnakeHeadColor();
        else
        {
            Debug.Log("The current color of the snake head couldn't be retrieved from the external playerInfo file. It is set to green by default.");
            return Color.green.ConvertColorToIntArray();
        }
    }

    /// <summary>
    /// Returns a color that is similar to the color of the snake, yet which has a slightly different shade.
    /// The shade of the color is more significantly different from the actual snake color than if using "GetSnakeHeadColor". 
    /// Note: This method only works when the RGB values it uses (see the switch statement) correspond to the colors of the color-select-objects
    /// in the snake-color-skins-scene.
    /// </summary>
    /// <returns></returns>
    public int[] GetSnakeHeadPixeledColor()
    {
        PlayerData data = RetrievePlayerDataFromFile();
        if (data != null)
            return data.GetSnakeHeadPixeledColor();
        else
        {
            Debug.Log("The current color of the snake head couldn't be retrieved from the external playerInfo file. It is set to green by default.");
            return Color.green.ConvertColorToIntArray();
        }
    }

    /// <summary>
    /// Returns the current color of the collectables. If it can't be retrieved, green is returned by default.
    /// </summary>
    /// <returns></returns>
    public int[] GetCollectablesColor()
        {
            PlayerData data = RetrievePlayerDataFromFile();
            if (data != null)
                return data.GetCollectablesColor();
            else
            {
                Debug.Log("The current color of the collectables couldn't be retrieved from the external playerInfo file. It is set to red by default.");
                int[] red = new int[4];
                red[0] = (int)Color.red.r;
                red[1] = (int)Color.red.g;
                red[2] = (int)Color.red.b;
                red[3] = (int)Color.red.a;
                return red;
            }
        }

    /// <summary>
    /// This method retrieves the playerData from an external file to which it is saved.
    /// </summary>
    /// <returns>It returns an object of the type PlayerData which holds all of the player-specific data.</returns>
    public PlayerData RetrievePlayerDataFromFile()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/playerinfo.dat"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/playerinfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            return data;
        }
        else
        {
            Debug.Log("Data couldn't be retrieved from file. Check whether the persistentDataPath " +
                "addresses the right directory");

            FileStream file = File.Create(Application.persistentDataPath + "/playerinfo.dat");
            PlayerData newPlayerData = new PlayerData();

            bf.Serialize(file, newPlayerData);
            file.Close();

            return newPlayerData;
        }
    }

    /// <summary>
    /// This method saves player data to an external persistent file.
    /// </summary>
    /// <param name="toBeSaved">The object of the type PlayerData (which holds the information 
    /// which should be saved) to pass.</param>
    public void SavePlayerDataToFile(PlayerData toBeSaved)
    {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/playerinfo.dat");

            bf.Serialize(file, toBeSaved);
            file.Close();
    }
}
/// <summary>
/// The controls-mode with which the snake is controlled.
/// </summary>
public enum ControlsMode { buttonsOnly, swipeOnly, keypad, buttonsAndSwipe };
/// <summary>
/// The scene from which the 'customize colors' scene was opened at last.
/// </summary>
public enum LastEditedColor { snake, collectable };
/// <summary>
/// The scene from which the 'customizeSounds' scene was opened at last.
/// </summary>
public enum LastEditedSound { preferences, moreOptions}

/// <summary>
/// This class provides methods and properties for saving and getting the player preferences. The data is stored as a serialized object.
/// For serialization, see singleton 'DataSaver' and the method 'SavePlayerDataToFile'.
/// For the conversion of worldSize and the speedSliderValue into a certain actual snake speed, see the method 'SetSnakeSpeed()'.
/// For the conditions how the different difficulty levels are determined, see 'DetermineNewDifficulty()'.
/// </summary>
[Serializable]
public class PlayerData
{
    int highscore, worldSize, speed;
    int speedSliderValue;
    bool worldBoundaries, showPixels, delayedSpawnings, funMode, snakeHeadMarked, showGridLines;
    int[] snakePlayingColor, collectableColor; //arrays (of the length 4) holding the r,g,b,a values of a color

    /// <summary>
    /// Constructor with the default player settings.
    /// </summary>
    public PlayerData()
    {
        snakePlayingColor = new int[4];
        collectableColor = new int[4];
        SetWorldSize(12);
        SpeedSliderValue = 5;
        ColorsCustomized = new int [3, 4];
        SaveCustomizedColors();

        SetHighscore(0);
        ToggleWorldBoundaries(false);
        ShowPixels(false);
        ToggleDelayedSpawnings(false);
        ToggleFunMode(false);
        SetCollectablesColor(Color.red);
        SetSnakeColor(Color.green); 
        ToggleSnakeHeadMarked(true);
        ShowGridLines(false);
        SoundOn = true;
        VibrationsOn = false;
        SwipesSensitivity = 0.5f;
        LastCustomizedColor = LastEditedColor.snake;
        ControlsModeActivated = ControlsMode.buttonsOnly;
    }

    /// <summary>
    /// Sets default customizable colors.
    /// </summary>
    void SaveCustomizedColors()
    {
        //color 1 - orange:
        ColorsCustomized[0, 0] = 245;
        ColorsCustomized[0, 1] = 109;
        ColorsCustomized[0, 2] = 0;
        ColorsCustomized[0, 3] = 255;

        //color 2 - dark green:
        ColorsCustomized[1, 0] = 115;
        ColorsCustomized[1, 1] = 125;
        ColorsCustomized[1, 2] = 34;
        ColorsCustomized[1, 3] = 255;

        //color 3 - faint red:
        ColorsCustomized[2, 0] = 186;
        ColorsCustomized[2, 1] = 69;
        ColorsCustomized[2, 2] = 69;
        ColorsCustomized[2, 3] = 255;
    }

    /// <summary>
    /// The index of the current 'collectAppleSound' (ranging from 0 to 5, the sound clips are also numbered like that).
    /// </summary>
    public int currentCollectAppleSound
    {
        get; set;
    }

    /// <summary>
    /// The scene from which the 'customize sounds' scene was opened at last.
    /// </summary>
    public LastEditedSound LastEditedSound { get; set; }

    /// <summary>
    /// The customized colors. Index one means which color, index two the r,g,b,a components of the color from 0 to 255.
    /// </summary>
    public int[,] ColorsCustomized
    {
        get; set;
    }

    /// <summary>
    /// The colors which were customized at last. Either the snake or the collectable colors.
    /// </summary>
    public LastEditedColor LastCustomizedColor
    {
        get; set;
    }

    /// <summary>
    /// Get or set whether the sound should be activated or not.
    /// </summary>
    public bool SoundOn
    {
        get; set;
    }

    /// <summary>
    /// Get or set whether the vibrations should be activated or not.
    /// </summary>
    public bool VibrationsOn
    {
        get; set;
    }

    /// <summary>
    /// Get or set the value of the speed slider in the 'difficulty' scene.
    /// Note: The actual player speed and the speed slider value are not the same. 
    /// </summary>
    public int SpeedSliderValue
    { get
        {
            return speedSliderValue;
        }
      set
        {
            speedSliderValue = value;
            SetSnakeSpeed();
        }
    }

    /// <summary>
    /// Get or set the current controls mode.
    /// </summary>
    public ControlsMode ControlsModeActivated
    {
        get; set;
    }

    /// <summary>
    /// The sensitivity with which the swipes (to control the snake) are processed. 
    /// </summary>
    public float SwipesSensitivity
    {
        get; set;
    }

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
        SetSnakeSpeed();
    }

    /// <summary>
    /// The speed of the player is set. It can only be changed when the speed slider value or the world size value is altered.
    /// </summary>
    void SetSnakeSpeed()
    {
        int minimalSpeed = 3; //the minimal speed is always 3 [units/unitOfTime]
        int maximalSpeed = Mathf.RoundToInt((worldSize * 20 / 24f) + 5); //the maximal speed is 10 for a world Size of 6 and 30 for a world Size of 30
                                                           //The relation of speed to world size is linear.
        float deltaSpeed = (maximalSpeed - minimalSpeed) / 15f; //the difference between max and min speed is divided in 15 units 
        // so that each speedSlider value (there are 16) can easily be assigned an actual snake speed
        speed = Mathf.RoundToInt((minimalSpeed + speedSliderValue * deltaSpeed)); //the actual speed
        DetermineNewDifficulty();
    }

    /// <summary>
    /// The world boundaries are toggled on or off.
    /// </summary>
    /// <param name="worldBoundariesOn">New state of the world boundaries as bool to pass</param>
    public void ToggleWorldBoundaries(bool worldBoundariesOn)
    {
        worldBoundaries = worldBoundariesOn;
        DetermineNewDifficulty();
    }

    /// <summary>
    /// Sets whether the head of the snake should be marked or not. If marked, it is colored in a darker shade than the other blocks of the snake.
    /// </summary>
    /// <param name="newSnakeHeadMarked">New state whether the snake head should be marked or not (as bool).</param>
    public void ToggleSnakeHeadMarked(bool newSnakeHeadMarked)
    {
        snakeHeadMarked = newSnakeHeadMarked;
    }

    /// <summary>
    /// The delayed spawnings are toggled on or off.
    /// </summary>
    /// <param name="delayedSpawningsOn">New state of the delayedSpawnings as bool to pass</param>
    public void ToggleDelayedSpawnings(bool delayedSpawningsOn)
    {
        delayedSpawnings = delayedSpawningsOn;
        DetermineNewDifficulty();
    }

    /// <summary>
    /// The fun mode is toggled on or off.
    /// </summary>
    /// <param name="funModeOn"></param>
    public bool ToggleFunMode(bool funModeOn)
    {
        return funMode = funModeOn;
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
    /// Sets show grid lines.
    /// </summary>
    /// <param name="showGridLines">New state as bool to pass.</param>
    public void ShowGridLines(bool newShowGridLines)
    {
        showGridLines = newShowGridLines;
    }

    /// <summary>
    /// Sets the new color of the snake.
    /// </summary>
    /// <param name="newColor">The new color as a Color to pass.</param>
    public void SetSnakeColor(Color newColor)
    {
        snakePlayingColor[0] = (int)(newColor.r * 255);
        snakePlayingColor[1] = (int)(newColor.g * 255);
        snakePlayingColor[2] = (int)(newColor.b * 255);
        snakePlayingColor[3] = (int)(newColor.a * 255);
    }

    /// <summary>
    /// Sets the new color of the snake.
    /// </summary>
    /// <param name="r">The red component of the color.</param>
    /// <param name="g">The green component of the color.</param>
    /// <param name="b">The blue component of the color.</param>
    /// <param name="a">The alpha component of the color.</param>
    public void SetSnakeColor(int r, int g, int b, int a)
    {
        snakePlayingColor[0] = r;
        snakePlayingColor[1] = g;
        snakePlayingColor[2] = b;
        snakePlayingColor[3] = a;
    }

    /// <summary>
    /// Sets the new color of the collectables.
    /// </summary>
    /// <param name="newColor">The new collectables color as a Color to pass.</param>
    public void SetCollectablesColor(Color newColor)
    {
        collectableColor[0] = (int)(newColor.r * 255);
        collectableColor[1] = (int)(newColor.g * 255);
        collectableColor[2] = (int)(newColor.b * 255);
        collectableColor[3] = (int)(newColor.a * 255);
    }

    /// <summary>
    /// Sets the new color of the collectables.
    /// </summary>
    /// <param name="r">The red component of the color.</param>
    /// <param name="g">The green component of the color.</param>
    /// <param name="b">The blue component of the color.</param>
    /// <param name="a">The alpha component of the color.</param>
    public void SetCollectablesColor(int r, int g, int b, int a)
    {
        collectableColor[0] = r;
        collectableColor[1] = g;
        collectableColor[2] = b;
        collectableColor[3] = a;
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
    /// Returns the current state of the delayedSpawnings as bool.
    /// </summary>
    public bool GetDelayedSpawningsState()
    {
        return delayedSpawnings;
    }

    /// <summary>
    /// Returns the current state of funMode as bool.
    /// </summary>
    public bool GetFunModeState()
    {
        try
        {
            Console.Out.WriteLine("Why is this working? " + funMode);
            return funMode;
        }
        //exceptions are caught because this variable was added in an update -> the data of player's who updated their app may not have had the fun mode yet, it needs to be allocated first
        catch (NullReferenceException ex)
        {
            Console.Out.WriteLine(ex.ToString());
            return ToggleFunMode(false);
        }
    }

    /// <summary>
    /// Returns the current state of showPixels as a bool.
    /// </summary>
    public bool GetShowPixels()
    {
        return showPixels;
    }

    /// <summary>
    /// Returns the current state of showGridLines as a bool.
    /// </summary>
    public bool GetShowGridLines()
    {
        return showGridLines;
    }

    /// <summary>
    /// Returns whether the head of the snake should be marked or not. 
    /// If marked, it is colored in a darker shade than the other blocks of the snake.
    /// </summary>
    public bool GetSnakeHeadMarked()
    {
        return snakeHeadMarked;
    }

    /// <summary>
    /// Returns the current color of the snake as a Color.
    /// </summary>
    /// <returns></returns>
    public int[] GetSnakeColor()
    {
        return snakePlayingColor;
    }

    /// <summary>
    /// Returns a color that is similar to the color of the snake, yet which has a slightly different shade.
    /// Note: This method only works when the RGB values it uses (see the switch statement) correspond to the colors of the color-select-objects
    /// in the snake-color-scene.
    /// </summary>
    /// <returns></returns>
    public int[] GetSnakeHeadColor()
    {
        Color c = snakePlayingColor.ConvertIntArrayIntoColor();

        return c.IsColorEqualRGB(Color.green) ? new int[] { 0, 100, 0, 255} :
               c.IsColorEqualRGB(Color.red) ? new int[] { 100, 0, 0, 255 } :
               c.IsColorEqualRGB(Color.blue) ? new int[] { 0, 80, 255, 255 } :
               c.IsColorEqualRGB(new Color(1, 1, 0, 1)) ? new int[] { 250, 125, 0, 255 } :
               c.IsColorEqualRGB(new Color(1, 0, 1, 1)) ? new int[] { 100, 0, 170, 255 } :
               c.IsColorEqualRGB(Color.cyan) ? new int[] { 0, 140, 255, 255 } :
               //c.IsColorEqualRGB(new Color(245/255f, 109/255f, 0, 1)) ? new int[] { 255, 67, 0, 255 } :
               //c.IsColorEqualRGB(new Color(115/255f, 125/255f, 34/255f, 1)) ? new int[] { 200, 110, 40, 255 } :
               //c.IsColorEqualRGB(new Color(186/255f, 69/255f, 69/255f, 1)) ? new int[] { 135, 20, 10, 255 } : new int[] { 0, 100, 0, 255 };
               DetermineCustomSnakeHeadColor();
    }

    /// <summary>
    /// Returns a color that is similar to the color of the snake, yet which has a slightly different shade.
    /// The shade of the color is more significantly different from the actual snake color than if using "GetSnakeHeadColor". 
    /// Note: This method only works when the RGB values it uses (see the switch statement) correspond to the colors of the color-select-objects
    /// in the snake-color-skins-scene.
    /// </summary>
    /// <returns></returns>
    public int[] GetSnakeHeadPixeledColor()
    {
        Color c = snakePlayingColor.ConvertIntArrayIntoColor();

        return c.IsColorEqualRGB(Color.green) ? new int[] { 0, 110, 0, 255 } :
               c.IsColorEqualRGB(Color.red) ? new int[] { 110, 0, 0, 255 } :
               c.IsColorEqualRGB(Color.blue) ? new int[] { 0, 90, 255, 255 } :
               c.IsColorEqualRGB(new Color(1, 1, 0, 1)) ? new int[] { 250, 115, 0, 255 } :
               c.IsColorEqualRGB(new Color(1, 0, 1, 1)) ? new int[] { 90, 0, 170, 255 } :
               c.IsColorEqualRGB(Color.cyan) ? new int[] { 0, 130, 255, 255 } :
               //c.IsColorEqualRGB(new Color(245/255f, 109/255f, 0, 1)) ? new int[] { 255, 67, 0, 255 } :
               //c.IsColorEqualRGB(new Color(115/255f, 125/255f, 34/255f, 1)) ? new int[] { 200, 110, 40, 255 } :
               //c.IsColorEqualRGB(new Color(186/255f, 69/255f, 69/255f, 1)) ? new int[] { 135, 20, 10, 255 } : new int[] { 0, 100, 0, 255 };
               DetermineCustomSnakeHeadColorPixeled();
    }

    /// <summary>
    /// Determines a darker or brighter shade of the snake color in which the snake head will be marked if 'snakeHeadMarked' is on.
    /// These custom snake head colors are only used if the snake color is also a customized color (only if the full version was unlocked).
    /// </summary>
    /// <returns></returns>
    int[] DetermineCustomSnakeHeadColor()
    {
        int r = snakePlayingColor[0];
        int g = snakePlayingColor[1];
        int b = snakePlayingColor[2];

        if (r >= 80 || g >= 80 || b >= 80)
            return ReturnDarkerShadeColor();
        else if (r > g && r > b)
            return ReturnMuchBrighterShadeColor(0);
        else if (g > r && g > b)
            return ReturnMuchBrighterShadeColor(1);
        else if (b > r && b > g)
            return ReturnMuchBrighterShadeColor(2);
        else
            return ReturnSlightlyBrighterShade();
    }

    /// <summary>
    /// Determines a darker or brighter shade of the snake color in which the snake head will be marked if 'snakeHeadMarked' is on.
    /// The color is significantly different from the actual snake color, so that you can notice the difference even if using the pixel mode.
    /// These custom snake head colors are only used if the snake color is also a customized color (only if the full version was unlocked).
    /// </summary>
    /// <returns></returns>
    int[] DetermineCustomSnakeHeadColorPixeled()
    {
        int r = snakePlayingColor[0];
        int g = snakePlayingColor[1];
        int b = snakePlayingColor[2];

        if (r >= 80 || g >= 80 || b >= 80)
            return ReturnDarkerShadeColorPixeled();
        else if (r > g && r > b)
            return ReturnMuchBrighterShadeColorPixeled(0);
        else if (g > r && g > b)
            return ReturnMuchBrighterShadeColorPixeled(1);
        else if (b > r && b > g)
            return ReturnMuchBrighterShadeColorPixeled(2);
        else
            return ReturnSlightlyBrighterShadePixeled();
    }

    /// <summary>
    /// Returns a player color where the intensity of each of the RGB values is reduced by 30 percents.
    /// </summary>
    /// <returns>Returns an int array.</returns>
    int[] ReturnDarkerShadeColor()
    {
        int[] array = snakePlayingColor;
        for(int i = 0; i <= 2; i++)
            snakePlayingColor[i] = (int) (0.7f * snakePlayingColor[i]);
        return array;
    }

    /// <summary>
    /// Returns a player color where the intensity of each of the RGB values is increased. The main component (R,G or B component with the highest 
    /// intensity) is increased by 20 (each intensity ranges from 0 to 255), the other components are increased by 5.
    /// </summary>
    /// <param name="colorIndex">The index of the R,G or B component that is most intense. (0 = R, 1 = G, 2 = B)</param>
    /// <returns>Returns an int array.</returns>
    int[] ReturnMuchBrighterShadeColor(int colorIndex)
    {
        int[] array = snakePlayingColor;
        array[colorIndex] += 20;
        switch(colorIndex)
        {
            case 0:
                array[1] += 5;
                array[2] += 5;
                break;
            case 1:
                array[0] += 5;
                array[2] += 5;
                break;
            case 2:
                array[0] += 5;
                array[1] += 5;
                break;
            default: //must not occur, shouldn't occur provided that no one passes another value than 0, 1 or 2
                break;
        }
        return array;
    }

    /// <summary>
    /// Returns a player color where the intensity of each of the RGB values is increased by 10. (Values ranging from 0 to 255)
    /// </summary>
    /// <returns>Returns an int array.</returns>
    int[] ReturnSlightlyBrighterShade()
    {
        int[] array = snakePlayingColor;
        array[0] += 10;
        array[1] += 10;
        array[2] += 10;
        return array;
    }

    /// <summary>
    /// Returns a player color where the intensity of each of the RGB values is reduced by 40 percents.
    /// </summary>
    /// <returns>Returns an int array.</returns>
    int[] ReturnDarkerShadeColorPixeled()
    {
        int[] array = snakePlayingColor;
        for (int i = 0; i <= 2; i++)
            snakePlayingColor[i] = (int)(0.6f * snakePlayingColor[i]);
        return array;
    }

    /// <summary>
    /// Returns a player color where the intensity of each of the RGB values is increased. The main component (R,G or B component with the highest 
    /// intensity) is increased by 30 (each intensity ranges from 0 to 255), the other components are increased by 5.
    /// </summary>
    /// <param name="colorIndex">The index of the R,G or B component that is most intense. (0 = R, 1 = G, 2 = B)</param>
    /// <returns>Returns an int array.</returns>
    int[] ReturnMuchBrighterShadeColorPixeled(int colorIndex)
    {
        int[] array = snakePlayingColor;
        array[colorIndex] += 30;
        switch (colorIndex)
        {
            case 0:
                array[1] += 5;
                array[2] += 5;
                break;
            case 1:
                array[0] += 5;
                array[2] += 5;
                break;
            case 2:
                array[0] += 5;
                array[1] += 5;
                break;
            default: //must not occur, shouldn't occur provided that no one passes another value than 0, 1 or 2
                break;
        }
        return array;
    }

    /// <summary>
    /// Returns a player color where the intensity of each of the RGB values is increased by 15. (Values ranging from 0 to 255)
    /// </summary>
    /// <returns>Returns an int array.</returns>
    int[] ReturnSlightlyBrighterShadePixeled()
    {
        int[] array = snakePlayingColor;
        array[0] += 15;
        array[1] += 15;
        array[2] += 15;
        return array;
    }

    /// <summary>
    /// Returns the current color of the collectables as a Color.
    /// </summary>
    /// <returns></returns>
    public int[] GetCollectablesColor()
    {
        return collectableColor;
    }

    /// <summary>
    /// This method is called whenever the world size, boundaries, snake speed or delayed spawnings are altered. 
    /// The new difficulty level is determined and saved to an external file.
    /// </summary>
    void DetermineNewDifficulty()
    {
        int minWorldSize = 6;
        int maxWorldSize = 30;
        int minSpeed = 3;
        int maxSpeed = 30;
        double boundariesFactor = 1.5;//difficulty factor of activated world boundaries
        double delayedFactor = 1.2; //difficulty factor of activated delayed spawnings

        int minDifficultyScore = minWorldSize * minSpeed;
        double maxDifficultyScore = maxWorldSize * maxSpeed * boundariesFactor * delayedFactor;

        //the required difficulty scores for the different difficulty levels are set:
        double n = maxDifficultyScore / minDifficultyScore;
        double factor = Math.Pow(n, .2);

        //the current difficulty score is determined:
        double currentScore = worldSize * speed;
        if (worldBoundaries)
        {
            if (delayedSpawnings)
                currentScore *= boundariesFactor * delayedFactor;
            else
                currentScore *= boundariesFactor;
        }
        else
        {
            if (delayedSpawnings)
                currentScore *= delayedFactor;
        }

        //the difficulty is finally set:
        if (currentScore <= minDifficultyScore * factor)
            HighScores.Instance.DifficultyLevel = Difficulty.VeryEasy;
        else if (currentScore <= minDifficultyScore * Math.Pow(factor, 2))
            HighScores.Instance.DifficultyLevel = Difficulty.Easy;
        else if (currentScore <= minDifficultyScore * Math.Pow(factor, 3))
            HighScores.Instance.DifficultyLevel = Difficulty.Medium;
        else if (currentScore <= minDifficultyScore * Math.Pow(factor, 4))
            HighScores.Instance.DifficultyLevel = Difficulty.Hard;
        else if (currentScore < maxDifficultyScore-1)
            HighScores.Instance.DifficultyLevel = Difficulty.VeryHard;
        else
            HighScores.Instance.DifficultyLevel = Difficulty.Ultimate;
    }
}
