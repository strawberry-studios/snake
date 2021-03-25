using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuInteractionManager : MonoBehaviour
{
    /// <summary>
    /// The text informing the player that the full version is unlocked.
    /// </summary>
    public TextMeshProUGUI fullVersionUnlockedText;
    /// <summary>
    /// The button with which the 'getFullVersion' scene can be loaded.
    /// </summary>
    public GameObject unlockFullVersionButton;

    //private void OnGUI()
    //{
    //    if (GUI.Button(new Rect(100, 100, Screen.width / 2, Screen.height / 2), "notSetConsent"))
    //        FullVersion.Instance.CollectionOfDataConsent = AdDataCollectionPermitted.notSet;
    //}

    private void Start()
    {
        //FullVersion.Instance.GetDeviceUniqueIdentifier();
        SetUpFullVersionUIs();
    }

    /// <summary>
    /// Sets up the full version UIs. If the full version is unlocked the 'unlockFullVersionButton' is set inactive and the 'fullVersionUnlocked'
    /// text is set active. Elsewhise 'fullVersionUnlocked' text is set inactive and 'unlockFullVersionButton' is set active.
    /// </summary>
    public void SetUpFullVersionUIs()
    {
        if (StaticValues.IAPsEnabled)
        {
            if (FullVersion.Instance.IsFullVersionUnlocked == FullVersionUnlocked.unlocked)
            {
                fullVersionUnlockedText.enabled = true;
                unlockFullVersionButton.SetActive(false);
                //title.transform.position = new Vector3(title.transform.position.x, title.transform.position.y + 15, title.transform.position.z); 
            }
            else
            {
                fullVersionUnlockedText.enabled = false;
                unlockFullVersionButton.SetActive(true);
            }
        }
        else
        {
            fullVersionUnlockedText.enabled = false;
            unlockFullVersionButton.SetActive(false);
        }
    }

    //to be attached to buttons:

    /// <summary>
    /// Loads the options scene which was opened at last. If none can be found (if the data wasn't saved), the difficulty scene is opened.
    /// </summary>
    public void Options()
    {
        switch(SceneInteraction.Instance.OptionsSceneLastOpened)
        {
            case OptionsSceneLastOpened.difficulty:
                SceneManager.LoadScene("Difficulty");
                break;
            case OptionsSceneLastOpened.preferences:
                SceneManager.LoadScene("Preferences");
                break;
        }
    }

    /// <summary>
    /// Loads the graphics scene which was opened at last. If none can be found (if the data wasn't saved), the graphics settings scene is opened.
    /// </summary>
    public void OpenLastGraphicsScene()
    {
        switch (SceneInteraction.Instance.GraphicsSceneLastOpened)
        {
            case GraphicsSceneLastOpened.graphicsSettings:
                SceneManager.LoadScene("GraphicsSettings");
                break;
            case GraphicsSceneLastOpened.snakeColor:
                SceneManager.LoadScene("Skins_Snake_Color");
                break;
            case GraphicsSceneLastOpened.collectablesColor:
                SceneManager.LoadScene("Skins_Collectables_Color");
                break;
        }
    }

    /// <summary>
    /// Opens the 'moreOptions' scene.
    /// </summary>
    public void OpenMoreOptionsScene()
    {
        SceneManager.LoadScene("MoreOptions");
    }

    /// <summary>
    /// Loads the game.
    /// </summary>
    public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// Loads the highScores scene.
    /// </summary>
    public void HighScores()
    {
        SceneManager.LoadScene("HighScores");
    }

    /// <summary>
    /// Opens the scene where the full version of the game can be purchased.
    /// </summary>
    public void OpenFullVersionPurchaseTab()
    {
        SceneManager.LoadScene("PurchaseFullVersion");
    }

    ///// <summary>
    ///// Returns 1 if passed true or -1 if passed false.
    ///// </summary>
    ///// <param name="myBool">Parameter value to pass.</param>
    ///// <returns>Returns an integer based on the passed value.</returns>
    //public bool FEW(bool myBool)
    //{
    //    return myBool;
    //}
}
