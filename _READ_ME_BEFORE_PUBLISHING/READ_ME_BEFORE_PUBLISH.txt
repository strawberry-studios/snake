The Snake app can be released on on several different stores. Depending on the store,
on which it is released, the following values must be checked before every build!
The script "StaticValues" implements 2 very important properties: 
	- IAPsEnabled

	//if Ads are enabled, it also implements:
	- testing enabled	
	- appodealAds
	- appodealAppKey

Make sure that...
- IAPs enabled is only true if publishing on Amazon/Apple/Google Play/Samsung (stores supported by the Unity IAP API)
	ONLY THEN IAPsEnabled CAN BE SET TO TRUE
- if publishing with IAPs, choose the right platform for the IAPs (Window -> IAP Plugin -> Android -> Google/Amazon)
- when you publish for Google Play, the App Id is 'com.StrawberyStudios.Snake', 
  for Amazon and everything else it is            'com.StrawberryStudios.Snake'

//only if Appodeal Ads are used:
- Appodeal ads should only be enabled if publishing on Amazon/Apple/Google Play
	ONLY THEN appodealAds CAN BE SET TO TRUE
- if appodealAds is set to true, the appodealAppKey variable must match the key of the related Appodeal App
	ONE APP KEY AND BUILD FOR EACH APPODEAL-IMPLEMENTING BUILD
- testing enabled is set to false: otherwise no real ads are served!


CREATE THE FOLLOWING VERSIONS FOR PUBLISHING:

- for Google Play: with IAPs, IAP-Target: Google (and the google play appodeal app key)
- for Amazon App Store: with IAPs, IAP-Target: Amazon (and the amazon appodeal app key)

- for all apps with IAPs and normal ads (Unity Ads - easiest opportunity - works for every store)
	- e.g. Samsung, Microsoft

- for all apps without IAPs and normal ads (Unity Ads)
	- everything else

IMPORTANT: All apps that don't integrate IAPs will lack some functionality (perhaps they are 
concepted slightly differently)
