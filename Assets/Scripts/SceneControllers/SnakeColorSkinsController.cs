using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public static class ColorExtensions
{
    /// <summary>
    /// Retruns true, if the color that executes the method has the same RGBA values as the color it is compared to. (rounded)
    /// It also returns true, when the colors are very similar (not possible to recognise the difference at first sight).
    /// </summary>
    /// <param name="c">The color which calls the object.</param>
    /// <param name="colorComparison">The color you compare the calling color to.</param>
    /// <returns></returns>
    public static bool IsColorEqualRGBA(this Color c, Color colorComparison)
    {
        int cR = (int)(c.r * 255);
        int cG = (int)(c.g * 255);
        int cB = (int)(c.b * 255);
        int cA = (int)(c.a * 255);
        int comparisonR = (int)(colorComparison.r * 255);
        int comparisonG = (int)(colorComparison.g * 255);
        int comparisonB = (int)(colorComparison.b * 255);
        int comparisonA = (int)(colorComparison.a * 255);
        return (Mathf.Abs(cR-comparisonR) <= 1 && Mathf.Abs(cG-comparisonG) <= 1 && Mathf.Abs(cB-comparisonB) <= 1 
            && Mathf.Abs(cA-comparisonA) <= 1);
    }

    /// <summary>
    /// Retruns true, if the color that executes the method has the same RGB values as the color it is compared to. (rounded)
    /// It also returns true, when the colors are very similar (not possible to recognise the difference at first sight). The transparency 
    /// (a-value of a color) of the colors might be different.
    /// </summary>
    /// <param name="c">The color which calls the object.</param>
    /// <param name="colorComparison">The color you compare the calling color to.</param>
    /// <returns></returns>
    public static bool IsColorEqualRGB(this Color c, Color colorComparison)
    {
        int cR = (int)(c.r * 255);
        int cG = (int)(c.g * 255);
        int cB = (int)(c.b * 255);
        int comparisonR = (int)(colorComparison.r * 255);
        int comparisonG = (int)(colorComparison.g * 255);
        int comparisonB = (int)(colorComparison.b * 255);
        return (Mathf.Abs(cR - comparisonR) <= 1 && Mathf.Abs(cG - comparisonG) <= 1 && Mathf.Abs(cB - comparisonB) <= 1);
    }

    /// <summary>
    /// Debugs the RGBA values of the color that calls this method. The values are printed in the console.
    /// </summary>
    /// <param name="c"></param>
    public static void PrintRGBAValues(this Color c)
    {
        Debug.Log((int)(c.r * 255));
        Debug.Log((int)(c.g * 255));
        Debug.Log((int)(c.b * 255));
        Debug.Log((int)(c.a * 255));
    }

    /// <summary>
    /// Sets the transparency of the calling color to the passed int value. The RGB values remain the same.
    /// </summary>
    /// <param name="newAlpha">The new alpha value of the color (ranging from 0 to 255).</param>
    /// <returns>Returns the new color (same RGB-values, new alpha-value).</returns>
    public static Color GetColorWithNewA(this Color c, int newAlpha)
    {
        return new Color(c.r, c.g, c.b, newAlpha / 255f);
    }

    /// <summary>
    /// Sets the transparency of the calling color to the passed int value. The RGB values remain the same.
    /// </summary>
    /// <param name="newAlpha">The new alpha value of the color (ranging from 0 to 1).</param>
    /// <returns>Returns the new color (same RGB-values, new alpha-value).</returns>
    public static Color GetColorWithNewA(this Color c, float newAlpha)
    {
        return new Color(c.r, c.g, c.b, newAlpha);
    }

    /// <summary>
    /// Converts a color into an int array of the length four (RGBA representation). Index 0 equals the r component of the color, 1 equals g, 2 equals b and 3 
    /// equals a (alpha). The values of the fields of the returned array correspond to the RGBA components of the color (ranging from 0 to 255).
    /// </summary>
    /// <param name="c"></param>
    /// <returns>Returns the array which the color was converted into.</returns>
    public static int[] ConvertColorToIntArray(this Color c)
    {
        int[] array = new int[4];
        array[0] = (int)(c.r * 255);
        array[1] = (int)(c.g * 255);
        array[2] = (int)(c.b * 255);
        array[3] = (int)(c.a * 255);
        return array;
    }

    /// <summary>
    /// Converts a 2 dimensional array into a colors array. The second dimension of the passed array must have a length of 4.
    /// Index 0 is the r-, 1 the g-, 2 the b- and 3 the alpha component of a new color.
    /// </summary>
    /// <param name="array">The array for which this method is applied as 2D int array.</param>
    /// <returns></returns>
    public static Color[] Convert2DIntArrayToColorArray(this int[,] array)
    {
        int length = array.GetLength(0);
        Color[] newColorArray = new Color[length];
        int[] newIntArray = new int[4];
        for(int i = 0; i < length; i++)
        {
            for (int j = 0; j < 4; j++)
                newIntArray[j] = array[i, j];
            newColorArray[i] = newIntArray.ConvertIntArrayIntoColor();
        }
        return newColorArray;
    }

    /// <summary>
    /// Converts a color into an int array of the length four (RGBA representation). Index 0 equals the r component of the color, 1 equals g, 2 equals b and 3 
    /// equals a (alpha). The values of the fields of the returned array correspond to the RGBA components of the color (ranging from 0 to 1).
    /// </summary>
    /// <param name="c"></param>
    /// <returns>Returns the array which the color was converted into.</returns>
    public static float[] ConvertColorToFloatArray(this Color c)
    {
        float[] array = new float[4];
        array[0] = c.r;
        array[1] = c.g;
        array[2] = c.b;
        array[3] = c.a;
        return array;
    }
}

