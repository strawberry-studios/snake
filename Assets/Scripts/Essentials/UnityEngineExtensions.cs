using UnityEngine;
using UnityEngine.UI;
using TMPro;

static public class UnityEngineExtensions
{
    /// <summary>
    /// Returns the component of Type type. If one doesn't already exist on the GameObject it will be added.
    /// </summary>
    /// <typeparam name="T">The type of Component to return.</typeparam>
    /// <param name="gameObject">The GameObject this Component is attached to.</param>
    /// <returns>Component</returns>
    static public T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
    }

    /// <summary>
    /// Returns a child of the calling game object which is named the passed name. If it doens't exist a new one is created and returned.
    /// </summary>
    /// <param name="gameObject">The calling game object.</param>
    /// <param name="name">The tag which is searched for/assigned to the new object.</param>
    /// <returns></returns>
    static public GameObject GetOrAddEmptyGameObject(this GameObject gameObject, string name)
    {
        foreach(Transform t in gameObject.transform)
        {
            if(t.name == name)
                return t.gameObject;
        }
        GameObject child = new GameObject();
        child.name = name;
        child.transform.SetParent(gameObject.transform);
        return child;
    }

    /// <summary>
    /// Assigns a new alpha value to the game object which calls this method and to all of its children.
    /// It thereby changes the transparency of the object group. Only works for UI elements (in particular for text and image components).
    /// </summary>
    /// <param name="newAlphaValue">The new alpha value as int (ranging from 0 (min) to 255 (max)).</param>
    public static void SetNewAlphaForObjectAndChildren(this GameObject thisObject, int newAlphaValue)
    {
        if (thisObject.GetComponent<Image>())
            thisObject.GetComponent<Image>().color = thisObject.GetComponent<Image>().color.GetColorWithNewA(newAlphaValue);
        if (thisObject.GetComponent<Text>())
            thisObject.GetComponent<Text>().color = thisObject.GetComponent<Text>().color.GetColorWithNewA(newAlphaValue);
        if (thisObject.GetComponent<TextMeshProUGUI>())
            thisObject.GetComponent<TextMeshProUGUI>().color = thisObject.GetComponent<TextMeshProUGUI>().color.GetColorWithNewA(newAlphaValue);

        foreach (Transform child in thisObject.transform)
        {
            child.gameObject.SetNewAlphaForObjectAndChildren(newAlphaValue);
        }
    }

    /// <summary>
    /// Assigns a new alpha value to the calling UI image.
    /// It thereby changes the transparency of the object. 
    /// </summary>
    /// <param name="newAlphaValue">The new alpha value as int (ranging from 0 (min) to 255 (max)).</param>
    public static void SetNewAlphaForImage(this Image thisImage, int newAlphaValue)
    {
        thisImage.color = thisImage.color.GetColorWithNewA(newAlphaValue);
    }

    /// <summary>
    /// Assigns a new alpha value to the calling UI image.
    /// It thereby changes the transparency of the object. 
    /// </summary>
    /// <param name="newAlphaValue">The new alpha value as float (ranging from 0 (min) to 1 (max)).</param>
    public static void SetNewAlphaForImage(this Image thisImage, float newAlphaValue)
    {
        thisImage.color = thisImage.color.GetColorWithNewA(newAlphaValue);
    }

    /// <summary>
    /// Rounds a float to the closest uneven number and returns it as int.
    /// </summary>
    /// <param name="number">The number that should be rounded.</param>
    /// <returns></returns>
    public static int RoundToUnevenNumber(float number)
    {
        return 2*Mathf.RoundToInt((number-1)/2) + 1;
    }

    /// <summary>
    /// Rounds a float to the closest even number and returns it as int.
    /// </summary>
    /// <param name="number">The number that should be rounded.</param>
    /// <returns></returns>
    public static int RoundToEvenNumber(float number)
    {
        return 2 * Mathf.RoundToInt(number / 2);
    }
}
