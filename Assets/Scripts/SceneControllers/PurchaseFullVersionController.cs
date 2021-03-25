using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PurchaseFullVersionController : MonoBehaviour, IPurchase
{
    public GameObject infoPanel, blocker;
    public Text infoPanelText;
    private int timeUntilClosureOfInfoPanel, fadingTimeInfoPanel; //the time how long the info panel is maximally opened, the time within which it fades again
    /// <summary>
    /// The price which you have to pay for purchasing the full version.
    /// </summary>
    public TextMeshProUGUI priceText;

    private void Awake()
    {
        SceneManager.sceneUnloaded += OnSceneExit;
    }

    // Start is called before the first frame update
    void Start()
    {
        timeUntilClosureOfInfoPanel = (int)(StaticValues.timeUntilClosureOfInfoPanel * 1.5f);
        fadingTimeInfoPanel = StaticValues.fadingTimeInfoPanel;
        infoPanel.SetActive(false);
        blocker.SetActive(false);
        StartCoroutine(IncreaseSize(priceText));
    }

    //private void OnGUI()
    //{
    //    if (GUI.Button(new Rect(Screen.width/2, Screen.height - 100, 500, 200), "DismissFullVersion"))
    //        DismissFullVersion();
    //}

    /// <summary>
    /// This method is always executed when the scene is unloaded.
    /// </summary>
    void OnSceneExit(Scene scene)
    {
        CoroutinesSingleton.Instance.StopClosingUIObjectAutomatically();
    }

    /// <summary>
    /// Gradually increases the size of the 'full version price' text. If the maximum font size is reached, it starts decreasing again.
    /// The animation ends when the scene is changed.
    /// </summary>
    /// <param name="thisText">The text componenent holding the text which is to be animated.</param>
    /// <returns></returns>
    IEnumerator IncreaseSize(TextMeshProUGUI thisText)
    {
        while (thisText.fontSize < 50)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            thisText.fontSize++;
        }
        StartCoroutine(DecreaseSize(thisText));
    }

    /// <summary>
    /// Gradually decreases the size of the 'full version price' text. If the maximum font size is reached, it starts decreasing again.
    /// The animation ends when the scene is changed.
    /// </summary>
    /// <param name="thisText">The text componenent holding the text which is to be animated.</param>
    /// <returns></returns>
    IEnumerator DecreaseSize(TextMeshProUGUI thisText)
    {
        while (thisText.fontSize > 42)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            thisText.fontSize--;
        }
        StartCoroutine(IncreaseSize(thisText));
    }

    // info panel related methods:

    /// <summary>
    /// Toggles the info panel (in)active. If it is toggled active, it will start fading after 'timeUntilClosureOfPanel' and it'll fade within
    /// 'fadingTimeInfoPanel'. When it is closed the invokes/coroutines closing it automatically are cancelled.
    /// </summary>
    /// <param name="newActivityStatus">The new activity status of the info panel.</param>
    public void ToggleInfoPanelActive(bool newActivityStatus)
    {
        infoPanel.SetActive(newActivityStatus);
        blocker.SetActive(newActivityStatus);
        if (newActivityStatus)
        {
            CoroutinesSingleton.Instance.CloseUIObjectAutomatically(infoPanel, timeUntilClosureOfInfoPanel, fadingTimeInfoPanel, null, blocker) ;
        }
        else
            CoroutinesSingleton.Instance.StopClosingUIObjectAutomatically();
    }

    /// <summary>
    /// Opens the info panel and shows a passed error message informing the player why the purchase couldn't be made.
    /// </summary>
    /// <param name="errorMessage">The error message as string.</param>
    public void ShowErrorMessageOnPanel(string errorMessage)
    {
        ToggleInfoPanelActive(true);
        infoPanelText.text = errorMessage;
    }

    // to be attached to buttons:

    ///// <summary>
    ///// Locks the full version - only for test purposes. The info is saved to an external file.
    ///// </summary>
    //public void DismissFullVersion()
    //{
    //    FullVersion.Instance.IsFullVersionUnlocked = FullVersionUnlocked.notUnlocked;
    //}

    /// <summary>
    /// Opens the scene where the 'FullVersion' can be restored.
    /// </summary>
    public void OpenRestoreFullVersion()
    {
        SceneManager.LoadScene("RestoreFullVersion");
    }
}
