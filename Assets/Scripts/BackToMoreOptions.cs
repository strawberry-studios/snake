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
}
