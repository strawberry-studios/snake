using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablesController : MonoBehaviour
{
    private GameObject gameModeManager;
    /// <summary>
    /// Parents all of the objects that are actually visible during the gameplay.
    /// </summary>
    public GameObject actualGameComponents;

    // Start is called before the first frame update
    void Start()
    {
        gameModeManager = GameObject.FindGameObjectWithTag("GameModeManager");
        transform.SetParent(actualGameComponents.GetOrAddEmptyGameObject("collectablesHolder").transform);

        SetSize();
        SetColor();
    }

    void SetColor()
    {
        int[] c = DataSaver.Instance.GetCollectablesColor();
        Color thisColor = new Color(c[0]/255f, c[1]/255f, c[2]/255f, c[3]/255f);
        gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", thisColor);
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", thisColor);
    }

    /// <summary>
    /// The size of one of the blocks of the snake is set.
    /// It matches the size of a square in the gaming area.
    /// </summary>
    private void SetSize()
    {
        float cubeLength = gameModeManager.GetComponent<CreateWorld>().GetSquareLength();
        transform.localScale = new Vector3(cubeLength, 0.3f, cubeLength);
    } 
}
