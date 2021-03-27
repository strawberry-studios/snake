using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PrivacySettingsController : MonoBehaviour
{
    /// <summary>
    /// The toggles showing granted and denyed consent when it comes to ads personalization
    /// </summary>
    public Toggle consentGiven, consentDenied;
    /// <summary>
    /// Whether the personalization of ads is granted or not.
    /// </summary>
    AdDataCollectionPermitted adPersonalizationAllowed;

    // Start is called before the first frame update
    void Start()
    {
        LoadToggleStates();   
    }

    /// <summary>
    /// Loads the toggle states of the consent-given and consent-denied toggles from an external file.
    /// </summary>
    private void LoadToggleStates()
    {
        FullVersionData d = FullVersion.Instance.RetrieveFullVersionDataFromFile();
        adPersonalizationAllowed = d.CollectionOfDataConsent;

        if(adPersonalizationAllowed == AdDataCollectionPermitted.permitted)
        {
            SetConsentToggleStates(true);
        }
        else
        {
            SetConsentToggleStates(false);

            //should never occur, but if this scene is opened even though the dataCollection state wasn't set yet, it is set to 'denied'.
            if (d.CollectionOfDataConsent == AdDataCollectionPermitted.notSet)
                FullVersion.Instance.CollectionOfDataConsent = AdDataCollectionPermitted.denied;
        }
    }

    /// <summary>
    /// Sets the consent toggle states of the consent-granted and consent-denied toggles. 
    /// </summary>
    /// <param name="consented">If true, the toggle consent-granted is ticked and consent-denied isn't and vice versa.</param>
    void SetConsentToggleStates(bool consented)
    {
        if (consented)
        {
            consentGiven.isOn = true;
            consentDenied.isOn = false;
            consentDenied.interactable = true;
            consentGiven.interactable = false;
        }
        else
        {
            consentGiven.isOn = false;
            consentDenied.isOn = true;
            consentDenied.interactable = false;
            consentGiven.interactable = true;
        }
    }

    /// <summary>
    /// Saves a new consent value and adjusts the positions of the toggles.
    /// Should be attached to the consent-granted toggle.
    /// </summary>
    /// <param name="consented">New state of (ads personalization) 'consent-granted' option.</param>
    public void SaveNewConsentGrantedToggleState(bool consented)
    {
        if (consented)
        {
            consentDenied.isOn = false;
            consentDenied.interactable = true;
            consentGiven.interactable = false;
        }
        else Debug.Log("This toggle should only be interactable when it is toggled of. Check the code for errors.");

        adPersonalizationAllowed = consented ? AdDataCollectionPermitted.permitted : AdDataCollectionPermitted.denied;
    }

    /// <summary>
    /// Saves a new consent value and adjusts the positions of the toggles.
    /// Should be attached to the consent-denied toggle.
    /// </summary>
    /// <param name="consented">New state of (ads personalization) 'consent-Denied' option.</param>
    public void SaveNewConsentDeniedToggleState(bool consented)
    {
        if (consented)
        {
            consentGiven.isOn = false;
            consentDenied.interactable = false;
            consentGiven.interactable = true;
        }
        else Debug.Log("This toggle should only be interactable when it is toggled of. Check the code for errors.");

        adPersonalizationAllowed = !consented ? AdDataCollectionPermitted.permitted : AdDataCollectionPermitted.denied;
    }

    /// <summary>
    /// Opens the appodeal privacy policy in the browser.
    /// </summary>
    public void OpenAppodealPrivacyPolicy()
    {
        Application.OpenURL("http://www.appodeal.com/home/privacy-policy/");
    }

    /// <summary>
    /// Saves all of the changes and reloads the more options scene.
    /// </summary>
    public void SaveChanges()
    {
        FullVersion.Instance.CollectionOfDataConsent = adPersonalizationAllowed;
        Back();
    }

    /// <summary>
    /// Dismisses all changes and returns to the more options scene.
    /// </summary>
    public void DismissChanges()
    {
        Back();
    }

    /// <summary>
    /// Opens the more options scene again.
    /// </summary>
    void Back()
    {
        SceneManager.LoadScene("MoreOptions");
    }
}