public static class IntegerExtensions
{
    /// <summary>
    /// Converts an int-array into a color (if it has the length 4). The ints of the array become the r, g, b and a values of the new color
    /// (ranging from 0 to 255 at a time).
    /// </summary>
    /// <param name="array"></param>
    /// <returns>Returns the newly created color. If the conversion of the array isn't possible (length of the array unequal to 4), 
    /// Color.white is returned.</returns>
    public static Color ConvertIntArrayIntoColor(this int[] array)
    {
        if (array.Length != 4)
            return Color.white;
        return new Color(array[0]/255f, array[1]/255f, array[2]/255f, array[3]/255f);
    }
}

/// <summary>
/// This class provides methods which are needed for the UI where the users can set the 'snake color'.
/// It also laods the current settings (in the scene).
/// </summary>
public class SnakeColorSkinsController : MonoBehaviour
{
    /// <summary>
    /// References all Buttons in the scene that represent one color the snake can have.
    /// </summary>
    [System.Serializable]
    public class Colors
    {
        public GameObject green, red, blue;
        public GameObject yellow, violet, turquoise;
        public GameObject orange, darkGreen, faintRed;

        /// <summary>
        /// Returns the game object with the passed color. If none of the objects has this color the green object is returned.
        /// </summary>
        /// <param name="colorOfTheObject">The wanted color as Color.</param>
        /// <returns></returns>
        public GameObject ChooseObjectWithColor(Color colorOfTheObject)
        {
            Color[] c = new Color[9];
            //cGreen, cRed, cBlue, cYellow, cViolet, cTurqoise, cOrange, cDarkGreen, cFaintRed; //the colors of the color objects
            c[0] = green.GetComponent<Image>().color;
            c[1] = red.GetComponent<Image>().color;
            c[2] = blue.GetComponent<Image>().color;
            c[3] = yellow.GetComponent<Image>().color;
            c[4] = violet.GetComponent<Image>().color;
            c[5] = turquoise.GetComponent<Image>().color;
            c[6] = orange.GetComponent<Image>().color;
            c[7] = darkGreen.GetComponent<Image>().color;
            c[8] = faintRed.GetComponent<Image>().color;

            for (int i = 0; i < 9; i++)
            {
                if (c[i].IsColorEqualRGB(colorOfTheObject))
                {
                    switch (i)
                    {
                        case 0:
                            return green;
                        case 1:
                            return red;
                        case 2:
                            return blue;
                        case 3:
                            return yellow;
                        case 4:
                            return violet;
                        case 5:
                            return turquoise;
                        case 6:
                            return orange;
                        case 7:
                            return darkGreen;
                        case 8:
                            return faintRed;
                        default:
                            return green; //green is returned by default
                    }
                }
            }
            return green; //green is returned by default
        }

