using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinsUIScalers : MonoBehaviour
{
    /// <summary>
    /// All UI objects that need a new scale (all UI object parents). These objects should have an x-position of 0.
    /// Only their x-scale is changed, they look kind of stretched afterwards.
    /// </summary>
    public GameObject[] UIObjectsNewScale;
    /// <summary>
    /// All UI objects that need a new position (because of the new screen aspect ratio), but no new scale.
    /// </summary>
    public GameObject[] UIObjectsNewPositionOldScale;
    /// <summary>
    /// The info panel game object. It is only rescaled if the screen ratio surpasses a certain 'stretching degree'
    /// </summary>
    public GameObject infoPanel;

    // Start is called before the first frame update
    void Start()
    {
        float uIWidthFactor = StaticValues.UIWidthFactor;

        foreach (GameObject g in UIObjectsNewScale)
        {
            Vector3 localScale = g.GetComponent<RectTransform>().localScale;
            g.GetComponent<RectTransform>().localScale = new Vector3(localScale.x * uIWidthFactor, localScale.y, localScale.z);
        }

        foreach(GameObject g in UIObjectsNewPositionOldScale)
        {
            Vector3 position = g.GetComponent<RectTransform>().localPosition;
            g.GetComponent<RectTransform>().localPosition = new Vector3(position.x * uIWidthFactor, position.y, position.z);
        }

        if (infoPanel != null)
        {
            Vector3 infoLocalScale = infoPanel.GetComponent<RectTransform>().localScale;
            if (uIWidthFactor < .9f)
                infoPanel.GetComponent<RectTransform>().localScale = new Vector3(infoLocalScale.x * .9f, infoLocalScale.y, infoLocalScale.z);
        }
    }
}
