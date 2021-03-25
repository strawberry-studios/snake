using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SetupOfGame : MonoBehaviour
{
    /// This class controlls what should happen when the game is loaded for the first time.
    /// The PlayerData is initialized, the default settings are applied

    //public Text highscore;

    void Start()
    {
        if (FullVersion.Instance.IsGameFirstLoaded == GameFirstLoaded.firstLoaded)//!PlayerPrefs.HasKey("firstOpened"))
        {
            SetUpPlayerData(); //also creates a new HighScoresData

            SceneInteractionData sceneInteractionData = SceneInteraction.Instance.RetrieveSceneInteractionDataFromFile(); //create if it doesn't exist yet

            FullVersion.Instance.IsGameFirstLoaded = GameFirstLoaded.notFirstLoaded;
        }
        //highscore.text = "HIGHSCORE: " + DataSaver.Instance.GetHighscore() + "%";
    }

    /// <summary>
    /// Saves the original customizable colors to an external file.
    /// </summary>
    /// <param name="data">The player data object to which the info should be saved.</param>
    void SaveCustomizedColors(PlayerData data)
    {
        //color 1 - orange:
        data.ColorsCustomized[0, 0] = 245;
        data.ColorsCustomized[0, 1] = 109;
        data.ColorsCustomized[0, 2] = 0;
        data.ColorsCustomized[0, 3] = 255;

        //color 2 - dark green:
        data.ColorsCustomized[1, 0] = 115;
        data.ColorsCustomized[1, 1] = 125;
        data.ColorsCustomized[1, 2] = 34;
        data.ColorsCustomized[1, 3] = 255;

        //color 3 - faint red:
        data.ColorsCustomized[2, 0] = 186;
        data.ColorsCustomized[2, 1] = 69;
        data.ColorsCustomized[2, 2] = 69;
        data.ColorsCustomized[2, 3] = 255;
    }

    /// <summary>
    /// Sets up the player data.
    /// </summary>
    void SetUpPlayerData()
    {
        PlayerData playerData = new PlayerData();
        playerData.SetHighscore(0);
        playerData.ToggleWorldBoundaries(false);
        playerData.SetWorldSize(12);
        playerData.SpeedSliderValue = 5;
        playerData.ShowPixels(false); //or perhaps better false??? I DON'T KNOW
        playerData.ToggleDelayedSpawnings(false);
        playerData.SetCollectablesColor(Color.red);
        playerData.SetSnakeColor(Color.green);
        playerData.ToggleSnakeHeadMarked(true);
        playerData.ShowGridLines(false);
        playerData.SoundOn = true;
        playerData.VibrationsOn = false;
        playerData.SwipesSensitivity = 0.5f;
        playerData.LastCustomizedColor = LastEditedColor.snake; // statement is not really necessary
        SaveCustomizedColors(playerData);
        playerData.ControlsModeActivated = ControlsMode.buttonsOnly;
        DataSaver.Instance.SavePlayerDataToFile(playerData); //saves the 'starting' settings to an external file
    }
}
