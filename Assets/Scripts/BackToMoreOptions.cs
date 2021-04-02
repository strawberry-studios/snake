using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMoreOptions : MonoBehaviour
{
    /// <summary>
    /// Loads the more options scene.
    /// </summary>
    public void LoadMoreOptionsScene()
    {
        SceneManager.LoadScene("MoreOptions");
    }

    private void Update()
    {
        //check if user wants to go back to the 'more options' scene
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            LoadMoreOptionsScene();
        }
    }
}
