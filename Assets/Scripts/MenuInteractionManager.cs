using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuInteractionManager : MonoBehaviour
{

    private void Update()
    {
        //check if user wants to go back to the 'more options' scene
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
    }


    //to be attached to buttons:

    /// <summary>
    /// Loads the options scene which was opened at last. If none can be found (if the data wasn't saved), the difficulty scene is opened.
    /// </summary>
    public void Options()
    {
        switch(SceneInteraction.Instance.OptionsSceneLastOpened)
        {
            case OptionsSceneLastOpened.difficulty:
                SceneManager.LoadScene("Difficulty");
                break;
            case OptionsSceneLastOpened.preferences:
                SceneManager.LoadScene("Preferences");
                break;
        }
    }

    /// <summary>
    /// Loads the graphics scene which was opened at last. If none can be found (if the data wasn't saved), the graphics settings scene is opened.
    /// </summary>
    public void OpenLastGraphicsScene()
    {
        switch (SceneInteraction.Instance.GraphicsSceneLastOpened)
        {
            case GraphicsSceneLastOpened.graphicsSettings:
                SceneManager.LoadScene("GraphicsSettings");
                break;
            case GraphicsSceneLastOpened.snakeColor:
                SceneManager.LoadScene("Skins_Snake_Color");
                break;
            case GraphicsSceneLastOpened.collectablesColor:
                SceneManager.LoadScene("Skins_Collectables_Color");
                break;
        }
    }

    /// <summary>
    /// Opens the 'moreOptions' scene.
    /// </summary>
    public void OpenMoreOptionsScene()
    {
        SceneManager.LoadScene("MoreOptions");
    }

    /// <summary>
    /// Loads the game.
    /// </summary>
    public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// Loads the highScores scene.
    /// </summary>
    public void HighScores()
    {
        SceneManager.LoadScene("HighScores");
    }

    /// <summary>
    /// Opens the scene where the full version of the game can be purchased.
    /// </summary>
    public void OpenFullVersionPurchaseTab()
    {
        SceneManager.LoadScene("PurchaseFullVersion");
    }

}