        /// <summary>
        /// Sets a new transparency for all objects referenced by this class. The RGB values of the colors of the objects remain the same.
        /// </summary>
        /// <param name="newAlpha">The new transparency as alpha-value (max = 255, min = 0).</param>
        public void SetNewTransparency(int newAlpha)
        {
            green.GetComponent<Image>().color = green.GetComponent<Image>().color.GetColorWithNewA(newAlpha);
            red.GetComponent<Image>().color = red.GetComponent<Image>().color.GetColorWithNewA(newAlpha);
            blue.GetComponent<Image>().color = blue.GetComponent<Image>().color.GetColorWithNewA(newAlpha);
            yellow.GetComponent<Image>().color = yellow.GetComponent<Image>().color.GetColorWithNewA(newAlpha);
            violet.GetComponent<Image>().color = violet.GetComponent<Image>().color.GetColorWithNewA(newAlpha);
            turquoise.GetComponent<Image>().color = turquoise.GetComponent<Image>().color.GetColorWithNewA(newAlpha);
            orange.GetComponent<Image>().color = orange.GetComponent<Image>().color.GetColorWithNewA(newAlpha);
            darkGreen.GetComponent<Image>().color = darkGreen.GetComponent<Image>().color.GetColorWithNewA(newAlpha);
            faintRed.GetComponent<Image>().color = faintRed.GetComponent<Image>().color.GetColorWithNewA(newAlpha);
        }

        /// <summary>
        /// Loads the customizable colors from an external file, if the full version was unlocked.
        /// </summary>
        public void LoadCustomizedColors()
        {
            if (FullVersion.Instance.IsFullVersionUnlocked == FullVersionUnlocked.unlocked)
            {
                Color[] customColors = DataSaver.Instance.ColorsCustomized.Convert2DIntArrayToColorArray();
                orange.GetComponent<Image>().color = customColors[0];
                darkGreen.GetComponent<Image>().color = customColors[1];
                faintRed.GetComponent<Image>().color = customColors[2];
            }
        }
    }

    /// <summary>
    /// This class references the three pixeled colors that can be customized and provides a method for loading their color from an external file.
    /// </summary>
    [System.Serializable]
    public class PixeledColors
    {
        public GameObject orange, darkGreen, faintRed;

        /// <summary>
        /// Loads the customizable colors from an external file, if the full version was unlocked.
        /// </summary>
        public void LoadCustomizedColors()
        {
            if (FullVersion.Instance.IsFullVersionUnlocked == FullVersionUnlocked.unlocked)
            {
                Color[] customColors = DataSaver.Instance.ColorsCustomized.Convert2DIntArrayToColorArray();
                foreach (Image g in orange.GetComponentsInChildren<Image>())
                    g.GetComponent<Image>().color = customColors[0];
                foreach (Image h in darkGreen.GetComponentsInChildren<Image>())
                    h.GetComponent<Image>().color = customColors[1];
                foreach (Image i in faintRed.GetComponentsInChildren<Image>())
                    i.GetComponent<Image>().color = customColors[2];
            }
        }
    }

    /// <summary>
    /// This class references all objects that are needed for marking the current color of the collactables.
    /// </summary>
    [System.Serializable]
    public class CurrentCollectablesColor
    {
        public GameObject parent; //the object which contains all elements for marking the collectables color
        public GameObject crossPixeled; //a cross which marks the current color of the collectables (pixeled)
        public GameObject crossUnpixeled; //a cross which marks the current color of the collectables (unpixeled)
    }

    /// <summary>
    /// This class references three public lock objects. (They mark the customizable colors which will only be accessible after purchasing the 
    /// full version.)
    /// </summary>
    [System.Serializable]
    public class Locks
    {
        /// <summary>
        /// One of the lock objects.
        /// </summary>
        public GameObject lock1, lock2, lock3;

        /// <summary>
        /// If the full version wasn't unlocked yet, all of the lock objects are set active and their positions are set to those of the 
        /// passed transforms.
        /// </summary>
        /// <param name="color1Transform">The transform of one of the colors that requires unlocking.</param>
        /// <param name="color2Transform">The transform of the second color that requires unlocking.</param>
        /// <param name="color3Transform">The transform of the third color that requires unlocking.</param>
        public void SetPositionOfLockObjects(Transform color1Transform, Transform color2Transform, Transform color3Transform)
        {
            if(FullVersion.Instance.IsFullVersionUnlocked == FullVersionUnlocked.notUnlocked)
            {
                ToggleLocksActive(true);
                lock1.transform.position = color1Transform.position;
                lock2.transform.position = color2Transform.position;
                lock3.transform.position = color3Transform.position;
            }
            else
            {
                ToggleLocksActive(false);
            }
        }

