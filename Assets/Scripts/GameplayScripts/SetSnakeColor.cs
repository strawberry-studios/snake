using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class only sets the color of the object it is attached to. The new color is the 'snakeColor'.
/// </summary>
public class SetSnakeColor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SetColor();
    }

    /// <summary>
    /// Sets the color of the object. The collectables color is retrieved from an external file.
    /// If this is attached to the snake head and snakeHeadMarked is on, the snake head is colored in a from-other-blocks-slightly-different color.
    /// </summary>
    void SetColor()
    {
        Color newColor;
        PlayerData data = DataSaver.Instance.RetrievePlayerDataFromFile();

        if(transform.parent.tag == "SnakeHead" && data.GetSnakeHeadMarked())
            newColor = data.GetSnakeHeadColor().ConvertIntArrayIntoColor();
        else
            newColor = data.GetSnakeColor().ConvertIntArrayIntoColor();
        GetComponent<Renderer>().material = Resources.Load("SnakeColor", typeof(Material)) as Material;
        GetComponent<Renderer>().material.SetColor("_EmissionColor", newColor);
        GetComponent<Renderer>().material.SetColor("_Color", newColor);
    }
}
