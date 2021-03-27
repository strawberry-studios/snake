using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public GameObject worldBoundariesToggle, worldSize, speed, showPixelsToggle;
    private enum SliderCategory {worldSize, speed};
    // Start is called before the first frame update
    void Start()
    {
        SetToggleState(worldBoundariesToggle);
        SetShowPixelsToggleState(showPixelsToggle);
        SetSliderState(worldSize, SliderCategory.worldSize);
        SetSliderState(speed, SliderCategory.speed);
    }

    /// <summary>
    /// Sets the state (position) of the a slider in the scene.
    /// If The SliderCategory should corrsepond to the gameObject with the Slider component.
    /// The new position of the Slider depends on the value held by the PlayerPref.
    /// </summary>
    /// <param name="slider">slider as GameObject to pass</param>
    /// <param name="sliderCategory">sliderCategory as SliderCategory to pass</param>
    private void SetSliderState(GameObject slider, SliderCategory sliderCategory)
    {
        if(sliderCategory == SliderCategory.speed)
        {
            slider.GetComponent<Slider>().value = DataSaver.Instance.GetPlayerSpeed();
        }
        else if(sliderCategory == SliderCategory.worldSize)
        {
            slider.GetComponent<Slider>().value = DataSaver.Instance.GetWorldSize();
        }
    }

    /// <summary>
    /// Sets the state of the worldBoundairesAsMargins toggle-object in the scene.
    /// Depending on the value held by an external file the toggle is toggled on or not.
    /// </summary>
    /// <param name="toggle">The toggle-object as GameObject to pass</param>
    private void SetToggleState(GameObject toggle)
    {
        if(DataSaver.Instance.GetWorldBoundariesState())
        {
            toggle.GetComponent<Toggle>().isOn = true;
        }
        else
        {
            toggle.GetComponent<Toggle>().isOn = false;
        }
    }

    /// <summary>
    /// Sets the state of the ShowPixels toggle-object in the scene.
    /// Depending on the value held by an external file the toggle is toggled on or not.
    /// </summary>
    /// <param name="thisToggle">The toggle-object as GameObject to pass.</param>
    private void SetShowPixelsToggleState(GameObject thisToggle)
    {
        if(DataSaver.Instance.GetShowPixels())
        {
            thisToggle.GetComponent<Toggle>().isOn = true;
        }
        else
        {
            thisToggle.GetComponent<Toggle>().isOn = false;
        }
    }

    /// <summary>
    /// Sets the new speed of the snake
    /// </summary>
    /// <param name="speed">new snake speed to pass.</param>
    public void SetSnakeSpeed(float speed)
    {
        int speedAsInt;
        speedAsInt = Mathf.RoundToInt(speed);
        DataSaver.Instance.SavePlayerSpeed(speedAsInt);
    }

    /// <summary>
    /// Sets the new size of the world in which you play
    /// </summary>
    /// <param name="speed">new snake speed to pass.</param>
    public void SetWorldSize(float size)
    {
        int sizeAsInt;
        sizeAsInt = Mathf.RoundToInt(size);
        DataSaver.Instance.SaveNewWorldSize(sizeAsInt);
    }

    /// <summary>
    /// Determins whether the margins of the world are a boundary or not
    /// If true, the PlayerPref "worldBoundary" is set to 1, otherwise to 0 
    /// </summary>
    /// <param name="state">new world margins state to pass.</param>
    public void SetMarginsAsWorldBoundary(bool state)
    {
        DataSaver.Instance.SaveWorldBoundariesToggleState(state);
    }

    /// <summary>
    /// Sets the value of the showPixels variable and saves it to an external file.
    /// </summary>
    /// <param name="newState">The new showPixelsState as bool to pass.</param>
    public void SetShowPixels(bool newState)
    {
        DataSaver.Instance.SetShowPixels(newState);
    }
}
