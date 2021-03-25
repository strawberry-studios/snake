using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

/// <summary>
/// This singleton provides methods/properties for getting or setting whether the full version of a game was purchased. The data is stored 
/// along with a device specific ID to prevent successful copying and cheating. Also holds the data whether the game was opened for the very 
/// first time or not.
/// </summary>
public class FullVersion : Singleton<FullVersion>
{
    /// <summary>
    /// Get or set whether the full version of the game was unlocked. When a new state is saved, the device unique identifier is also saved.
    /// When the current state is returned, true is only returned if the saved device identifier matches the identifier of the device.
    /// Theryby cheating is prevented.
    /// </summary>
    public FullVersionUnlocked IsFullVersionUnlocked
    {
        get
        {
            FullVersionData data = RetrieveFullVersionDataFromFile();
            return data.IsFullVersionUnlocked;
        }
        set
        {
            FullVersionData data = RetrieveFullVersionDataFromFile();
            data.IsFullVersionUnlocked = value;
            SaveFullVersionDataToFile(data);
        }
    }

    /// <summary>
    /// Get or set whether the game was loaded for the first time.
    /// </summary>
    public GameFirstLoaded IsGameFirstLoaded
    {
        get
        {
            FullVersionData data = RetrieveFullVersionDataFromFile();
            return data.IsGameFirstLoaded;
        }
        set
        {
            FullVersionData data = RetrieveFullVersionDataFromFile();
            data.IsGameFirstLoaded = value;
            SaveFullVersionDataToFile(data);
        }
    }

    /// <summary>
    /// Whether the user allows appodeal to collect ad data or not. If not the ads should still be played (at least on newer 
    /// versions), but without personalization. [Retrieved from/saved to an external file.]
    /// </summary>
    public AdDataCollectionPermitted CollectionOfDataConsent
    {
        get
        {
            FullVersionData data = RetrieveFullVersionDataFromFile();
            return data.CollectionOfDataConsent;
        }
        set
        {
            FullVersionData data = RetrieveFullVersionDataFromFile();
            data.CollectionOfDataConsent = value;
            SaveFullVersionDataToFile(data);
        }
    }

    /// <summary>
    /// Counter that counts how many blocks where collected since the last ad was shown. Determines when the next ad should be played.
    /// See 'AdManager' for criteria how often ads are played. [Retrieved from/saved to an external file.]
    /// </summary>
    public int ShowAdCounter
    { get
        {
            FullVersionData data = RetrieveFullVersionDataFromFile();
            return data.ShowAdCounter;
        }
        set
        {
            FullVersionData data = RetrieveFullVersionDataFromFile();
            data.ShowAdCounter = value;
            SaveFullVersionDataToFile(data);
        }
    }

    /// <summary>
    /// Counter that counts how many scenes were loaded since the last banner ad was shown. Determines when the next ad should be played.
    /// See 'AdManager' for criteria how often ads are played. [Retrieved from/saved to an external file.]
    /// </summary>
    public int ShowBannerAdCounter
    {
        get
        {
            FullVersionData data = RetrieveFullVersionDataFromFile();
            return data.ShowBannerAdCounter;
        }
        set
        {
            FullVersionData data = RetrieveFullVersionDataFromFile();
            data.ShowBannerAdCounter = value;
            SaveFullVersionDataToFile(data);
        }
    }

    /// <summary>
    /// How many interstitials were shown since the last video. [Retrieved from/saved to an external file.]
    /// </summary>
    public int InterstitialsShown
    {
        get
        {
            FullVersionData data = RetrieveFullVersionDataFromFile();
            return data.InterstitialsShown;
        }
        set
        {
            FullVersionData data = RetrieveFullVersionDataFromFile();
            data.InterstitialsShown = value;
            SaveFullVersionDataToFile(data);
        }
    }

    /// <summary>
    /// Returns an identifier that is unique for each device.
    /// </summary>
    public void GetDeviceUniqueIdentifier()
    {
        Debug.Log(SystemInfo.deviceUniqueIdentifier);
    }

    /// <summary>
    /// Only needed if IAPs are disabled. Stores the information of each customized color whether it was unlocked or is still locked. The information
    /// is stored in an external file.
    /// The index of the bool array identifies the color whose info is stored (1 = orange, 2 = dark green, 3 = faint red)
    /// </summary>
    public bool[] CustomColorUnlocked
    {
        get
        {
            FullVersionData data = RetrieveFullVersionDataFromFile();
            return data.CustomColorUnlocked;
        }
        set
        {
            FullVersionData data = RetrieveFullVersionDataFromFile();
            data.CustomColorUnlocked = value;
            SaveFullVersionDataToFile(data);
        }
    }