        /// <summary>
        /// Toggles all of the lock-objects (in)active.
        /// </summary>
        /// <param name="state">Whether the lock objects should be active or not.</param>
        private void ToggleLocksActive(bool state)
        {
            lock1.SetActive(state);
            lock2.SetActive(state);
            lock3.SetActive(state);
        }
    }

    /// <summary>
    /// The time until an info panel (UI image and text objects and their children) is closed.
    /// </summary>
    int timeUntilClosureOfInfoPanel;
    /// <summary>
    /// The time within which an info panel (UI image and text objects and their children) fades and fully disappears.
    /// </summary>
    int fadingTimeInfoPanel;
    bool pixelsOn; //whether the pixel mode is activated or not
    public CurrentCollectablesColor currentCollectablesColor; //objects for marking the current color of the collectables
    public GameObject colorSelector; //the frame that is displayed around the currently selected color
    public GameObject selectedColorsPixeled; //the object which shows the currently selected color, pixeled presentation
    public Colors colors; // the colors in the scene (that can be selected by the player) 
    /// <summary>
    /// The pixeled, customizable colors.
    /// </summary>
    public PixeledColors pixeledColors; 
    public GameObject colorsPixeled; //the colors in the scene (that can be selected by the player; pixeled)
    public GameObject selectedColor; //the object which shows the currently selected color, unpixeled presentation
    public GameObject infoPanel; //an info panel informing the player that they can't select the current collectables color
    public Locks locks; //the locks locking the customizable colors (if not unlocked via the full version yet)
    /// <summary>
    /// The text of the info panel.
    /// </summary>
    public Text infoPanelText;
    /// <summary>
    /// The unlockFullVersion button of the info panel.
    /// </summary>
    public GameObject unlockFullVersionButton;
    /// <summary>
    /// Transparent button covering the whole screen which 'blocks' any action while the info panel is opened. If pressed the info panel is closed.
    /// </summary>
    public GameObject blocker;

    private void Awake()
    {
        SceneManager.sceneUnloaded += OnSceneExit;
    }

    /// <summary>
    /// This method is always executed when the scene is unloaded.
    /// </summary>
    void OnSceneExit(Scene scene)
    {
        CoroutinesSingleton.Instance.StopClosingUIObjectAutomatically();
    }

    // Start is called before the first frame update
    void Start()
    {
        timeUntilClosureOfInfoPanel = StaticValues.timeUntilClosureOfInfoPanel;
        fadingTimeInfoPanel = StaticValues.fadingTimeInfoPanel;
        pixelsOn = DataSaver.Instance.GetShowPixels();
        infoPanel.SetActive(false);
        blocker.SetActive(false);
        locks.SetPositionOfLockObjects(colors.orange.transform, colors.darkGreen.transform, colors.faintRed.transform) ;
        if (pixelsOn)
            pixeledColors.LoadCustomizedColors();
        colors.LoadCustomizedColors(); //always need to be loaded as their color is used for setting new colors, even if pixelMode is on
        LoadCurrentColor();
        MarkCurrentCollactablesColor();
    }

    /// <summary>
    /// Loads the current color of the snake. The position of the colorSelector object is set and the selectedColor-object is colored.
    /// Pixels the objects if pixel mode is activated.
    /// </summary>
    public void LoadCurrentColor()
    {
        //int[] c = DataSaver.Instance.GetSnakeColor();
        Color currentColor = DataSaver.Instance.GetSnakeColor().ConvertIntArrayIntoColor(); //new Color(c[0]/255f, c[1]/255f, c[2]/255f, c[3]/255f);
        currentColor.PrintRGBAValues();
        if (!pixelsOn)
        {
            selectedColor.GetComponent<Image>().color = currentColor;
            selectedColorsPixeled.SetActive(false);
            colorsPixeled.SetActive(false);
        }
        else
        {
            foreach (Transform t in selectedColorsPixeled.transform)
            { 
                t.GetComponent<Image>().color = currentColor;
            }
            selectedColor.SetActive(false);
            colors.SetNewTransparency(0);
        }
        colorSelector.transform.position = colors.ChooseObjectWithColor(currentColor).transform.position;
    }

