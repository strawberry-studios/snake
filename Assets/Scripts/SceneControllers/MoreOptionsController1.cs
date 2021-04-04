using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MoreOptionsController1 : MonoBehaviour
{
    public GameObject soundToggle, vibrationToggle;
    public GameObject infoPanel, blocker;
    public GameObject[] infoPanelButtons; //all buttons of the info panel as game objects; MUST BE 3 TO AVOID A COMPILATION ERROR       
    public Text infoText, infoHeader; //child of the infoPanel, doesn't need to be set (in)active itself
    private int timeUntilClosureOfInfoPanel, fadingTimeInfoPanel; //the time how long the info panel is maximally opened, the time within which it fades again
    public GameObject ExitGamePanel;
    /// <summary>
    /// The sound controller. Object which isn't destroyed on load.
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

    private void Awake()
    {
        SceneManager.sceneUnloaded += OnSceneExit;
    }

    // Start is called before the first frame update
    void Start()
    {
        soundController = GameObject.FindGameObjectWithTag("SoundController");
        timeUntilClosureOfInfoPanel = StaticValues.TimeUntilClosureOfInfoPanel;
        fadingTimeInfoPanel = StaticValues.FadingTimeInfoPanel;
        LoadSoundsToggleState(soundToggle);
        LoadVibrationToggleState(vibrationToggle);
        infoPanel.SetActive(false);
        blocker.SetActive(false);
        infoText.resizeTextForBestFit = true;
        ToggleExitGamePanelActive(false);
        ToggleCustomizeSoundsButtonActive(DataSaver.Instance.SoundOn);
    }

    /// <summary>
    /// This method is always executed when the scene is unloaded.
    /// </summary>
    void OnSceneExit(Scene scene)
    {
        CoroutinesSingleton.Instance.StopClosingUIObjectAutomatically();
    }

    /// <summary>
    /// Loads the value of 'soundToggle' and sets the corresponding toggle.
    /// </summary>
    /// <param name="toggle">The toggle-object (of which the position ahould be altered) as GameObject to pass</param>
    private void LoadSoundsToggleState(GameObject toggle)
    {
        bool soundOn = DataSaver.Instance.SoundOn;
        soundToggle.GetComponent<Toggle>().isOn = soundOn;
    }

    /// <summary>
    /// Loads the value of 'vibrationToggle' and sets the corresponding toggle.
    /// </summary>
    /// <param name="toggle">The toggle-object (of which the position ahould be altered) as GameObject to pass</param>
    private void LoadVibrationToggleState(GameObject toggle)
    {
        vibrationToggle.GetComponent<Toggle>().isOn = DataSaver.Instance.VibrationsOn;
    }

    /// <summary>
    /// Toggles the 'customizeSounds' button (in)active.
    /// </summary>
    /// <param name="active">The new activity status of the 'customizeSoundsButton'.</param>
    void ToggleCustomizeSoundsButtonActive(bool active)
    {
        if(customizeSoundsButton.activeInHierarchy != active)
         customizeSoundsButton.SetActive(active);
    }

    //methods that need to be assigned to buttons in the scene:

    /// <summary>
    /// Opens the credits scene.
    /// </summary>
    public void OpenCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    /// <summary>
    /// Opens the most recent instructions scene.
    /// </summary>
    public void OpenInstructions()
    {
        SceneManager.LoadScene("Instructions");
    }

    /// <summary>
    /// Opens a panel where the player can decide whether they really want to exit the game.
    /// </summary>
    public void ExitGame()
    {
        ToggleExitGamePanelActive(true);
    }

    /// <summary>
    /// Toggles the panel where the player can confirm exiting the game (in)active.
    /// </summary>
    /// <param name="state">The new activity status of the 'exitGamePanel'.</param>
    public void ToggleExitGamePanelActive(bool state)
    {
        ExitGamePanel.SetActive(state);
        blocker.SetActive(state);
    }

    /// <summary>
    /// Confirms whether the game should be exited or not.
    /// </summary>
    /// <param name="exitGame">Confirm (true) or dismiss (false) game as bool.</param>
    public void ConfirmExitGame(bool exitGame)
    {
        if (exitGame)
            Application.Quit();
        else
            ToggleExitGamePanelActive(false);
    }

    //methods for setting new values for the categories: snakeSpeed, worldSize, worldBoundariesOn, delayedSpawningsOn:

    /// <summary>
    /// Determins whether the sound is switched on or not. If the sound is switched on, a sound is played.
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
    /// Determins whether the vibrations are switched on or not. If the vibration is switched on, the device also vibrates for a half second.
    /// </summary>
    /// <param name="state">The new vibrations-on state to pass.</param>
    public void SetVibrationOn(bool state)
    {
        if (state && !DataSaver.Instance.VibrationsOn)
            Vibration.Vibrate(500);
        DataSaver.Instance.VibrationsOn = state;
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

    /// <summary>
    /// Sets a certain amount of the buttons of the info panel active.
    /// </summary>
    /// <param name="forOpeningFullVersionPurchase">If true, the 'purchase full version' and 'ok' buttons are active, otherwise only the
    /// 'dismiss' button is active.</param>
    void ShowInfoButtons(bool forOpeningFullVersionPurchase)
    {
        if(forOpeningFullVersionPurchase)
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
    /// Displays information about 'soundON'.
    /// </summary>
    public void InformAboutSound()
    {
        ToggleInfoPanelActive(true);
        ShowInfoButtons(false);
        infoHeader.text = "SOUNDS:";
        infoText.text = "If this option is toggled on, sound effects are played." +
            "\nYou can even choose which sounds should be used.";
    }

    /// <summary>
    /// Displays information about 'vibration'.
    /// </summary>
    public void InformAboutVibration()
    {
        ToggleInfoPanelActive(true);
        ShowInfoButtons(false);
        infoHeader.text = "VIBRATIONS:";
        infoText.text = "If this option is toggled on, your device vibrates when you collect apples or lose.";
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

    //to be attached to buttons:

    /// <summary>
    /// Opens the 'CustomizeSounds' Scene.
    /// </summary>
    public void OpenCustomizeSounds()
    {
        DataSaver.Instance.LastEditedSound = LastEditedSound.moreOptions;
        SceneManager.LoadScene("CustomizeSounds");
    }

    //methods for the scene interaction:

    /// <summary>
    /// Loads the scene where the full version can be purchased.
    /// </summary>
    public void LoadUnlockFullVersion()
    {
        SceneManager.LoadScene("PurchaseFullVersion");
    }

    /// <summary>
    /// Loads the scene where the legal information can be looked up.
    /// </summary>
    public void LoadLegalInformation()
    {
        SceneManager.LoadScene("LegalInformation");
    }

}
