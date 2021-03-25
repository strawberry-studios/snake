using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds static values which can and also need to be accessed by every script.
/// The values can hence only be changed in this script.
/// </summary>
public static class StaticValues 
{
    /// <summary>
    /// Whether the store on which the app is published can be used with the Unity IAP API or not. If not, App Purchases and the full version are
    /// not available.
    /// </summary>
    public static bool IAPsEnabled
    {
        //IF PUBLISHED ON A STORE WHICH SUPPORTS THE UNITY IAP API, UNCOMMENT:
        get { return true; }

        //IF PUBLISHED ON A STORE WHICH DOESN'T SUPPORT THE UNITY IAP API, UNCOMMENT:
        //get { return false; }
    }

    ///// <summary>
    ///// Whether the app will be published on the gooogle play store or not. Information is needed for setting up the IAPs. 
    ///// If returning true, the IAPs are associated with the Play Store, if false, with Amazon App Store.
    ///// </summary>
    //public static bool targetGooglePlay
    //{
    //    //IF PUBLISHED ON GOOGLE PLAY, UNCOMMENT:
    //    get { return true; }

    //    //IF PUBLISHED ON AMAZON APP STORE, UNCOMMENT:
    //    //get { return false; }
    //}

    /// <summary>
    /// The string identifier of the full version. 
    /// IMPORTANT: has to be pasted in the google play console as identifier of the "Full Version" product.
    /// DON'T ALTER!
    /// </summary>
    public static string productIDFullVersion
    {
        get { return "com.strawberrystudios.snake.fullversion"; }
    }

    /// <summary>
    /// The time until an info panel (UI image and text objects and their children) is closed.
    /// </summary>
    public static int TimeUntilClosureOfInfoPanel
    {
        get
        {
            return 10000;
        }
    }

    /// <summary>
    /// The time within which an info panel (UI image and text objects and their children) fades and fully disappears.
    /// </summary>
    public static int FadingTimeInfoPanel
    { get
        {
            return 400;
        }
    }

    /// <summary>
    /// The x position of the player when the game is started.
    /// </summary>
    public static int PlayerStartX
    {
        get { return 3; }
    }

    /// <summary>
    /// The y position of the player when the game is started.
    /// </summary>
    public static int PlayerStartY
    {
        get { return 3; }
    }

    /// <summary>
    /// The factor with which the size of all UIs can be modified to match the size of different mobile screens. 
    /// By multiplying the width (x-scale) and x-position with this factor the size is adjusted to different aspect ratios.
    /// This factor is set when the game is laoded, see 'IntroTheme' for the implementation
    /// </summary>
    public static float UIWidthFactor
    {
        //the default aspect ratio is 16:9, if the aspect ratio of a device is different, the UIs which were created for the ratio 16:9 must be 
        //modified: this is the factor the x-scale of the UIs needs to be multiplied by:
        get; set;
    }
}