    /// <summary>
    /// Marks the color which is currently selected as collactables color. It cannot be selected as snake color.
    /// </summary>
    public void MarkCurrentCollactablesColor()
    {
        Color collectablesColor = DataSaver.Instance.GetCollectablesColor().ConvertIntArrayIntoColor(); //the current color of the collectables
        currentCollectablesColor.parent.transform.position = colors.ChooseObjectWithColor(collectablesColor).transform.position;
        if(!pixelsOn)
        {
            currentCollectablesColor.crossPixeled.SetActive(false);
        }
        else
        {
            currentCollectablesColor.crossUnpixeled.SetActive(false);
        }
    }

    ///// <summary>
    ///// Closes the info panel automatically after a given amount of time.
    ///// </summary>
    ///// <param name="timeInMillis">The time in millis as int.</param>
    ///// <returns></returns>
    //IEnumerator CloseInfoPanelAutomatically(int timeInMillis)
    //{
    //    yield return new WaitForSecondsRealtime(timeInMillis / 1000f);
    //    if (infoPanel.activeInHierarchy)
    //        StartCoroutine(FadeInfoPanel(1500)); //the info panel fades within the passed time in millis
    //}

    ///// <summary>
    ///// Makes the info panel disappear fadingly.
    ///// </summary>
    ///// <param name="fadingTimeInMillis">The time which it takes from the beginnning to the completion of the fading.</param>
    ///// <returns></returns>
    //IEnumerator FadeInfoPanel(int fadingTimeInMillis)
    //{
    //    float timeInterval = (fadingTimeInMillis / 255f); //the time after which at a time another contribution to the fading is made
    //    int currentAlpha = 255; //the current alpha value of the info panel (when it reaches zero the object becomes fully transparent)
    //    while (currentAlpha > 0 && infoPanel.activeInHierarchy)
    //    {
    //        SetNewAlphaForInfoPanelChildren(currentAlpha);
    //        currentAlpha -= 1;
    //        yield return new WaitForSecondsRealtime(timeInterval / 1000f);
    //    }
    //    CloseInfoPanel();
    //    SetNewAlphaForInfoPanelChildren(255);
    //}

    ///// <summary>
    ///// Assigns a new alpha value to all objects that are part (children) of the info panel. Hence changes the transparency of the info panel.
    ///// </summary>
    ///// <param name="newAlphaValue">The new alpha value as int (ranging from 0 (min) to 255 (max)).</param>
    //void SetNewAlphaForInfoPanelChildren(int newAlphaValue)
    //{
    //    foreach (Transform child in infoPanel.transform)
    //    {
    //        if (child.GetComponent<Image>())
    //            child.GetComponent<Image>().color = child.GetComponent<Image>().color.GetColorWithNewA(newAlphaValue);
    //        if (child.GetComponent<Text>())
    //            child.GetComponent<Text>().color = child.GetComponent<Text>().color.GetColorWithNewA(newAlphaValue);
    //        if (child.GetComponent<Button>())
    //            foreach (Transform t in child.transform)
    //                t.GetComponent<Text>().color = t.GetComponent<Text>().color.GetColorWithNewA(newAlphaValue);
    //    }
    //} 

    //currently not needed methods (replaced by methods of the singleton 'CoroutinesSingleton')

    //methods for the info panel:

    /// <summary>
    /// Sets a new snake color and saves it to an external file. The position of the colorSelector and the color of the color-displayer are also adjusted.
    /// </summary>
    /// <param name="objectWithWantedColor">The button by which this method is called as GameObject. (Its color becomes the new snake color.)</param>
    public void SetColor(GameObject objectWithWantedColor)
    {
        Color c = objectWithWantedColor.GetComponent<Image>().color;
        Color newColor = !pixelsOn ? c : c.GetColorWithNewA(255); //if pixel mode is on, the buttons in the scene are transparent
        colorSelector.transform.position = objectWithWantedColor.transform.position;
        if(pixelsOn)
        {
            foreach (Transform t in selectedColorsPixeled.transform)
            {
                t.GetComponent<Image>().color = newColor;
            }
        }
        else
        {
            selectedColor.GetComponent<Image>().color = newColor;
        }
        DataSaver.Instance.SetSnakeColor(newColor);
    }

