using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// References snake logo components.
/// </summary>
[Serializable]
public class SnakeLogoComponents
{
    public Image snakeBody, snakeDot, appleDot;

    /// <summary>
    /// Sets the color of the snake logo. It is colored in the snake color, apple color and in the snake-highlighting color (the color with which
    /// the snake head can be highlighted)
    /// </summary>
    public void SetColorOfSnakeLogo()
    {
        PlayerData currentData = DataSaver.Instance.RetrievePlayerDataFromFile();
        snakeBody.color = currentData.GetSnakeColor().ConvertIntArrayIntoColor();
        snakeDot.color = currentData.GetSnakeHeadColor().ConvertIntArrayIntoColor();
        appleDot.color = currentData.GetCollectablesColor().ConvertIntArrayIntoColor();
    }
}

/// <summary>
/// Manages the intro theme and how it is played.
/// Also manages all things that have to be done exactly once when the game is (re)loaded. 
/// Also serves as a kind of Menu Manager. #MultiTasking
/// </summary>
public class IntroTheme : MonoBehaviour
{
    /// <summary>
    /// The parent that contains all objects that constitute the logo.
    /// </summary>
    public GameObject uiThemeHolder;
    /// <summary>
    /// The parent of the intro theme objects.
    /// </summary>
    public GameObject introTheme;
    /// <summary>
    /// Object which is created once whenever the game is loaded and doesn't get deleted beginning from then (until the game is exited).
    /// </summary>
    GameObject gameLoaded;
    /// <summary>
    /// The components of the snake logo.
    /// </summary>
    public SnakeLogoComponents snakeLogoComponents;
    

    private void Awake()
    {
        gameLoaded = GameObject.FindGameObjectWithTag("GameLoadedManager");
        if (gameLoaded != null)
            introTheme.SetActive(false);
        else
        {
            gameLoaded = new GameObject("gameLoadedManager");
            gameLoaded.tag = "GameLoadedManager";

            SetUIWidthFactor();

            //a coroutinesSingleton is created to avoid errors when a method on this script is called during 'OnDestroy'
            CoroutinesSingleton.Instance.Create();

            DontDestroyOnLoad(gameLoaded);
            StartCoroutine(PlayIntroTheme(1500)); //the intro theme is shown for 1.5 seconds (is more in reality because the method calls also 
                                                  //need some time
        }
    }

    private void Start()
    {
        snakeLogoComponents.SetColorOfSnakeLogo();
    }

    /// <summary>
    /// This method sets the UI width factor. The UIs were concepted for a screen aspect ratio of 16:9, if the actual aspect ratio is different,
    /// the width (x-scale) of all UIs is modified (multiplied by this factor). This method is only called once per loading of the game (at the very
    /// beginning).
    /// </summary>
    void SetUIWidthFactor()                         
    {
        //the default aspect ratio is 16:9, if the aspect ratio of a device is different, the UIs which were created for the ratio 16:9 must be 
        //modified: this is the factor the x-scale of the UIs needs to be multiplied by:
        float actualAspectRatio = Camera.main.aspect;
        float defaultAspectRatio = 9 / 16f;
        float factor = actualAspectRatio / defaultAspectRatio;
        StaticValues.UIWidthFactor = factor;
        print(factor);
    }

    /// <summary>
    /// The intro theme fades in and out within the passed time.
    /// </summary>
    /// <param name="durationInMillis">The time within which the intro theme fades in and out.</param>
    /// <returns></returns>
    IEnumerator PlayIntroTheme(float durationInMillis)
    {
        introTheme.SetActive(true);
        uiThemeHolder.SetNewAlphaForObjectAndChildren(0);
        int currentAlphaValue = 0; //the current alpha value of the object with changing transparency

        //time after which the alpha value of the objects with changing transparency is altered by onfloat timeStep; 
        float timeStep = durationInMillis / (102*1.1f); //(102*1.1) instead of 102 (number of alpha-value-alterations) because executing the method always 
        //requires so much time that the duration of the coroutine is eventually similar to the 'durationInMillis'

        //increase transparency:

        while(currentAlphaValue < 255)
        {
            yield return new WaitForSecondsRealtime(timeStep / 1000);
            currentAlphaValue += 5;
            uiThemeHolder.SetNewAlphaForObjectAndChildren(currentAlphaValue);
        }

        //decrease transparency again:

        while (currentAlphaValue > 0)
        {
            yield return new WaitForSecondsRealtime(timeStep / 1000);
            currentAlphaValue -= 5;
            uiThemeHolder.SetNewAlphaForObjectAndChildren(currentAlphaValue);
        }

        introTheme.SetActive(false);
    }
}
