using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// This class manages the 'restoring' of the full version. Its methods make it possible for players to restore the full version
/// if the file containing the information was deleted. Elsewhise an info panel opens telling them why they can't restore the full
/// version or that they haven't unlocked the full version yet.
/// </summary>
public class RestoreFullVersionController : MonoBehaviour, IPurchase
{
    public GameObject infoPanel, blocker;
    public Text infoPanelText;
    private int timeUntilClosureOfInfoPanel, fadingTimeInfoPanel;
    public GameObject troubleshootingLink;

    private void Awake()
    {
        SceneManager.sceneUnloaded += OnSceneExit;
    }

    // Start is called before the first frame update
    void Start()
    {
        timeUntilClosureOfInfoPanel = (int)(StaticValues.TimeUntilClosureOfInfoPanel * 1.5f);
        fadingTimeInfoPanel = StaticValues.FadingTimeInfoPanel;
        infoPanel.SetActive(false);
        blocker.SetActive(false);
        troubleshootingLink.SetActive(false);
    }

    void OnSceneExit(Scene scene)
    {
        CoroutinesSingleton.Instance.StopClosingUIObjectAutomatically();
    }

    //methods for showing an error message on a panel:

    /// <summary>
    /// Opens the info panel and shows a passed error message informing the player why the purchase couldn't be made.
    /// </summary>
    /// <param name="errorMessage">The error message as string.</param>
    public void ShowErrorMessageOnPanel(string errorMessage)
    {
        ToggleInfoPanelActive(true);
        infoPanelText.text = errorMessage;
    }

    /// <summary>
    /// Shows a text which informs the player that the full version wasn't successfully restored. 
    /// Also unhides a link which links the troubleshooting section of the "Strawberry Studios" website.
    /// It can be consulted for further information.
    /// </summary>
    public void ShowRestoreFullVersionFailed()
    {
        ToggleInfoPanelActive(true);
        infoPanelText.text = "The Full Version couldn't be restored. " +
            "\nIt wasn't unlocked on this account." +
            "\nIf you are certain that you unlocked the Full Version, follow the instructions described on";
        troubleshootingLink.SetActive(true);
    }

    /// <summary>
    /// Opens the section of the Strawberry Studios website where it is explained how you can restore the Full Version.
    /// </summary>
    public void OpenTroubleshootingHyperlink()
    {
        Application.OpenURL("http://www.thestrawberrystudios.com/our-games/snake/restoring-the-full-version");
    }

    /// <summary>
    /// Toggles the info panel (in)active. If it is toggled active, it will start fading after 'timeUntilClosureOfPanel' and it'll fade within
    /// 'FadingTimeInfoPanel'. When it is closed the invokes/coroutines closing it automatically are cancelled.
    /// </summary>
    /// <param name="newActivityStatus">The new activity status of the info panel.</param>
    public void ToggleInfoPanelActive(bool newActivityStatus)
    {
        infoPanel.SetActive(newActivityStatus);
        blocker.SetActive(newActivityStatus);
        troubleshootingLink.SetActive(false);
        if (newActivityStatus)
        {
            CoroutinesSingleton.Instance.CloseUIObjectAutomatically(infoPanel, timeUntilClosureOfInfoPanel, fadingTimeInfoPanel, null, blocker);
        }
        else
        {
            CoroutinesSingleton.Instance.StopClosingUIObjectAutomatically();
        }
    }

    //scene interaction - to be attached to buttons:

    /// <summary>
    /// Opens the scene where the full version can be purchased.
    /// </summary>
    public void BackToPurchaseFullVersion()
    {
        SceneManager.LoadScene("PurchaseFullVersion");
    }
}