    //methods dealing with the info panel and displaying of information:

    /// <summary>
    /// Opens an info panel. (Which informs the player that they can't select the current collectables color.)
    /// After 10 seconds it automatically closes again.
    /// </summary>
    /// <param name="showFullVersionButton">If true a button which links this scene with the full version scene is activated.</param>
    public void OpenInfoPanel(bool showFullVersionButton)
    {
        infoPanel.SetActive(true);
        blocker.SetActive(true);
        unlockFullVersionButton.SetActive(showFullVersionButton);
        CoroutinesSingleton.Instance.CloseUIObjectAutomatically(infoPanel, timeUntilClosureOfInfoPanel, fadingTimeInfoPanel, null, blocker);
        //StartCoroutine(CloseInfoPanelAutomatically(10000));
    }

    /// <summary>
    /// Closes the info panel. (Which informs the player that they can't select the current collectables color.)
    /// </summary>
    public void CloseInfoPanel()
    {
        if (infoPanel.activeInHierarchy)
        {
            infoPanel.SetActive(false);
            blocker.SetActive(false);
            CoroutinesSingleton.Instance.StopClosingUIObjectAutomatically();
        }
    }

    //to be attached to buttons:

    /// <summary>
    /// Shows a message which informs the player that they need to unlock the full version if they want to select a certain locked color.
    /// </summary>
    public void ShowCustomColorLockedMessage()
    {
        OpenInfoPanel(true);
        infoPanelText.text = "YOU NEED TO UNLOCK THE FULL VERSION TO SELECT THIS COLOR.";
    }

    /// <summary>
    /// Shows a message which informs the player that they need to unlock the full version if they want to customize their own colors.
    /// </summary>
    void ShowCustomizingOptionLockedMessage()
    {
        OpenInfoPanel(true);
        infoPanelText.text = "YOU NEED TO UNLOCK THE FULL VERSION TO CUSTOMIZE YOUR OWN COLORS.";
    }

    /// <summary>
    /// Shows a message which informs the player that they can't select this color because it was already set as collectables color.
    /// </summary>
    public void ShowColorAlreadySelectedMessage()
    {
        OpenInfoPanel(false);
        infoPanelText.text = "YOU CAN'T SELECT THIS COLOR. IT WAS ALREADY SET AS APPLE COLOR.";
    }

    /// <summary>
    /// Loads the 'customizeColors' scene if the full version was unlocked, shows an error message elsewhise (saying the full version needs to 
    /// be unlocked for customizing colors).
    /// </summary>
    public void OpenCustomizeColorsScene()
    {
        if (FullVersion.Instance.IsFullVersionUnlocked == FullVersionUnlocked.unlocked)
            LoadCustomizeColorsScene();
        else
            ShowCustomizingOptionLockedMessage();
    }

    //methods that are required for scene interaction:

    /// <summary>
    /// Loads the collectablesColor scene. Saves the collectables-color-scene as 'GraphicsSceneLastOpened' to an external file.
    /// </summary>
    public void LoadCollectablesColorSkins()
    {
        SceneInteraction.Instance.GraphicsSceneLastOpened = GraphicsSceneLastOpened.collectablesColor;
        SceneManager.LoadScene("Skins_Collectables_Color");
    }

    /// <summary>
    /// Loads the graphicsSettings scene. Saves the graphics-settings-scene as 'GraphicsSceneLastOpened' to an external file.
    /// </summary>
    public void LoadGraphicsSettings()
    {
        SceneInteraction.Instance.GraphicsSceneLastOpened = GraphicsSceneLastOpened.graphicsSettings;
        SceneManager.LoadScene("GraphicsSettings");
    }

    /// <summary>
    /// Loads the 'CustomizeColors' Scene. Saves this scene (snake color scene) as the last opened color-scene.
    /// </summary>
    void LoadCustomizeColorsScene()
    {
        DataSaver.Instance.LastCustomizedColor = LastEditedColor.snake;
        SceneManager.LoadScene("CustomizeColors");
    }

    /// <summary>
    /// Loads the scene where the full version can be purchased.
    /// </summary>
    public void LoadUnlockFullVersionScene()
    {
        SceneManager.LoadScene("PurchaseFullVersion");
    }
}
