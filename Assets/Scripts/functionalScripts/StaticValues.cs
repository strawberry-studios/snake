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

    /// <summary>
    /// If true, the app implements Appodeal ads, elsewhise not (only AdMob Ads).
    /// Appodeal ads should be implemented if publishing on Google Play, App Store or Amazon Store.
    /// </summary>
    public static bool appodealAds
    {        
        //IF PUBLISHED ON GOOGLE PLAY, APPLE APP STORE OR AMAZON APP STORE, UNCOMMENT:
        get { return true; }

        //IF NOT PUBLISHED ON GOOGLE PLAY, APPLE APP STORE OR AMAZON APP STORE, UNCOMMENT:
        //get { return false; }
    }

    /// <summary>
    /// The key of the appodeal app which this game is linked with.
    /// IMPORTANT: DEPENDING ON THE PLATFORM FOR WHICH THIS GAME IS DESIGNED, THE KEY HAS TO BE DIFFERENT!
    /// </summary>
    public static string appodealAppKey
    {
        //IF GOOGLE, UNCOMMENT: 
        //get { return "fa2bb48a4ed477841ade90237c01dc7e09f190ad887e70cc"; }

        //IF AMAZON APPS, UNCOMMENT: 
        get { return "028cb9eae36d44cc99bed96b65ae80002518ae53de1e047d"; }
    }

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
    /// Whether the ads which are shown are real ads or only test ads.
    /// If set to true, all ads are only test ads. During the testing this option must be enabled!
    /// If set to false, all ads are real, revenue-bringing ads. Only enable this option, if the game is actually published.
    /// The state of this variable can only be changed in the 'Static Values' script.
    /// </summary>
    public static bool AdsTestingEnabled
    {
        get
        {
            return false;
        }
    }

    /// <summary>
    /// The time until an info panel (UI image and text objects and their children) is closed.
    /// </summary>
    public static int timeUntilClosureOfInfoPanel
    {
        get
        {
            return 10000;
        }
    }

    /// <summary>
    /// The time within which an info panel (UI image and text objects and their children) fades and fully disappears.
    /// </summary>
    public static int fadingTimeInfoPanel
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
