using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using UnityEngine.SceneManagement;

/// <summary>
/// A delegate containing a (or several) void methods without arguments which are used for retrieving the user consent on ads personalization.
/// </summary>
public delegate void UserConsentMethod();

public class AdManager : Singleton<AdManager>
{
    /// <summary>
    /// The app key which is associated with this app. It is necessary to make sure that 'Appodeal' works properly. It can be looked up on the 
    /// appodeal website.
    /// </summary>
    string appKey;
    /// <summary>
    /// Whether the user allows data collection or not.
    /// </summary>
    bool consent;
    /// <summary>
    /// The time in milliseconds after which a shown banner ad is hidden again.
    /// </summary>
    int timeUntilHideBanner;
    /// <summary>
    /// Delegate containing methods used for getting the users choice on ads personalization.
    /// </summary>
    UserConsentMethod userConsent;
    /// <summary>
    /// The current full version data. BUT: it is only up to data, if it was also re-assigned when the fullVersionData was last altered.
    /// </summary>
    FullVersionData currentFullVersionData;
    /// <summary>
    /// Whether the sounds are on or not.
    /// </summary>
    bool soundOn;
    /// <summary>
    /// Whether the vibration is on or not.
    /// </summary>
    bool vibrationOn;


    // Start is called before the first frame update
    void Start()
    {
        appKey = "fa2bb48a4ed477841ade90237c01dc7e09f190ad887e70cc";
        consent = FullVersion.Instance.CollectionOfDataConsent == AdDataCollectionPermitted.permitted ? true : false;
        Appodeal.disableLocationPermissionCheck();
        Appodeal.setTesting(true);
        Appodeal.initialize(appKey, Appodeal.INTERSTITIAL | Appodeal.NON_SKIPPABLE_VIDEO | Appodeal.BANNER, consent);
        timeUntilHideBanner = 5000;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneExit;
        print("ads.initialized");
    }

    /// <summary>
    /// Can be called in order to create an 'AdManger' Singleton.
    /// The 'showBannerAdCounter' is also increased by one.
    /// </summary>
    public void CreateSingleton()
    {
        FullVersion.Instance.ShowBannerAdCounter++;
    }

    /// <summary>
    /// (Re)Initializes the appodeal ads.
    /// </summary>
    public void InitializeAds()
    {
        Appodeal.initialize(appKey, Appodeal.INTERSTITIAL | Appodeal.NON_SKIPPABLE_VIDEO | Appodeal.BANNER, consent);
    }

