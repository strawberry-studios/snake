using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablesController : MonoBehaviour
{
    private GameObject gameModeManager;
    // Start is called before the first frame update
    void Start()
    {
        gameModeManager = GameObject.FindGameObjectWithTag("GameModeManager");
        SetSize();
    }

    /// <summary>
    /// The size of one of the blocks of the snake is set.
    /// It matches the size of a square in the gaming area.
    /// </summary>
    private void SetSize()
    {
        float cubeLength = gameModeManager.GetComponent<CreateWorld>().GetSquareLength();
        transform.localScale = new Vector3(cubeLength, 0.4f, cubeLength);
    } 
}
