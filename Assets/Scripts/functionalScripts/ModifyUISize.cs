using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script should be attached to the manager object of each scene. It should reference all UI objects of the scene. Their width and x-position
/// is adjusted to the size of the device on which the game is run.
/// </summary>
public class ModifyUISize : MonoBehaviour
{
    /// <summary>
    /// All UI objects that need a new scale (all UI object parents). These objects should have an x-position of 0.
    /// </summary>
    public GameObject[] UIObjectsNewScale;
    ///// <summary>
    ///// All UI objects that need a new position (all UI object children).
    ///// </summary>
    //public GameObject[] UIObjectsNewPosition;


    // Start is called before the first frame update
    void Start()
    {
        float uIWidthFactor = StaticValues.UIWidthFactor;

        foreach (GameObject g in UIObjectsNewScale)
        {
            Vector3 localScale = g.GetComponent<RectTransform>().localScale;
            g.GetComponent<RectTransform>().localScale = new Vector3(localScale.x * uIWidthFactor, localScale.y, localScale.z);
        }
        //foreach (GameObject g in UIObjectsNewPosition)
        //{
        //    Vector3 position = g.GetComponent<RectTransform>().position;
        //    g.GetComponent<RectTransform>().position = new Vector3(position.x * uIWidthFactor, position.y, position.z);
        //}
    }
}
