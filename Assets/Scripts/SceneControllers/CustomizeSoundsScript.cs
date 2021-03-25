using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CustomizeSoundsScript : MonoBehaviour
{
    private int timeUntilClosureOfInfoPanel, fadingTimeInfoPanel; //the time how long the info panel is maximally opened, the time within which it fades again
    /// <summary>
    /// The sound controller. Object which isn't destroyed on load.
    /// </summary>
    GameObject soundController;
    /// <summary>
    /// The 'selectSound' buttons.
    /// </summary>
    public GameObject[] selectSoundButtons;
    /// <summary>
    /// The 'playSound' buttons.
    /// </summary>
    public GameObject[] playSoundsButtons;
    /// <summary>
    /// The index of the currently selected sound. (Ranging from 0 to 5)
    /// </summary>
    int currentlySelectedSoundIndex;
    /// <summary>
    /// The info panel as GameObject.
    /// </summary>
    public GameObject infoPanel;
    /// <summary>
    /// Transparent button covering the whole screen which 'blocks' any action while the info panel is opened. If pressed the info panel is closed.
    /// </summary>
    public GameObject blocker;
    /// <summary>
    /// The info buttons in the scene as an array.
    /// </summary>
    public GameObject[] infoButtons;

    // Start is called before the first frame update
    void Start()
    {
        timeUntilClosureOfInfoPanel = StaticValues.TimeUntilClosureOfInfoPanel;
        fadingTimeInfoPanel = StaticValues.FadingTimeInfoPanel;
        currentlySelectedSoundIndex = DataSaver.Instance.currentCollectAppleSound;
        soundController = GameObject.FindGameObjectWithTag("SoundController");
        MarkSoundAsSelected(currentlySelectedSoundIndex, true, false);
        infoPanel.SetActive(false);
        blocker.SetActive(false);
    }

    /// <summary>
    /// Plays one of the sounds that can be selected as 'collectApple' sound.
    /// </summary>
    /// <param name="index">The index of the sound to be played. (Indeces ranging from 0 to 5.)</param>
    public void PlaySound(int index)
    {
        soundController.GetComponent<SoundController>().PlayAppleCollected(index);
    }

    /// <summary>
    /// Saves the sound with the passed index as selected sound to an external file.
    /// </summary>
    /// <param name="index">The index of the sound as int (ranging from 0 to 5).</param>
    public void SelectSound(int index)
    {
        MarkSoundAsSelected(currentlySelectedSoundIndex, false);
        currentlySelectedSoundIndex = index;
        DataSaver.Instance.currentCollectAppleSound = index;
        MarkSoundAsSelected(index, true);
    }

    /// <summary>
    /// Toggles the info panel (in)active. If it is toggled active, it will start fading after 'timeUntilClosureOfPanel' and it'll fade within
    /// 'FadingTimeInfoPanel'. When it is closed the invokes/coroutines closing it automatically are cancelled.
    /// Note: When the info panel is set active, all info buttons are set inactive (they're reactivated when the info panel is closed).
    /// </summary>
    /// <param name="newActivityStatus">The new activity status of the info panel.</param>
    public void ToggleInfoPanelActive(bool newActivityStatus)
    {
        infoPanel.SetActive(newActivityStatus);
        blocker.SetActive(newActivityStatus);
        ToggleInfoButtonsActive(!newActivityStatus);
        if (newActivityStatus)
            CoroutinesSingleton.Instance.CloseUIObjectAutomatically(infoPanel, timeUntilClosureOfInfoPanel, fadingTimeInfoPanel, infoButtons, blocker);
        else
            CoroutinesSingleton.Instance.StopClosingUIObjectAutomatically();
    }

    /// <summary>
    /// Toggles all info buttons in the scene (in)active.
    /// </summary>
    /// <param name="newActivity">The new acitivity as bool.</param>
    void ToggleInfoButtonsActive(bool newActivity)
    {
        foreach (GameObject g in infoButtons)
            g.SetActive(newActivity);
    }

    /// <summary>
    /// Marks the sound with the passed index as selected (in the scene). If it is selected, its size will be increased and the 'select' button 
    /// set inactive, elsewhise its size is decreased again and it the 'select' button is set active.
    /// </summary>
    /// <param name="indexOfSound">The index of the sound that is (de)selected.</param>
    /// <param name="selected">Whether the sound is selected (=true) or deselected (=false).</param>
    void MarkSoundAsSelected(int indexOfSound, bool selected)
    {
        if(selected)
        {
            playSoundsButtons[indexOfSound].transform.localPosition = new Vector3(0, 5, 0);
            playSoundsButtons[indexOfSound].GetComponent<RectTransform>().sizeDelta = new Vector2(100, 90);
            if(selectSoundButtons[indexOfSound].activeInHierarchy)
                selectSoundButtons[indexOfSound].SetActive(false);
            PlaySound(indexOfSound);
        }
        else
        {
            playSoundsButtons[indexOfSound].transform.localPosition = new Vector3(0, 15, 0);
            playSoundsButtons[indexOfSound].GetComponent<RectTransform>().sizeDelta = new Vector2(80, 70);
            if (!selectSoundButtons[indexOfSound].activeInHierarchy)
                selectSoundButtons[indexOfSound].SetActive(true);
        }
    }

    /// <summary>
    /// Marks the sound with the passed index as selected (in the scene). If it is selected, its size will be increased and the 'select' button 
    /// set inactive, elsewhise its size is decreased again and it the 'select' button is set active.
    /// </summary>
    /// <param name="indexOfSound">The index of the sound that is (de)selected.</param>
    /// <param name="selected">Whether the sound is selected (=true) or deselected (=false).</param>
    /// <param name="playSound">Whether the sound should be played or not, if a new sound is selected.</param>
    void MarkSoundAsSelected(int indexOfSound, bool selected, bool playSound)
    {
        if (selected)
        {
            playSoundsButtons[indexOfSound].transform.localPosition = new Vector3(0, 5, 0);
            playSoundsButtons[indexOfSound].GetComponent<RectTransform>().sizeDelta = new Vector2(100, 90);
            if (selectSoundButtons[indexOfSound].activeInHierarchy)
                selectSoundButtons[indexOfSound].SetActive(false);
            if(playSound)
                PlaySound(indexOfSound);
        }
        else
        {
            playSoundsButtons[indexOfSound].transform.localPosition = new Vector3(0, 15, 0);
            playSoundsButtons[indexOfSound].GetComponent<RectTransform>().sizeDelta = new Vector2(80, 70);
            if (!selectSoundButtons[indexOfSound].activeInHierarchy)
                selectSoundButtons[indexOfSound].SetActive(true);
        }
    }

    //methods for scene interaction:

    /// <summary>
    /// Opens the scene where the customizing of the sounds was started. (Either the preferences or more options scene.)
    /// </summary>
    public void Back()
    {
        if (DataSaver.Instance.LastEditedSound == LastEditedSound.preferences)
            SceneManager.LoadScene("Preferences");
        else
            SceneManager.LoadScene("MoreOptions");
    }
}
