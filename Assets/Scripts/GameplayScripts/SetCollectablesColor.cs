using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class only sets the color of the object it is attached to. The new color is the 'collectablesColor'.
/// </summary>
public class SetCollectablesColor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SetColor();
    }

    /// <summary>
    /// Sets the color of the object. The collectables color is retrieved from an external file.
    /// </summary>
    void SetColor()
    {
        Color newColor = DataSaver.Instance.GetCollectablesColor().ConvertIntArrayIntoColor();
        GetComponent<Renderer>().material = Resources.Load("CollectableColor", typeof(Material)) as Material;
        GetComponent<Renderer>().material.SetColor("_EmissionColor", newColor);
        GetComponent<Renderer>().material.SetColor("_Color", newColor);
    }
}
