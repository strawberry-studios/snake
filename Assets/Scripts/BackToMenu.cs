using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Provides method(s) to return to the menu. And also to load a new scene.
/// </summary>
public class BackToMenu : MonoBehaviour
{
    /// <summary>
    /// Loads the menu scene. (And closes the currently opened (one).)
    /// </summary>
    public void ReturnToMenu()
    {
        Time.timeScale = 1.0f; //makes sure that the timeScale is set to 1 (means: real time)
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// Loads a new scene and closes the current one.
    /// </summary>
    /// <param name="sceneBuildNumber">The build number of the new scene which should be loaded.</param>
    public void LoadNewScene(int sceneBuildNumber)
    {
        SceneManager.LoadScene(sceneBuildNumber);
    }
}