    /// <summary>
    /// Is called whenever a new scene is loaded.
    /// Determines whether a new banner ad should be shown.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FullVersionData data = FullVersion.Instance.RetrieveFullVersionDataFromFile();
        if (data.IsFullVersionUnlocked == FullVersionUnlocked.notUnlocked)
        {
            data.ShowBannerAdCounter++;
            LoadBannerAdIf(data, timeUntilHideBanner);
            //print(data.ShowBannerAdCounter);
        }
    }

    /// <summary>
    /// Is called whenever a scene is unloaded.
    /// All banner ads which are currently shown are hidden and all coroutines cancelled.
    /// </summary>
    void OnSceneExit(Scene scene)
    {
        HideBanner();
        StopAllCoroutines();
    }

    //actual ad-showing methods that take into account which ad was played recently and so on...

    /// <summary>
    /// Loads a banner ad if the currently loaded scene isn't the game scene and if the 'ShowBannerAdCounter' is greater or equal to 8.
    /// </summary>
    /// <param name="passedData">The current full version data.</param>
    /// <param name="timeInMillis">The time after which the ad should be hidden again.</param>
    void LoadBannerAdIf(FullVersionData passedData, int timeInMillis)
    {
        bool adShown = false;

        if (passedData.ShowBannerAdCounter >= 8 && SceneManager.GetActiveScene().name != "Game")
            adShown = ShowBannerAd(false); //if true, the banners are shown on top of the screen, elsewhise at the bottom

        if (adShown)
        {
            passedData.ShowBannerAdCounter = 0;
            StartCoroutine(HideBannerAfter(timeInMillis));
        }

        FullVersion.Instance.SaveFullVersionDataToFile(passedData);
    }

    /// <summary>
    /// Hides a banner ad after the passed time.
    /// </summary>
    /// <param name="timeInMillis">The time in milliseconds after which the banner ad should be hidded.</param>
    /// <returns></returns>
    IEnumerator HideBannerAfter(int timeInMillis)
    {
        yield return new WaitForSecondsRealtime(timeInMillis/1000f);
        HideBanner();
    }

    /// <summary>
    /// Loads whether the sounds and vibrations are on (data retrieved from an external file).
    /// </summary>
    /// <return>Returns the current player data.</return>
    void LoadVibrationAndSoundOn()
    {
        PlayerData d = DataSaver.Instance.RetrievePlayerDataFromFile();
        soundOn = d.SoundOn;
        vibrationOn = d.VibrationsOn;
        //return d;
    }

    /// <summary>
    /// Shows an interstitial ad if the 'adCounter' of 'FullVersionData' has reached at least 40. Yet if the last 2 times interstitials were 
    /// already shown, a non-skippable ad is shown this time. The 'adCounter' and 'interstitialsShown' are reset. 
    /// Special cases: If the 'adCounter' immidiately reached 80 or 120, a video ad is already shown if the interstitial was only played at 
    /// least 1/0 times so far.
    /// </summary>
    /// <param name="gameWon">Whether the game was won or not.</param>
    public void ShowVideoAdOnLose(bool gameWon)
    {
        if (!gameWon)
        { 
            FullVersionData data = FullVersion.Instance.RetrieveFullVersionDataFromFile();
            int adCounter = data.ShowAdCounter;
            LoadVibrationAndSoundOn();
            

            if (adCounter >= 50)
            {
                data = ShowInterstitialOrVideo(2, data);
            }
            else if (adCounter >= 100)
            {
                data = ShowInterstitialOrVideo(1, data);
            }
            else if (adCounter >= 150)
            {
                ShowNonSkippableVideo(data);
                data.InterstitialsShown = 0;
            }
            print(data.ShowAdCounter);

            FullVersion.Instance.SaveFullVersionDataToFile(data);
        }
    }

    /// <summary>
    /// Shows an interstitial ad if the 'adCounter' of 'FullVersionData' has reached at least 40. Yet if the last 2 times interstitials were 
    /// already shown, a non-skippable ad is shown this time. The 'adCounter' and 'interstitialsShown' are reset. 
    /// Special cases: If the 'adCounter' immidiately reached 80 or 120, a video ad is already shown if the interstitial was only played at 
    /// least 1/0 times so far.
    /// Note: If no ad is shown, the passed method is executed. (Also happens if the player wins)
    /// </summary>
    /// <param name="gameWon">Whether the game was won or not.</param>
    /// <param name="playGameOverSound">A method which plays a game over sound if no ad will be shown.</param>
    /// <param name="playVibration">A method which makes the device vibrate if no ad is shown.</param>
    public void ShowVideoAdOnLose(bool gameWon, SoundMethod playGameOverSound, SoundMethod playVibration)
    {
        LoadVibrationAndSoundOn();

        if (gameWon)
        { 
            playGameOverSound(true, gameWon);
            playVibration(true, gameWon);
        }
        else
        {
            FullVersionData data = FullVersion.Instance.RetrieveFullVersionDataFromFile();
            int adCounter = data.ShowAdCounter;

            if (adCounter >= 50)
            {
                data = ShowInterstitialOrVideo(2, data, playGameOverSound, playVibration);
            }
            else if (adCounter >= 100)
            {
                data = ShowInterstitialOrVideo(1, data, playGameOverSound, playVibration);
            }
            else if (adCounter >= 150)
            {
                data = ShowNonSkippableVideo(data, playGameOverSound, playVibration);
                data.InterstitialsShown = 0;
            }
            else
                playGameOverSound(true, false);

            FullVersion.Instance.SaveFullVersionDataToFile(data);
        }
    }

    /// <summary>
    /// Shows an interstitial or a non-skippable video ad.
    /// </summary>
    /// <param name="interstitialsMax">The number of interstitials which must have already been watched to show a video.</param>
    /// <param name="data">The current 'FullVersionData'.</param>
    /// <returns>Returns the 'FullVersionData' object with all of the alterations that were made.</returns>
    FullVersionData ShowInterstitialOrVideo(int interstitialsMax, FullVersionData data)
    {
        if (data.InterstitialsShown < interstitialsMax)
        {
            data = ShowInterstitial(data);
            data.InterstitialsShown++;
        }
        else
        {
            data = ShowNonSkippableVideo(data);
            data.InterstitialsShown = 0;
        }
        print(data.ShowAdCounter);
        return data;
    }

    /// <summary>
    /// Shows an interstitial or a non-skippable video ad.
    /// </summary>
    /// <param name="interstitialsMax">The number of interstitials which must have already been watched to show a video.</param>
    /// <param name="data">The current 'FullVersionData'.</param>
    /// <param name="playGameOverSound">A method which plays a game over sound if no ad is to be shown.</param>
    /// <param name="playVibration">A method which makes the device vibrate if no ad is shown.</param>
    /// <returns>Returns the 'FullVersionData' object with all of the alterations that were made.</returns>
    FullVersionData ShowInterstitialOrVideo(int interstitialsMax, FullVersionData data, SoundMethod playGameOverSound, SoundMethod playVibration)
    {
        if (data.InterstitialsShown < interstitialsMax)
        {
            data = ShowInterstitial(data, playGameOverSound, playVibration);
            data.InterstitialsShown++;
        }
        else
        {
            data = ShowNonSkippableVideo(data, playGameOverSound, playVibration);
            data.InterstitialsShown = 0;
        }
        return data;
    }

    //methods only showing/hiding ads:

    /// <summary>
    /// Shows a banner ad, if it can be loaded.
    /// </summary>
    /// <param name="showAtTop">If true, the banner ad is shown on top of the screen, elsewhise it is shown at its bottom.</param>
    /// <returns>Returns true if the banner ad could be loaded, elsewhise false is returned.</returns>
    public bool ShowBannerAd(bool showAtTop)
    {
        if (Appodeal.isLoaded(Appodeal.BANNER))
        {
            if (FullVersion.Instance.CollectionOfDataConsent != AdDataCollectionPermitted.notSet)
            {
                if (showAtTop) ShowLoadedBannerTop();
                else ShowLoadedBannerBottom();
            }
            else
            {
                if (showAtTop) userConsent = ShowLoadedBannerTop;
                else userConsent = ShowLoadedBannerBottom;

                AskUserForAdsPersonalizationContent(userConsent);
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Shows a banner ad at the top without testing whether it was loaded or not.
    /// </summary>
    void ShowLoadedBannerTop()
    {
            Appodeal.show(Appodeal.BANNER_TOP);
    }

    /// <summary>
    /// Shows a banner ad at the bottom without testing whether it was loaded or not.
    /// </summary>
    void ShowLoadedBannerBottom()
    {
        Appodeal.show(Appodeal.BANNER_BOTTOM);
    }

    /// <summary>
    /// Opens the panel where the user is asked whether they consent with the personalizatin of ads or not and executes the passed method afterwards.
    /// If the panel can't be opened, the user consent is automatically set to false (not changed).
    /// </summary>
    /// <param name="currentUserConsent">The method which should be executed after the ads personalization selection was made by the user.</param>
    void AskUserForAdsPersonalizationContent(UserConsentMethod currentUserConsent)
    {
        GameObject userConsentPanel = GameObject.FindGameObjectWithTag("PersonalizedAdsConsent");
        if (userConsentPanel != null)
            userConsentPanel.GetComponent<AdsPersonalizationConsent>().TogglePanelsActive(true, currentUserConsent);
        else
            currentUserConsent();
    }

    /// <summary>
    /// Hides a banner ad.
    /// </summary>
    public void HideBanner()
    {
        Appodeal.hide(Appodeal.BANNER);
    }

    /// <summary>
    /// Shows an interstitial ad. If the ad can be played, the 'showAdCounter' of 'FullVersionData' is reset.
    /// If the ads personalization wasn't set yet, a panel is opened at first, where the user can save their choice.
    /// </summary>
    /// <param name="thisData">The current full version data that might be altered (only the ad controlling components).</param>
    /// <returns>Returns the altered full version data.</returns>
    public FullVersionData ShowInterstitial(FullVersionData thisData)
    {
        if (Appodeal.isLoaded(Appodeal.INTERSTITIAL))
        {
            currentFullVersionData = thisData;

            if (FullVersion.Instance.CollectionOfDataConsent != AdDataCollectionPermitted.notSet)
                ShowLoadedInterstitial();
            else
            {
                userConsent = ShowLoadedInterstitial;
                AskUserForAdsPersonalizationContent(userConsent);
            }
        }
        return thisData;
    }

    /// <summary>
    /// Shows an interstitial ad. If the ad can be played, the 'showAdCounter' of 'FullVersionData' is reset.
    /// If the ads personalization wasn't set yet, a panel is opened at first, where the user can save their choice.
    /// </summary>
    /// <param name="playGameOverSound">A method which plays a game over sound if no ad will be shown.</param>
    /// <param name="playVibration">A method which makes the device vibrate if no ad is shown.</param>
    /// <param name="thisData">The current full version data that might be altered (only the ad controlling components).</param>
    /// <returns>Returns the altered full version data.</returns>
    public FullVersionData ShowInterstitial(FullVersionData thisData, SoundMethod playGameOverSound, SoundMethod playVibration)
    {
        if (Appodeal.isLoaded(Appodeal.INTERSTITIAL))
        {
            currentFullVersionData = thisData;

            if (FullVersion.Instance.CollectionOfDataConsent != AdDataCollectionPermitted.notSet)
                ShowLoadedInterstitial();
            else
            {
                userConsent = ShowLoadedInterstitial;
                AskUserForAdsPersonalizationContent(userConsent);
            }
        }
        else
        {
            playGameOverSound(true, false);
            playVibration(true, false);
        }
        return thisData;
    }

    /// <summary>
    /// Shows a loaded interstitial without checking whether it was loaded.
    /// </summary>
    void ShowLoadedInterstitial()
    {
        Appodeal.show(Appodeal.INTERSTITIAL);
        currentFullVersionData.ShowAdCounter = 0;
    }

    /// <summary>
    /// Shows a non-skippable video. If the ad can be played, the 'showAdCounter' of 'FullVersionData' is reset.
    /// </summary>
    /// <param name="thisData">The current full version data that might be altered (only the ad controlling components).</param>
    /// <returns>Returns the altered full version data.</returns>
    public FullVersionData ShowNonSkippableVideo(FullVersionData thisData)
    {
        if (Appodeal.isLoaded(Appodeal.NON_SKIPPABLE_VIDEO))
        {
            currentFullVersionData = thisData;

            if (FullVersion.Instance.CollectionOfDataConsent != AdDataCollectionPermitted.notSet)
                ShowLoadedNonSkippableVideo();
            else
            {
                userConsent = ShowLoadedNonSkippableVideo;
                AskUserForAdsPersonalizationContent(userConsent);
            }
        }
        return thisData;
    }

    /// <summary>
    /// Shows a non-skippable video. If the ad can be played, the 'showAdCounter' of 'FullVersionData' is reset.
    /// </summary>
    /// <param name="playGameOverSound">A method which plays a game over sound if no ad will be shown.</param>
    /// <param name="playVibration">A method which makes the device vibrate if no ad is shown.</param>
    /// <param name="thisData">The current full version data that might be altered (only the ad controlling components).</param>
    /// <returns>Returns the altered full version data.</returns>
    public FullVersionData ShowNonSkippableVideo(FullVersionData thisData, SoundMethod playGameOverSound, SoundMethod playVibration)
    {
        if (Appodeal.isLoaded(Appodeal.NON_SKIPPABLE_VIDEO))
        {
            currentFullVersionData = thisData;

            if (FullVersion.Instance.CollectionOfDataConsent != AdDataCollectionPermitted.notSet)
                ShowLoadedNonSkippableVideo();
            else
            {
                userConsent = ShowLoadedNonSkippableVideo;
                AskUserForAdsPersonalizationContent(userConsent);
            }
        }
        else
        {
            playGameOverSound(true, false);
            playVibration(true, false);
        }
        return thisData;
    }

    /// <summary>
    /// Shows a loaded non skippable video without checking whether it was loaded.
    /// </summary>
    void ShowLoadedNonSkippableVideo()
    {
        Appodeal.show(Appodeal.NON_SKIPPABLE_VIDEO);
        currentFullVersionData.ShowAdCounter = 0;
    }
}
