using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DifficultyController : MonoBehaviour
{
    public GameObject worldBoundariesToggle, delayedSpawningsToggle;
    public GameObject worldSizeSlider, speedSlider;
    public GameObject infoPanel;
    float originalInfoSizeY, originalInfoPositionY; //the original size of the rect transform of the info text
    public RectTransform infoRect; //the rect transform of the info text
    public Text infoText, infoHeader; //child of the infoPanel, doesn't need to be set (in)active itself
    private int timeUntilClosureOfInfoPanel, fadingTimeInfoPanel; //the time how long the info panel is maximally opened, the time within which it fades again
    public Text difficultyText; //the text which displays the current difficulty level
    /// <summary>
    /// The info buttons in the scene as an array.
    /// </summary>
    public GameObject[] infoButtons;
    /// <summary>
    /// Transparent button covering the whole screen which 'blocks' any action while the info panel is opened. If pressed the info panel is closed.
    /// </summary>
    public GameObject blocker;

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
        timeUntilClosureOfInfoPanel = StaticValues.timeUntilClosureOfInfoPanel;
        fadingTimeInfoPanel = StaticValues.fadingTimeInfoPanel;
        LoadWorldBoundariesToggleState(worldBoundariesToggle);
        LoadDelayedSpawningsToggleState(delayedSpawningsToggle);
        LoadSpeedSliderState(speedSlider);
        LoadWorldSizeSliderState(worldSizeSlider);
        LoadCurrentDifficulty();
        originalInfoSizeY = infoRect.sizeDelta.y;
        originalInfoPositionY = infoRect.position.y;
        infoPanel.SetActive(false);
        blocker.SetActive(false);
        infoText.resizeTextForBestFit = true;
    }

    /// <summary>
    /// Assigns the appropriate text to 'difficultyText' depending on the current difficulty held by an external file.
    /// </summary>
    void LoadCurrentDifficulty()
    {
        switch(HighScores.Instance.DifficultyLevel)
        {
            case Difficulty.VeryEasy:
                difficultyText.text = "DIFFICULTY: VERY EASY";
                break;
            case Difficulty.Easy:
                difficultyText.text = "DIFFICULTY: EASY";
                break;
            case Difficulty.Medium:
                difficultyText.text = "DIFFICULTY: MEDIUM";
                break;
            case Difficulty.Hard:
                difficultyText.text = "DIFFICULTY: HARD";
                break;
            case Difficulty.VeryHard:
                difficultyText.text = "DIFFICULTY: VERY HARD";
                break;
            case Difficulty.Ultimate:
                difficultyText.text = "DIFFICULTY: ULTIMATE";
                break;
            default:
                difficultyText.text = "DIFFICULTY: VERY EASY"; //can never be called, is only added to avoid compilation errors
                break;
        }
    }

    /// <summary>
    /// Loads the 'player speed' and sets the position of the corresponding slider.
    /// </summary>
    /// <param name="speedSlider">The slider (of which the position should be altered) as GameObject.</param>
    private void LoadSpeedSliderState(GameObject speedSlider)
    {
        speedSlider.GetComponent<Slider>().value = DataSaver.Instance.SpeedSliderValue;
    }

    /// <summary>
    /// Loads the 'world size' and sets the position of the corresponding slider.
    /// </summary>
    /// <param name="worldSizeSlider">The slider (of which the position should be altered) as GameObject.</param>
    private void LoadWorldSizeSliderState(GameObject worldSizeSlider)
    {
        worldSizeSlider.GetComponent<Slider>().value = DataSaver.Instance.GetWorldSize();
    }

    /// <summary>
    /// Loads the value of 'worldBoundariesOn' and sets the corresponding toggle.
    /// </summary>
    /// <param name="toggle">The toggle-object (of which the position ahould be altered) as GameObject to pass</param>
    private void LoadWorldBoundariesToggleState(GameObject toggle)
    {
            toggle.GetComponent<Toggle>().isOn = DataSaver.Instance.GetWorldBoundariesState() ? true : false;
    }

    /// <summary>
    /// Loads the value of 'delayedSpawnings' and sets the state of the corresponding toggle.
    /// </summary>
    /// <param name="toggle">The toggle-object (of which the position should be altered) as GameObject to pass</param>
    private void LoadDelayedSpawningsToggleState(GameObject thisToggle)
    {
        thisToggle.GetComponent<Toggle>().isOn = DataSaver.Instance.GetDelayedSpawningsState() ? true : false;
    }

    //methods that need to be assigned to buttons in the scene:

    //methods for setting new values for the categories: snakeSpeed, worldSize, worldBoundariesOn, delayedSpawningsOn:

    /// <summary>
    /// Sets the new speedSlider value of the snake and hence also the new speed.
    /// </summary>
    /// <param name="speed">new snake speed to pass.</param>
    public void SetSnakeSpeed(float speed)
    {
        int speedAsInt;
        speedAsInt = Mathf.RoundToInt(speed);
        DataSaver.Instance.SpeedSliderValue = speedAsInt;
        LoadCurrentDifficulty();
    }

    /// <summary>
    /// Sets the new size of the world in which you play and hence also the new speed. See 'DataSaver'.
    /// </summary>
    /// <param name="size">new snake speed to pass.</param>
    public void SetWorldSize(float size)
    {
        int sizeAsInt;
        sizeAsInt = Mathf.RoundToInt(size);
        DataSaver.Instance.SaveNewWorldSize(sizeAsInt);
        LoadCurrentDifficulty();
    }

    /// <summary>
    /// Determins whether the margins of the worlds are boundaries or not.
    /// </summary>
    /// <param name="state">new world margins state to pass.</param>
    public void SetMarginsAsWorldBoundary(bool state)
    {
        DataSaver.Instance.SaveWorldBoundariesToggleState(state);
        LoadCurrentDifficulty();
    }

    /// <summary>
    /// Determins whether the spawning of new collectables should be on or not.
    /// If so, collectables might even spawn 'under' the snake, i.e. they can only be collected once the snake moved away from this position.
    /// </summary>
    /// <param name="state">new delayedSpawnings state to pass.</param>
    public void SetDelayedSpawnings(bool state)
    {
        DataSaver.Instance.SaveDelayedSpawningsToggleState(state);
        LoadCurrentDifficulty();
    }

    //methods for displaying information on the info tabs:

    /// <summary>
    /// Toggles the info panel (in)active. If it is toggled active, it will start fading after 'timeUntilClosureOfPanel' and it'll fade within
    /// 'fadingTimeInfoPanel'. When it is closed the invokes/coroutines closing it automatically are cancelled.
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
        {
            CoroutinesSingleton.Instance.StopClosingUIObjectAutomatically();
        }
    }

    /// <summary>
    /// Displays information about the 'worldSize'.
    /// </summary>
    public void InformAboutWorldSize()
    {
        ToggleInfoPanelActive(true);
        ToggleInfoPanelSize(false);
        infoHeader.text = "WORLD SIZE:";    
        infoText.text = "By changing the position of this slider, the size of the world gets bigger or smaller. " +
            "\nThe bigger the world is, the more difficult the game.";
    }

    /// <summary>
    /// Displays information about the 'playerSpeed'.
    /// </summary>
    public void InformAboutPlayerSpeed()
    {
        ToggleInfoPanelActive(true);
        ToggleInfoPanelSize(false);
        infoHeader.text = "SPEED:";
        infoText.text = "By changing the position of this slider, the speed of the snake increases or decreases. " +
            "\nThe faster the snake is, the more difficult the game.";
    }

    /// <summary>
    /// Displays information about 'delayedSpawnings'.
    /// </summary>
    public void InformAboutDelayedSpawnings()
    {
        ToggleInfoPanelActive(true);
        ToggleInfoPanelSize(true);
        infoHeader.text = "DELAYED SPAWNING:";
        infoText.text = "If this option is toggled on, the game becomes more difficult. " +
            "\nThe longer you play, the more likely it is that a new apple doesn't spawn immediately after collecting one.";
    }

    /// <summary>
    /// Displays information about the 'worldBoundaries' option.
    /// </summary>
    public void InformAboutWorldBoundaries()
    {
        ToggleInfoPanelActive(true);
        ToggleInfoPanelSize(true);
        infoHeader.text = "WORLD BOUNDARIES:";
        infoText.text = "If this option is toggled on, you lose if the snake touches one of the margins of the world. \nThis makes the game more difficult.";
    }

    /// <summary>
    /// Toggles the info text larger or resets it.
    /// </summary>
    /// <param name="larger">If true, it the text is toggled larger, elsewhise the size is reset.</param>
    void ToggleInfoPanelSize(bool larger)
    {
        if (larger)
        {
            infoRect.sizeDelta = new Vector2(infoRect.sizeDelta.x, 330);
            infoRect.position = new Vector3(infoRect.position.x, originalInfoPositionY - 75);
        }
        else
        {
            infoRect.sizeDelta = new Vector2(infoRect.sizeDelta.x, originalInfoSizeY);
            infoRect.position = new Vector3(infoRect.position.x, originalInfoPositionY);
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

    //methods for the scene interaction:

    /// <summary>
    /// Opens the preferences scene. Saves 'preferences' as the last opened options scene to an external file.
    /// </summary>
    public void OpenPreferences()
    {
        SceneInteraction.Instance.OptionsSceneLastOpened = OptionsSceneLastOpened.preferences;
        SceneManager.LoadScene("Preferences");
    }
}
