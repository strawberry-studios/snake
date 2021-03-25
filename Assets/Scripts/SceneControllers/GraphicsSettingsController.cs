using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GraphicsSettingsController : MonoBehaviour
{
    public GameObject pixelModeToggle, snakeHeadMarkedToggle, gridLinesOnToggle; //the toggles in the scene
    public GameObject infoPanel; //an info panel that can display further information about one of the toggle-categories 
                                 //when a corresponding info button is pressed
    public Text infoPanelText, infoPanelHeader; //the text which is displayed on the info panel (a child of the infoPanel, doesn't need to be set 
                                                //(in)active itself)
    public RectTransform textRect;
    Vector2 defaultSizeTextRect; //the default size of the text - rect transform

    private int timeUntilClosureOfInfoPanel; //the time between opening an info panel and its closing (in millis)
    private int fadingTimeInfoPanel; //the time during which the info panel fades off (once it starts disappearing, in millis)
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
        timeUntilClosureOfInfoPanel = StaticValues.TimeUntilClosureOfInfoPanel;
        fadingTimeInfoPanel = StaticValues.FadingTimeInfoPanel;
        defaultSizeTextRect = new Vector2(textRect.sizeDelta.x, textRect.sizeDelta.y);
        LoadShowPixelsToggleState(pixelModeToggle);
        LoadSnakeHeadMarkedToggleState(snakeHeadMarkedToggle);
        LoadShowGridLinesToggleState(gridLinesOnToggle);
        ToggleInfoPanelActive(false);
        infoPanelText.resizeTextForBestFit = true;
    }

    /// <summary>
    /// Loads the current state of 'showPixels' and sets the position of the corresponding toggle in the scene.
    /// </summary>
    /// <param name="thisToggle">The toggle-object as GameObject to pass.</param>
    private void LoadShowPixelsToggleState(GameObject thisToggle)
    {
        thisToggle.GetComponent<Toggle>().isOn = DataSaver.Instance.GetShowPixels() ? true : false;
    }

    /// <summary>
    /// Loads the current state of 'snakeHeadMarked' and sets the position of the corresponding toggle in the scene.
    /// </summary>
    /// <param name="thisToggle">The toggle-object as GameObject to pass.</param>
    private void LoadSnakeHeadMarkedToggleState(GameObject thisToggle)
    {
        thisToggle.GetComponent<Toggle>().isOn = DataSaver.Instance.GetSnakeHeadMarked() ? true : false;
    }

    /// <summary>
    /// Loads the current state of 'showGridLines' and sets the position of the corresponding toggle in the scene.
    /// </summary>
    /// <param name="thisToggle">The toggle-object as GameObject to pass.</param>
    private void LoadShowGridLinesToggleState(GameObject thisToggle)
    {
        thisToggle.GetComponent<Toggle>().isOn = DataSaver.Instance.GetShowGridLines() ? true : false;
    }

    //Methods that need to be attached to buttons:

    //methods for setting new values for the following categories: pixelMode, snakeHeadMarked, gridLinesOn 

    /// <summary>
    /// Assigns a new value to the show-pixel-mode category. The info is saved to an external file.
    /// </summary>
    /// <param name="newValue">Whether the pixel-mode should be on or not as bool.</param>
    public void SetPixelMode(bool newValue)
    {
        DataSaver.Instance.SetShowPixels(newValue);
    }

    /// <summary>
    /// Assigns a new value to the snake-head-marked category. The info is saved to an external file.
    /// </summary>
    /// <param name="newValue">Whether the head of the snake should be marked or not as bool.</param>
    public void SetSnakeHeadMarked(bool newValue)
    {
        DataSaver.Instance.SetSnakeHeadMarked(newValue);
    }

    /// <summary>
    /// Assigns a new value to the show-grid-lines category. The info is saved to an external file.
    /// </summary>
    /// <param name="newValue">Whether the grid-lines should be visible or not as bool.</param>
    public void SetShowGridLines(bool newValue)
    {
        DataSaver.Instance.SetShowGridLines(newValue);
    }

    //methods which load the info that is related to the different toggles:

    /// <summary>
    /// Displays information about the 'showPixels' option.
    /// </summary>
    public void InformAboutShowPixels()
    {
        ToggleInfoPanelActive(true);
        ResetTextSize();
        infoPanelHeader.text = "PIXEL MODE:"; 
        infoPanelText.text = "If this option is toggled on, the snake and apples are pixeled.\nIf not, they are displayed as one block.";
    }

    /// <summary>
    /// Displays information about the 'snakeHeadMarked' option.
    /// </summary>
    public void InformAboutSnakeHeadMarked()
    {
        ToggleInfoPanelActive(true);
        textRect.sizeDelta = new Vector2(textRect.sizeDelta.x, 260);
        infoPanelHeader.text = "SNAKE HEAD MARKED:";
        infoPanelText.text = "If this option is toggled on, the snake-head is colored in a different shade than the other snake-blocks.";
    }

    /// <summary>
    /// Displays information about the 'showGridLines' option.
    /// </summary>
    public void InformAboutShowGridLines()
    {
        ToggleInfoPanelActive(true);
        ResetTextSize();
        infoPanelHeader.text = "GRID LINES:";
        infoPanelText.text = "If this option is toggled on, all squares of the world are marked by grid lines. " +
            "\nOtherwise, the world only has one color.";
    }

    /// <summary>
    /// Resets the size of the 'text rect transform' to its original size.
    /// </summary>
    void ResetTextSize()
    {
        textRect.sizeDelta = defaultSizeTextRect;
    }

    /// <summary>
    /// Toggles the info panel (in)active. If it is toggled active, it will start fading after 'timeUntilClosureOfPanel' and it'll fade within
    /// 'FadingTimeInfoPanel'.
    /// Note: When the info panel is set active, all info buttons are set inactive (they're reactivated when the info panel is closed).
    /// </summary>
    /// <param name="newActivityStatus">The new activity status of the info panel.</param>
    public void ToggleInfoPanelActive(bool newActivityStatus)
    {
        blocker.SetActive(newActivityStatus);
        infoPanel.SetActive(newActivityStatus);
        ToggleInfoButtonsActive(!newActivityStatus);
        //when the info panel is opened, it will automatically close again after 'TimeUntilClosureOfInfoPanel', 
        //fading within 'FadingTimeInfoPanel'
        //elsewhise the execution of the coroutine is cancelled
        if (newActivityStatus)
            CoroutinesSingleton.Instance.CloseUIObjectAutomatically(infoPanel, timeUntilClosureOfInfoPanel, fadingTimeInfoPanel, infoButtons, blocker);
        else
        {
            CoroutinesSingleton.Instance.StopClosingUIObjectAutomatically();
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

    //methods that are required for scene interaction:

    /// <summary>
    /// Loads the collectablesColor scene. Saves the collectables-color-scene as 'GraphicsSceneLastOpened' to an external file.
    /// </summary>
    public void LoadCollectablesColorSkins()
    {
        SceneInteraction.Instance.GraphicsSceneLastOpened = GraphicsSceneLastOpened.collectablesColor;
        SceneManager.LoadScene("Skins_Collectables_Color");
    }

    /// <summary>
    /// Loads the snakeColor scene. Saves the snake-color-scene as 'GraphicsSceneLastOpened' to an external file.
    /// </summary>
    public void LoadSnakeColorSkins()
    {
        SceneInteraction.Instance.GraphicsSceneLastOpened = GraphicsSceneLastOpened.snakeColor;
        SceneManager.LoadScene("Skins_Snake_Color");
    }
}
