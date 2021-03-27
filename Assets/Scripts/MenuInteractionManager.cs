using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInteractionManager : MonoBehaviour
{
    /// <summary>
    /// Loads the menu scene
    /// </summary>
    public void BackToMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// Exits the game
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Loads the instructions scene
    /// </summary>
    public void Instructions()
    {
        SceneManager.LoadScene("Instructions");
    }

    /// <summary>
    /// Loads the settings scene
    /// </summary>
    public void Settings()
    {
        SceneManager.LoadScene("Settings");
    }

    /// <summary>
    /// Loads the game
    /// </summary>
    public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// Returns 1 if passed true or -1 if passed false.
    /// </summary>
    /// <param name="myBool">Parameter value to pass.</param>
    /// <returns>Returns an integer based on the passed value.</returns>
    public bool FEW(bool myBool)
    {
        return myBool;
    }
}
