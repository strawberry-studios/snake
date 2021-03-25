using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PreferencesController : MonoBehaviour
{
    public GameObject soundToggle, vibrationToggle;
    public Dropdown controlsMode;
    public GameObject infoPanel;
    float originalInfoSizeY, originalInfoPositionY; //the original size of the rect transform of the info text
    public RectTransform infoRect; //the rect transform of the info text
    public GameObject[] infoPanelButtons; //all buttons of the info panel as game objects; MUST BE 3 TO AVOID A COMPILATION ERROR       
    public Text infoText, infoHeader; //child of the infoPanel, doesn't need to be set (in)active itself
    private int timeUntilClosureOfInfoPanel, fadingTimeInfoPanel; //the time how long the info panel is maximally opened, the time within which it fades again
    /// <summary>
    /// The sound controller of the scene. An object which isn't destroyed on load.
    /// </summary>
    GameObject soundController;
    /// <summary>
    /// The info buttons in the scene as an array.
    /// </summary>
    public GameObject[] infoButtons;
    /// <summary>
    /// A button which is the child of the 'infoPanel'. It opens the 'UnlockFullVersion' Scene if pressed.
    /// </summary>
    public GameObject fullVersionButton;
    /// <summary>
    /// The customizeSounds button as GameObject.
    /// </summary>
    public GameObject customizeSoundsButton;
    /// <summary>
    /// Transparent button covering the whole screen which 'blocks' any action while the info panel is opened. If pressed the info panel is closed.
    /// </summary>
    public GameObject blocker;
    /// <summary>
    /// Parent of all game objects that have something to do with the 'sensitivity' of the swipes-recognizing.
    /// </summary>
    public GameObject swipeSensitivity;
    /// <summary>
    /// Game object parenting all objects dealing with the sound-settings.
    /// </summary>
    public GameObject soundSettings;
    /// <summary>
    /// Game object parenting all objects dealing with the vibration-settings.
    /// </summary>
    public GameObject vibrationSettings;
    /// <summary>
    /// Whether In-App Purchases are enabled or not. If enabled, the sounds can be customized if the Full Version was unlocked, elsewhise the 
    /// button "Customize Sound" doens't exist.
    /// </summary>
    bool iAPsEnabled;

    private void Awake()
    {
        SceneManager.sceneUnloaded += OnSceneExit;
    }

    /// <summary>
    /// This method is always executed when the scene is unloaded.
    /// </summary>
    void OnSceneExit(Scene scene)
    {
        CoroutinesSingleton.Instance.StopClosingUIObjectAutomatically();
    }

    // Start is called before the first frame update
    void Start()
    {
        iAPsEnabled = StaticValues.IAPsEnabled;
        soundController = GameObject.FindGameObjectWithTag("SoundController");
        timeUntilClosureOfInfoPanel = StaticValues.TimeUntilClosureOfInfoPanel;
        fadingTimeInfoPanel = StaticValues.FadingTimeInfoPanel;
        LoadSoundsToggleState(soundToggle);
        LoadVibrationToggleState(vibrationToggle);
        originalInfoSizeY = infoRect.sizeDelta.y;
        originalInfoPositionY = infoRect.localPosition.y;
        print(originalInfoPositionY);
        swipeSensitivity.SetActive(false);
        LoadControlsMode(controlsMode);
        infoPanel.SetActive(false);
        blocker.SetActive(false);
        infoText.resizeTextForBestFit = true;
        ToggleInfoButtonsActive(true);
        ToggleCustomizeSoundsButtonActive(DataSaver.Instance.SoundOn);
    }

    /// <summary>
    /// Loads the 'controls mode' and sets the appropriate dropdown state of the corresponding dropdown.
    /// </summary>
    /// <param name="controlsDropdown">The dropdown (of which the state should be altered) as GameObject.</param>
    private void LoadControlsMode(Dropdown controlsDropdown)
    {
        ControlsMode c = DataSaver.Instance.ControlsModeActivated;
        controlsMode.value = c == ControlsMode.buttonsOnly ? 0 :
                                c == ControlsMode.swipeOnly ? 1 : 
                                    c == ControlsMode.keypad ? 2 : 3;
        if (controlsMode.value == (0 | 3))
            ToggleSwipesSensitivityActive(true);
    }

    /// <summary>
    /// Loads the sensitivity of the swipes-recognizing system from an external file.
    /// </summary>
    void LoadSwipeSensitivity()
    {
        swipeSensitivity.GetComponent<Slider>().value = DataSaver.Instance.SwipesSensitivity;
    }

    /// <summary>
    /// Loads the value of 'soundToggle' and sets the corresponding toggle.
    /// </summary>
    /// <param name="toggle">The toggle-object (of which the position ahould be altered) as GameObject to pass</param>
    private void LoadSoundsToggleState(GameObject toggle)
    {
        soundToggle.GetComponent<Toggle>().isOn = DataSaver.Instance.SoundOn;
    }

    /// <summary>
    /// Loads the value of 'vibrationToggle' and sets the corresponding toggle.
    /// </summary>
    /// <param name="toggle">The toggle-object (of which the position ahould be altered) as GameObject to pass</param>
    private void LoadVibrationToggleState(GameObject toggle)
    {
        vibrationToggle.GetComponent<Toggle>().isOn = DataSaver.Instance.VibrationsOn;
    } 

    //methods that need to be assigned to buttons in the scene:

    //methods for setting new values for the categories: snakeSpeed, worldSize, worldBoundariesOn, delayedSpawningsOn:

    /// <summary>
    /// Determins whether the sound is switched on or not. Also plays a sound if the sounds are activated.
    /// </summary>
    /// <param name="state">The new sounds-on state to pass.</param>
    public void SetSoundsOn(bool state)
    {
        if (state && !DataSaver.Instance.SoundOn)
            soundController.GetComponent<SoundController>().PlayActivateSoundsClip();
        if (customizeSoundsButton.activeInHierarchy != state)
            ToggleCustomizeSoundsButtonActive(state);
        DataSaver.Instance.SoundOn = state;
    }

    /// <summary>
    /// Determins whether the vibrations are switched on or not. The device vibrates for a half second if the vibration is activated.
    /// </summary>
    /// <param name="state">The new vibrations-on state to pass.</param>
    public void SetVibrationOn(bool state)
    {
        if (state && !DataSaver.Instance.VibrationsOn)
            Vibration.Vibrate(500);
        DataSaver.Instance.VibrationsOn = state;
    }

    /// <summary>
    /// Sets the mode with which the snake can be controlled.
    /// </summary>
    /// <param name="state">The state as int (implementation of this method needs to comply with the dropdown options in the scene).</param>
    public void SetControlsMode(int state)
    {
        switch(state)
        {
            case 0:
                DataSaver.Instance.ControlsModeActivated = ControlsMode.buttonsOnly;
                ToggleSwipesSensitivityActive(false);
                break;
            case 1:
                DataSaver.Instance.ControlsModeActivated = ControlsMode.swipeOnly;
                ToggleSwipesSensitivityActive(true);
                break;
            case 2:
                DataSaver.Instance.ControlsModeActivated = ControlsMode.keypad;
                ToggleSwipesSensitivityActive(false);
                break;
            case 3:
                DataSaver.Instance.ControlsModeActivated = ControlsMode.buttonsAndSwipe;
                ToggleSwipesSensitivityActive(true);
                break;
            default: //should never occur
                break;
        }
    }

    /// <summary>
    /// Sets the sensitivity with which the snake can be controlled with swipes.
    /// </summary>
    public void SetSwipeSensitivity(float value)
    {
        DataSaver.Instance.SwipesSensitivity = value;
    }

    /// <summary>
    /// Toggles the swipe sensitivity objects (in)active and moves the other objects downward/upward.
    /// </summary>
    /// <param name="activityState">The new state of the swipe sensitivity objects.</param>
    void ToggleSwipesSensitivityActive(bool activityState)
    {
        if (activityState != swipeSensitivity.activeInHierarchy)
        {
            Vector3 distance = new Vector3(0, 115, 0); //the distance by which the UIs below the 'swipe sensitivity' UI are moved up/down 
            if (activityState)
            {
                LoadSwipeSensitivity();
                vibrationSettings.transform.localPosition -= distance;
                soundSettings.transform.localPosition -= distance;
                infoPanel.transform.localPosition -= distance;
            }
            else
            {
                vibrationSettings.transform.localPosition += distance;
                soundSettings.transform.localPosition += distance;
                infoPanel.transform.localPosition += distance;
            }
        }
        swipeSensitivity.SetActive(activityState);
    }

    /// <summary>
    /// Toggles the 'customizeSounds' button (in)active.
    /// </summary>
    /// <param name="active">The new activity status of the 'customizeSoundsButton'. Only if IAPs are enabled, elsewhise the button is 
    /// disabled by default.</param>
    void ToggleCustomizeSoundsButtonActive(bool active)
    {
        if (iAPsEnabled)
        {
            if (customizeSoundsButton.activeInHierarchy != active)
                customizeSoundsButton.SetActive(active);
        }
        else
            customizeSoundsButton.SetActive(false);
    }

    //methods for displaying information on the info tabs:

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
        {
            if (fullVersionButton.activeInHierarchy)
                fullVersionButton.SetActive(false);
            CoroutinesSingleton.Instance.CloseUIObjectAutomatically(infoPanel, timeUntilClosureOfInfoPanel, fadingTimeInfoPanel, infoButtons, blocker);
        }
        else
            CoroutinesSingleton.Instance.StopClosingUIObjectAutomatically();
    }

    ///// <summary>
    ///// Toggles the info panel (in)active. If it is toggled active, it will start fading after 'timeUntilClosureOfPanel' and it'll fade within
    ///// 'FadingTimeInfoPanel'. When it is closed the invokes/coroutines closing it automatically are cancelled.
    ///// Note: When the info panel is set active, all info buttons are set inactive (they're reactivated when the info panel is closed).
    ///// The full version button is activated if the info panel is set active and 'showFullVersionButton' is set to true.
    ///// </summary>
    ///// <param name="newActivityStatus">The new activity status of the info panel.</param>
    ///// <param name="showFullVersionButton">Whether the full version button should be set active or not.</param>
    //public void ToggleInfoPanelActive(bool newActivityStatus, bool showFullVersionButton)
    //{
    //    infoPanel.SetActive(newActivityStatus);
    //    blocker.SetActive(newActivityStatus);
    //    ToggleInfoButtonsActive(!newActivityStatus);
    //    if (newActivityStatus)
    //    {
    //        if(fullVersionButton.activeInHierarchy != showFullVersionButton)
    //            fullVersionButton.SetActive(showFullVersionButton);
    //        CoroutinesSingleton.Instance.CloseUIObjectAutomatically(infoPanel, TimeUntilClosureOfInfoPanel, FadingTimeInfoPanel, infoButtons, blocker);
    //    }
    //    else
    //        CoroutinesSingleton.Instance.StopClosingUIObjectAutomatically();
    //}

    /// <summary>
    /// Sets a certain amount of the buttons of the info panel active.
    /// </summary>
    /// <param name="forOpeningFullVersionPurchase">If true, the 'purchase full version' and 'ok' buttons are active, otherwise only the
    /// 'dismiss' button is active.</param>
    void ShowInfoButtons(bool forOpeningFullVersionPurchase)
    {
        if (forOpeningFullVersionPurchase)
        {
            infoPanelButtons[0].SetActive(false);
            infoPanelButtons[1].SetActive(true);
            infoPanelButtons[2].SetActive(true);
        }
        else
        {
            infoPanelButtons[0].SetActive(true);
            infoPanelButtons[1].SetActive(false);
            infoPanelButtons[2].SetActive(false);
        }
    }

    /// <summary>
    /// Displays information about the 'controls-modes'.
    /// </summary>
    public void InformAboutControlsMode()
    {
        ToggleInfoPanelActive(true);
        ToggleInfoPanelSize(true);
        ShowInfoButtons(false);
        infoHeader.text = "CONTROLS MODE:"; 
        infoText.text = "By altering this mode, you change the way how you can navigate the snake. " +
            "\nEither by swiping or by pressing buttons.";
    }

    /// <summary>
    /// Displays information about the 'swipe-sensitivity'.
    /// </summary>
    public void InformAboutSwipeSensitivity()
    {
        ToggleInfoPanelActive(true);
        ToggleInfoPanelSize(true);
        ShowInfoButtons(false);
        infoHeader.text = "SWIPE SENSITIVITY:";
        infoText.text = "By changing the position of the slider, the sensitivity of the swipes is altered. " +
            "\nThe higher the swipe sensitivity is, the faster the snake reacts to your swipes. ";
    }

    /// <summary>
    /// Displays information about 'soundON'.
    /// </summary>
    public void InformAboutSound()
    {
        ToggleInfoPanelActive(true);
        ToggleInfoPanelSize(false);
        ShowInfoButtons(false);
        infoHeader.text = "SOUNDS:";
        if (iAPsEnabled)
        {
            infoText.text = "If this option is toggled on, sound effects will be played." +
                "\nThey can even be customized if the full version was purchased.";
        }
        else
        {
            infoText.text = "If this option is toggled on, sound effects will be played.";
        }
    }

    /// <summary>
    /// Displays information about 'vibration'.
    /// </summary>
    public void InformAboutVibration()
    {
        ToggleInfoPanelActive(true);
        ToggleInfoPanelSize(false);
        ShowInfoButtons(false);
        infoHeader.text = "VIBRATIONS:";
        infoText.text = "If this option is toggled on, your device vibrates when you collect apples or lose.";
    }

    /// <summary>
    /// Toggles the info text larger and repositions it or resets the original size and position.
    /// </summary>
    /// <param name="larger">If true, it the text is toggled larger, elsewhise the size is reset.</param>
    void ToggleInfoPanelSize(bool larger)
    {
        if (larger)
        {
            infoRect.sizeDelta = new Vector2(infoRect.sizeDelta.x, 310);
            infoRect.localPosition = new Vector3(infoRect.localPosition.x, originalInfoPositionY - 30);
        }
        else
        {
            infoRect.sizeDelta = new Vector2(infoRect.sizeDelta.x, originalInfoSizeY);
            infoRect.localPosition = new Vector3(infoRect.localPosition.x, originalInfoPositionY);
            print(originalInfoPositionY);
            print(infoRect.position.y);
        }
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
    /// Shows a message which informs the player that they need to unlock the full version if they want to customize their own colors.
    /// </summary>
    void ShowCustomizingOptionLockedMessage()
    {
        ToggleInfoPanelActive(true);
        ShowInfoButtons(true);
        infoHeader.text = "  LOCKED:";
        infoText.text = "You need to unlock the full version to customize sound effects.";
    }

    //to be attached to buttons:

    /// <summary>
    /// Opens the 'CustomizeSounds' Scene, but only if the full version was already unlocked. Elsewhise the info panel is opened and informs 
    /// the player that they need to unlock the full version.
    /// </summary>
    public void OpenCustomizeSounds()
    {
        if (FullVersion.Instance.IsFullVersionUnlocked == FullVersionUnlocked.unlocked)
            LoadCustomizeSoundsScene();
        else
            ShowCustomizingOptionLockedMessage();
    }

    //methods for the scene interaction:

    /// <summary>
    /// Opens the difficulty scene. Saves 'difficulty' as the last opened options scene to an external file.
    /// </summary>
    public void OpenDifficulty()
    {
        SceneInteraction.Instance.OptionsSceneLastOpened = OptionsSceneLastOpened.difficulty;
        SceneManager.LoadScene("Difficulty");
    }

    /// <summary>
    /// Loads the scene where the full version can be purchased.
    /// </summary>
    public void LoadUnlockFullVersion()
    {
        SceneManager.LoadScene("PurchaseFullVersion");
    }

    /// <summary>
    /// Loads the 'CustomizeSounds' Scene.
    /// </summary>
    void LoadCustomizeSoundsScene()
    {
        DataSaver.Instance.LastEditedSound = LastEditedSound.preferences;
        SceneManager.LoadScene("CustomizeSounds");
    }
}
