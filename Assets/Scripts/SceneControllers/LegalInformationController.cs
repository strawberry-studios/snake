using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LegalInformationController : MonoBehaviour
{
    /// <summary>
    /// Loads the "More Options" Scene.
    /// </summary>
    public void Back()
    {
        SceneManager.LoadScene("MoreOptions");
    }

    /// <summary>
    /// Opens the terms of use of Strawberry Studios.
    /// </summary>
    public void OpenTermsOfUse()
    {
        Application.OpenURL("http://www.thestrawberrystudios.com/legal-information/terms-of-use");
    }

    /// <summary>
    /// Opens the privacy policy of Strawberry Studios.
    /// </summary>
    public void OpenPrivacyPolicy()
    {
        Application.OpenURL("http://www.thestrawberrystudios.com/legal-information/privacy-policy");
    }
}
