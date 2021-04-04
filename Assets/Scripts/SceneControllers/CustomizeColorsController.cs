using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CustomizeColorsController : MonoBehaviour
{
    /// <summary>
    /// A class referencing all objects of the customize-colors panel and providing the necessary methods to interact.
    /// </summary>
    [System.Serializable]
    public class CustomPanel
    {
        /// <summary>
        /// The panel where a color can be customized.
        /// </summary>
        public GameObject panel;
        /// <summary>
        /// A button symbolizing the previous color (which will now be altered).
        /// </summary>
        public Button previousColor;
        /// <summary>
        /// An image object symbolizing the current color.
        /// </summary>
        public Image currentColor;
        /// <summary>
        /// A slider determining one of the RGB components of the new color.
        /// </summary>
        public Slider redSlider, blueSlider, greenSlider;
    }

    /// <summary>
    /// Whether the pixel mode is on or not.
    /// </summary>
    bool pixelMode;
    /// <summary>
    /// The three customizable colors.
    /// </summary>
    Color[] customizableColors;
    /// <summary>
    /// The images in the scene which represent the customizable colors.
    /// </summary>
    public Image[] colorsImages;
    /// <summary>
    /// The pixeled images in the scene which represent the customizable colors.
    /// </summary>
    public GameObject[] colorsImagesPixeled;
    /// <summary>
    /// The image in the scene which represents the snake color.
    /// </summary>
    public Image snakeImage;
    /// <summary>
    /// The pixeled image in the scene which represents the snake color.
    /// </summary>
    public GameObject snakeImagePixeled;
    /// <summary>
    /// The image in the scene which represents the collectables (apple) color.
    /// </summary>
    public Image collectablesImage;
    /// <summary>
    /// The pixeled image in the scene which represents the collectables (apple) color.
    /// </summary>
    public GameObject collectablesImagePixeled;
    /// <summary>
    /// The player data. (Information about the preferences of the player retrieved from an external file.)
    /// </summary>
    PlayerData data;
    /// <summary>
    /// Object providing all fields and methods for interacting with the customizable-color panel.
    /// </summary>
    public CustomPanel customPanel;
    /// <summary>
    /// The index of the color that should be altered.
    /// </summary>
    int colorToBeAltered;
    /// <summary>
    /// The color (that is customized) which will be overriden if the alteration is saved.
    /// </summary>
    Color oldColor;
    /// <summary>
    /// The color of the apple/snake.
    /// </summary>
    Color snakeColor, appleColor;

    // Start is called before the first frame update
    void Start()
    {
        //PlayerProgress.Instance.IsFullVersionUnlocked = FullVersionUnlocked.unlocked;
        data = DataSaver.Instance.RetrievePlayerDataFromFile();
        customPanel.panel.SetActive(false);
        pixelMode = data.GetShowPixels();
        snakeColor = data.GetSnakeColor().ConvertIntArrayIntoColor();
        appleColor = data.GetCollectablesColor().ConvertIntArrayIntoColor();
        ToggleRedundantObjectsInactive();
        LoadCustomizableColors();
        ColorImagesInScene();
        LoadSnakeAndAppleColor();
    }

    private void Update()
    {
        //check if user wants to go back to the last scene
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Back();
        }
    }


    /// <summary>
    /// Toggles either all pixeled or all non-pixeled objects inactive depending on it whether the pixelMode is activated.
    /// (If pixelMode is activated all pixeled objects will remain visible, all others are deactivated; vice versa.)
    /// </summary>
    void ToggleRedundantObjectsInactive()
    {
        foreach (Image i in colorsImages)
            i.enabled = !pixelMode;
        foreach (GameObject g in colorsImagesPixeled)
        {
            g.SetActive(pixelMode);
        }
        snakeImage.enabled = !pixelMode;
        snakeImagePixeled.SetActive(pixelMode);
        collectablesImage.enabled = !pixelMode;
        collectablesImagePixeled.SetActive(pixelMode);
    }

    /// <summary>
    /// Loads the three customizable colors from an external file.
    /// </summary>
    void LoadCustomizableColors()
    {
        customizableColors = new Color[3];
        for (int i = 0; i < 3; i++)
        {
            int[] color = new int[4];
            for (int j = 0; j < 4; j++)
                color[j] = data.ColorsCustomized[i, j];
            customizableColors[i] = color.ConvertIntArrayIntoColor();
        }
    }

    /// <summary>
    /// Colors the images in the scene which represent the three customizable colors.
    /// </summary>
    void ColorImagesInScene()
    {
        for (int i = 0; i < 3; i++)
        {
            if (pixelMode)
            {
                foreach (Image p in colorsImagesPixeled[i].GetComponentsInChildren<Image>())
                    p.color = customizableColors[i];
            }
            else
                colorsImages[i].color = customizableColors[i];
        }
    }

    /// <summary>
    /// Loads the snake and apple (collectable) color from an external file. The image objects representing the snake and apple color are colored.
    /// </summary>
    void LoadSnakeAndAppleColor()
    {
        if (pixelMode)
        {
            print("do");
            foreach (Image i in snakeImagePixeled.GetComponentsInChildren<Image>())
                i.color = snakeColor;
            foreach(Image r in collectablesImagePixeled.GetComponentsInChildren<Image>())
                r.color = appleColor;
        }
        else
        {
            snakeImage.color = snakeColor;
            collectablesImage.color = appleColor;
        }
    }

    //methods for customizing colors - need to be attached to buttons

    /// <summary>
    /// Opens the panel where the selected color can be altered. The panel is set up. (The color which will be altered is loaded as previous color, etc.)
    /// </summary>
    /// <param name="i">The index of the color that should be altered. (Ranging from 0 to 2, if not this method doesn't work)</param>
    public void AlterColor(int i)
    {
        customPanel.panel.SetActive(true);
        colorToBeAltered = i;
        customPanel.previousColor.GetComponent<Image>().color = customizableColors[colorToBeAltered];
        customPanel.currentColor.color = customizableColors[colorToBeAltered];
        oldColor = customizableColors[colorToBeAltered];
        LoadSliderValues(colorToBeAltered);
    }

    /// <summary>
    /// Loads the position of the r,g,b sliders. The slider-values are taken from one of the customizable colors.
    /// </summary>
    /// <param name="colorIndex">The index of the color to be loaded. (0 = color1, 1 = color2, 2 = color3)</param>
    void LoadSliderValues(int colorIndex)
    {
        float[] colorAsFloatArray = customizableColors[colorToBeAltered].ConvertColorToFloatArray();
        customPanel.redSlider.value = colorAsFloatArray[0];
        customPanel.greenSlider.value = colorAsFloatArray[1];
        customPanel.blueSlider.value = colorAsFloatArray[2];
    }

    /// <summary>
    /// The customize-color panel is closed. The changes aren't saved, the newly customized color is dismissed.
    /// </summary>
    public void DismissCustomizedColor()
    {
        customizableColors[colorToBeAltered] = oldColor;
        CloseCustomizeColorsPanel();
    }

    /// <summary>
    /// Saves the newly customized color to an external file and closes the customize-color panel.
    /// Also saves the new snake/apple color to an external file if the color which was just altered was assigned to either of them.
    /// </summary>
    public void SaveCustomizedColor()
    {
        PlayerData newData = DataSaver.Instance.RetrievePlayerDataFromFile();
        Color newColor = new Color(customPanel.redSlider.value, customPanel.greenSlider.value,
            customPanel.blueSlider.value , 1);
        SaveSnakeAppleColor(newData, newColor);
        newData.ColorsCustomized[colorToBeAltered, 0] = (int)(newColor.r * 255);
        newData.ColorsCustomized[colorToBeAltered, 1] = (int)(newColor.g * 255);
        newData.ColorsCustomized[colorToBeAltered, 2] = (int)(newColor.b * 255);
        newData.ColorsCustomized[colorToBeAltered, 3] = 255;
        DataSaver.Instance.SavePlayerDataToFile(newData);
        ColorImagesInScene();
        CloseCustomizeColorsPanel();
    }

    /// <summary>
    /// Saves a new snake or apple color if the color which was just altered also was the current snake/apple color.
    /// The altered 'PlayerData' object is returned.
    /// </summary>
    /// <param name="thisData">The 'PlayerData' where the old snake/apple color is saved.</param>
    /// <param name="newColor">The new color after the alteration.</param>
    PlayerData SaveSnakeAppleColor(PlayerData thisData, Color newColor)
    {
        Color colorBeforeAlteration = thisData.ColorsCustomized.Convert2DIntArrayToColorArray()[colorToBeAltered];

        if(colorBeforeAlteration.IsColorEqualRGB(snakeColor))
        { 
            snakeColor = newColor;
            thisData.SetSnakeColor(snakeColor);
            LoadSnakeAndAppleColor();
        }
        else if(colorBeforeAlteration.IsColorEqualRGB(appleColor))
        {
            appleColor = newColor;
            thisData.SetCollectablesColor(appleColor);
            LoadSnakeAndAppleColor();
        }
        return thisData;
    }

    /// <summary>
    /// Reloads the old color and dismisses the changes which were made so far.
    /// </summary>
    public void ReloadOldColor()
    {
        //int[] oldColor = new int[4] ;
        //PlayerData oldData = DataSaver.Instance.RetrievePlayerDataFromFile();
        //for (int i = 0; i < 4; i++)
        //    oldColor[i] = oldData.ColorsCustomized[colorToBeAltered, i];
        customPanel.currentColor.color = oldColor;//oldColor.ConvertIntArrayIntoColor();
        customizableColors[colorToBeAltered] = oldColor;
        LoadSliderValues(colorToBeAltered);
    }

    /// <summary>
    /// The customize-colors panel is closed.
    /// </summary>
    void CloseCustomizeColorsPanel()
    {
        customPanel.panel.SetActive(false);
    }

    //methods for altering the colorToBeAltered - need to be attached to sliders:

    /// <summary>
    /// Loads a new red-value for the color which is currently being customized.
    /// </summary>
    /// <param name="newValue">The new red-value of the color as float (from 0 to 1).</param>
    public void ChangeRedComponent(float newValue)
    {
        customizableColors[colorToBeAltered] = new Color(newValue, customizableColors[colorToBeAltered].g, customizableColors[colorToBeAltered].b, 1);
        LoadNewColor();
    }

    /// <summary>
    /// Loads a new green-value for the color which is currently being customized.
    /// </summary>
    /// <param name="newValue">The new green-value of the color as float (from 0 to 1).</param>
    public void ChangeGreenComponent(float newValue)
    {
        customizableColors[colorToBeAltered] = new Color(customizableColors[colorToBeAltered].r, newValue, customizableColors[colorToBeAltered].b, 1);
        LoadNewColor();
    }

    /// <summary>
    /// Loads a new blue-value for the color which is currently being customized.
    /// </summary>
    /// <param name="newValue">The new blue-value of the color as float (from 0 to 1).</param>
    public void ChangeBlueComponent(float newValue)
    {
        customizableColors[colorToBeAltered] = new Color(customizableColors[colorToBeAltered].r, customizableColors[colorToBeAltered].g, newValue, 1);
        LoadNewColor();
    }

    /// <summary>
    /// Loads a new color to the image which represents the newly customized color. 
    /// </summary>
    void LoadNewColor()
    {
        customPanel.currentColor.color = customizableColors[colorToBeAltered];
    }

    //methods for scene interaction:

    /// <summary>
    /// Opens the scene from which the customizing of the colors was started. (Either the snake or collectables color scene.)
    /// </summary>
    public void Back()
    {
        if (DataSaver.Instance.LastCustomizedColor == LastEditedColor.snake)
            SceneManager.LoadScene("Skins_Snake_Color");
        else
            SceneManager.LoadScene("Skins_Collectables_Color");
    }
}