    /// <summary>
    /// This method retrieves the fullVersion data from an external file where it is saved.
    /// </summary>
    /// <returns>It returns an object of the type FullVersionData which holds all of the full-verion-specific data.</returns>
    public FullVersionData RetrieveFullVersionDataFromFile()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/dontDeleteOrAlter.dat"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/dontDeleteOrAlter.dat", FileMode.Open);
            FullVersionData data = (FullVersionData)bf.Deserialize(file);
            file.Close();

            return data;
        }
        else
        {
            Debug.Log("Full version data couldn't be retrieved from the external file. Check whether the persistentDataPath " +
                "addresses the right directory");
            FileStream file = File.Create(Application.persistentDataPath + "/dontDeleteOrAlter.dat");
            FullVersionData newFullVersionData = new FullVersionData();

            bf.Serialize(file, newFullVersionData);
            file.Close();

            return newFullVersionData;
        }
    }

    /// <summary>
    /// This method saves full version data to an external persistent file.
    /// </summary>
    /// <param name="toBeSaved">The object of the type FullVersionData (which holds the information 
    /// which should be saved) to pass.</param>
    public void SaveFullVersionDataToFile(FullVersionData toBeSaved)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/dontDeleteOrAlter.dat");

        bf.Serialize(file, toBeSaved);
        file.Close();
    }
}

/// <summary>
/// Whether the user gave their consent to personalize ads, denied it or didn't make a selection yet.
/// </summary>
public enum AdDataCollectionPermitted { notSet, permitted, denied };

public enum FullVersionUnlocked { notUnlocked, unlocked };
public enum GameFirstLoaded { firstLoaded, notFirstLoaded };

[Serializable]
public class FullVersionData
{
    public FullVersionData()
    {
        IsGameFirstLoaded = GameFirstLoaded.firstLoaded; //actually redundant, as the first defined value of an enum is assigned to it by default
        IsFullVersionUnlocked = FullVersionUnlocked.notUnlocked; //redundant, see comment above
        //deviceIdentifier = SystemInfo.deviceUniqueIdentifier; //device unique identifier
        CustomColorUnlocked = new bool[] { false, false, false };
    }

    private String deviceIdentifier;
    private FullVersionUnlocked unlockedState;

    /// <summary>
    /// Get or set whether the full version of the game was unlocked. When a new state is saved, the device unique identifier is also saved.
    /// When the current state is returned, true is only returned if the saved device identifier matches the identifier of the device.
    /// Theryby cheating is prevented.
    /// </summary>
    public FullVersionUnlocked IsFullVersionUnlocked
    {
        get
        {
            return SystemInfo.deviceUniqueIdentifier == deviceIdentifier ? unlockedState : FullVersionUnlocked.notUnlocked;
        }
        set
        {
            deviceIdentifier = SystemInfo.deviceUniqueIdentifier;
            unlockedState = value;
        }
    }

    /// <summary>
    /// Get or set whether the game was loaded for the first time.
    /// </summary>
    public GameFirstLoaded IsGameFirstLoaded
    { get; set; }

    /// <summary>
    /// Whether the user allows appodeal to collect ad data or not. If not the ads should still be played (at least on newer 
    /// versions), but without personalization.
    /// </summary>
    public AdDataCollectionPermitted CollectionOfDataConsent
    { get; set; }

    /// <summary>
    /// Counter that counts how many blocks where collected since the last ad was shown. Determines when the next ad should be played.
    /// See 'AdManager' for criteria how often ads are played.
    /// </summary>
    public int ShowAdCounter
    { get; set; }

    /// <summary>
    /// Counter that counts how many scenes were loaded since the last banner ad was shown. Determines when the next ad should be played.
    /// See 'AdManager' for criteria how often ads are played.
    /// </summary>
    public int ShowBannerAdCounter
    { get; set; }

    /// <summary>
    /// How many interstitials were shown since the last video.
    /// </summary>
    public int InterstitialsShown
    { get; set; }

    /// <summary>
    /// Only needed if IAPs are disabled. Stores the information of each customized color whether it was unlocked or is still locked.
    /// The index of the bool array identifies the color whose info is stored (1 = orange, 2 = dark green, 3 = faint red)
    /// </summary>
    public bool[] CustomColorUnlocked
    { get; set; }
}
