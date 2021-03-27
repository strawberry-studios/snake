using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Monetization;

public class PopUpAdsManager : Singleton<PopUpAdsManager>
{
    string gameId;
    bool testMode = true;
    bool setUp = false;

    public void SetUpMonetization()
    {
        if (!setUp)
        {
            SetGameId();
            if (Monetization.isSupported)
                Monetization.Initialize(gameId, testMode);
            setUp = true;
        }
    }

    public void ShowAd(string placementId)
    {
        if(Monetization.IsReady(placementId))
        {
            ShowAdPlacementContent ad = Monetization.GetPlacementContent(placementId) as ShowAdPlacementContent;
            ad.Show();
        }
    }

    private void SetGameId()
    {
        #if UNITY_IOS
                        gameId = "3130122";
        #elif UNITY_ANDROID
                gameId = "3130123";
        #endif
    }
}
