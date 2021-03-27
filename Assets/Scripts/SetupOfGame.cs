using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SetupOfGame : MonoBehaviour
{
    /// This class controlls what should happen when the game is loaded for the first time.
    /// The PlayerData is initialized, the default settings are applied

    public Text highscore;

    void Start()
    {
        if (!PlayerPrefs.HasKey("firstOpened"))
        {
            DataSaver.Instance.SaveNewHighscore(0);
            DataSaver.Instance.SaveWorldBoundariesToggleState(false);
            DataSaver.Instance.SaveNewWorldSize(8);
            DataSaver.Instance.SavePlayerSpeed(5);
            DataSaver.Instance.SetShowPixels(false);
            PlayerPrefs.SetInt("firstOpened", 1);
        }
        highscore.text = "HIGHSCORE: " + DataSaver.Instance.GetHighscore() + "%";
    }
}
