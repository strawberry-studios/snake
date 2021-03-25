using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    /// <summary>
    /// Panel where the message: "The email address was successfully copied to the clipboard" is displayed.
    /// </summary>
    public GameObject messagePanel;

    // Start is called before the first frame update
    void Start()
    {
        messagePanel.SetActive(false);
        SceneManager.sceneUnloaded += OnSceneExit;
    }

    /// <summary>
    /// Is called when the scene is unloaded.
    /// </summary>
    /// <param name="scene"></param>
    void OnSceneExit(Scene scene)
    {
        //StopAllCoroutines();
        //CancelInvoke();
    }

    /// <summary>
    /// Opens the website of Strawberry Studios.
    /// </summary>
    public void OpenStrawberryStudiosWebsite()
    {
        Application.OpenURL("http://www.thestrawberrystudios.com");
    }

    /// <summary>
    /// Copies the support eMail address to the clipboard of the user's device and informs them about it.
    /// </summary>
    public void SupportEMailToClipboard()
    {
        string email = "support@theStrawberryStudios.com"; 
        CopyTextToDeviceClipboard(email);
        ToggleShowMessagePanelActive(true);
        Invoke("CloseMessagePanel", 4);
    }

    /// <summary>
    /// Closes the message panel immidiately.
    /// </summary>
    /// <returns></returns>
    void CloseMessagePanel()
    {
        ToggleShowMessagePanelActive(false);
    }

    ///// <summary>
    ///// Shows the user a message which informs them that the email address was successfully copied to the clipboard.
    ///// </summary>
    //void ShowCopiedToClipboardMessage()
    //{
    //    ToggleShowMessagePanelActive()
    //}

    /// <summary>
    /// Toggles the panel (which informs the player that the text was successfully copied to the clipboard) on or off.
    /// </summary>
    /// <param name="active">Whether the panel should be active or inactive.</param>
    public void ToggleShowMessagePanelActive(bool active)
    {
        messagePanel.SetActive(active);
        if (!active)
            CancelInvoke();
    }

    /// <summary>
    /// Copies a passed text to the clipboard (of an android device, the IOS plugin wasn't added).
    /// </summary>
    /// <param name="textToBeCopied">The text to be copied to the clipboard.</param>
    void CopyTextToDeviceClipboard(string textToBeCopied)
    {
        UniClipboard.SetText(textToBeCopied);
    }
}
