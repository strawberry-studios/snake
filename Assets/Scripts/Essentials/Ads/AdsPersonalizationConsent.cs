using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages a consent window which is opened when the an ad is played for the very first time.
/// After the player made a selection (whether they want to view personalized ads or not) this panel will never be opened again.
/// This object exists exactly once. It is never destroyed on load. If a second instance is created, it is immidiately deleted again.
/// </summary>
public class AdsPersonalizationConsent : MonoBehaviour
{
    /// <summary>
    /// References all panels which are needed for the ads personalization consent settings.
    /// </summary>
    [Serializable]
    public class Panels
    {
        /// <summary>
        /// The header of the ads personalization panel (which just says Appodeal)
        /// </summary>
        public GameObject header;
        /// <summary>
        /// The panel where the player can decide whether they want to view personalized ads.
        /// </summary>
        public GameObject consentSelectionPanel;
        /// <summary>
        /// The panel which is opened if the player denied their content for ads personalization.
        /// </summary>
        public GameObject consentDeniedPanel;
        /// <summary>
        /// The panel which is opened if the player gave their content for ads personalization.
        /// </summary>
        public GameObject consentGivenPanel;
    }

    /// <summary>
    /// The panels which manage the data consent of the player.
    /// </summary>
    public Panels panels;
    /// <summary>
    /// Delegate containing methods which should be executed when the panel is closed (after a selection was made).
    /// </summary>
    UserConsentMethod consentMethod;

    // Start is called before the first frame update
    void Start()
    {
        ////if the ads personalization wasn't set yet, this game object shouldn't be destroyed on load:
        //if (FullVersion.Instance.CollectionOfDataConsent == AdDataCollectionPermitted.notSet)
        if (GameObject.FindGameObjectsWithTag("PersonalizedAdsConsent").Length < 2)
            DontDestroyOnLoad(gameObject);
        else
            Destroy(gameObject);
        TogglePanelsActive(false);
    }

    /// <summary>
    /// Opens the panel where the player can decide whether the ads should be personalized or not.
    /// </summary>
    void OpenDataCollectionPanel()
    {
        panels.consentDeniedPanel.SetActive(false);
        panels.consentGivenPanel.SetActive(false);
        panels.consentSelectionPanel.SetActive(true);
    }

    //to be attached to buttons of the ads personalization panel:

    /// <summary>
    /// Opens or closes the personal data consent panel. If it is opened, the ads personalization selection panel will be opened.
    /// </summary>
    /// <param name="active">Whether the panels should be activated or deactivated.</param>
    public void TogglePanelsActive(bool active)
    {
        panels.header.SetActive(active);
        if(active)
            OpenDataCollectionPanel();
        else
        {
            panels.consentSelectionPanel.SetActive(false);
            panels.consentGivenPanel.SetActive(false);
            panels.consentDeniedPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Opens or closes the personal data consent panel. If it is opened, the ads personalization selection panel will be opened.
    /// </summary>
    /// <param name="active">Whether the panels should be activated or deactivated.</param>
    public void TogglePanelsActive(bool active, UserConsentMethod currentUserConsent)
    {
        panels.header.SetActive(active);
        consentMethod = currentUserConsent;
        if (active)
            OpenDataCollectionPanel();
        else
        {
            panels.consentSelectionPanel.SetActive(false);
            panels.consentGivenPanel.SetActive(false);
            panels.consentDeniedPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Toggles all ads personalization panels inactive and also undoes the 'DontDestroyOnLoad'.
    /// If a method was attached to the 'ConsentMethod' delegate, it is played now.
    /// </summary>
    public void ClosePanelsAfterSelection()
    {
        TogglePanelsActive(false);
        //SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());

        //the ads are initialized with the new consent and the consentMethod is played:
        AdManager.Instance.InitializeAds();
        consentMethod();
    }

    /// <summary>
    /// Opens the privacy policy of Appodeal. (In the browser.)
    /// </summary>
    public void OpenAppodealPrivacy()
    {
        Application.OpenURL("http://www.appodeal.com/home/privacy-policy/");
    }

    /// <summary>
    /// Saves the selected option of the player (whether the ads should be personalized or not) to an external file.
    /// If true is passed, the ads will be personalized, elsewhise not.
    /// </summary>
    /// <param name="consentGiven">Whether the ads should be personalized or not.</param>
    public void SetAdsPersonalizationContent(bool consentGiven)
    {
        if (consentGiven)
        {
            FullVersion.Instance.CollectionOfDataConsent = AdDataCollectionPermitted.permitted;
            panels.consentSelectionPanel.SetActive(false);
            panels.consentGivenPanel.SetActive(true);
        }
        else
        {
            FullVersion.Instance.CollectionOfDataConsent = AdDataCollectionPermitted.denied;
            panels.consentSelectionPanel.SetActive(false);
            panels.consentDeniedPanel.SetActive(true);
        }
    }
}
