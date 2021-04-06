using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This singleton declares and implements several coroutines that are needed in different scenes.
/// </summary>
public class CoroutinesSingleton : Singleton<CoroutinesSingleton>
{
    private int fadingDuration; //the time within which an object and its children fade (when faded by 'CloseUIObjectAutomatically')
    private GameObject objectToFade; //the object and its children (only UIs) that will fade (when faded by 'CloseUIObjectAutomatically')
    /// <summary>
    /// The objects that should be activated as soon as 'objectToFade' has faded.
    /// </summary>
    GameObject[] objectsToAvtivate;
    /// <summary>
    /// A blocker object which makes sure that no action can be taken while an info panel is opened.
    /// </summary>
    GameObject blocker;
    private int blockerAlpha; //current alpha of the blocker image
    bool blockerOn;

    /// <summary>
    /// This method only creates the 'Coroutines' Singleton, it doesn't execute any kind of command.
    /// </summary>
    public void Create() { }

    /// <summary>
    /// Starts closing a UI object and all of its children after a passed time. The object is going to fade within a passed time.
    /// Once the UI object and its children have faded, each member of the array of passed objects is set active.
    /// </summary>
    /// <param name="timeUntilClosing">The time in millis (as int) until the object starts to fade.</param>
    /// <param name="fadingDurationInMillis">The time which it takes for the object fo fade entirely.</param>
    /// <param name="objectToBeClosed">The object (should be a UI object) that is to be closed by fading.</param>
    /// <param name="objectsToBeOpened">The objects which should be set active once the UI object faded.</param>
    /// <param name="newBlocker">A second object which should fade and be set inactive as soon as the 'objectToBeClosed' has faded.
    /// The children of this object aren't affected, it must be an image.</param>
    /// <returns></returns>
    public void CloseUIObjectAutomatically(GameObject objectToBeClosed, int timeUntilClosing, int fadingDurationInMillis, GameObject[] objectsToBeOpened, GameObject newBlocker)
    {
        //objectToBeClosed.SetNewAlphaForObjectAndChildren(255);
        fadingDuration = fadingDurationInMillis;
        objectToFade = objectToBeClosed;
        objectsToAvtivate = objectsToBeOpened;
        blocker = newBlocker;

        blockerOn = blocker ? blocker.GetComponent<Image>() : false;
        print(blockerOn);
        blockerAlpha = blockerOn ? (int) (blocker.GetComponent<Image>().color.a*255) : 140;
        print(blockerAlpha);
        //if(blocker)
        //    blockerAlpha = blocker.GetComponent<Image>() ? (int)(blocker.GetComponent<Image>().color.a * 255) : 140;
        Invoke("StartUIFadingCoroutine", timeUntilClosing / 1000f);
    }

    /// <summary>
    /// Starts the coroutine which fades the passed object and its children.
    /// </summary>
    private void StartUIFadingCoroutine()
    {
        if (objectToFade.activeInHierarchy)
        {
            StartCoroutine(FadeUIObject(objectToFade, fadingDuration));
        }
    }

    /// <summary>
    /// Makes an object and all of its children (only UIs) disappear fadingly.
    /// Once they have disappeared, the fields of 'ObjectsToActivate' will be set active.
    /// Also fades a blocker object (a UI image) if assigned (to 'blocker')
    /// </summary>
    /// <param name="fadingTimeInMillis">The time within which the object entirely fades.</param>
    /// <param name="objectToFade">The object (UI) that should disappear fadingly.</param>
    /// <returns></returns>
    IEnumerator FadeUIObject(GameObject objectToFade, int fadingTimeInMillis)
    {
        float startTime = Time.realtimeSinceStartup * 1000;
        float fadingDurationSoFar = 0;

        //float timeInterval = (fadingTimeInMillis / 51f); //the time after which at a time another contribution to the fading is made
        //int currentAlpha = 255; //the current alpha value of the object (when it reaches zero the object becomes fully transparent)

        //float stepIteration = 0;
        //float currentBlockerAlpha = 140;
        //if (blockerOn)
        //{
        //    currentBlockerAlpha = blocker.GetComponent<Image>().color.a;
        //    stepIteration = currentBlockerAlpha / 51;
        //    blockerAlpha = (int)(currentBlockerAlpha*255);
        //}

        while(fadingDurationSoFar < fadingTimeInMillis && objectToFade.activeInHierarchy)
        {
            fadingDurationSoFar = (Time.realtimeSinceStartup * 1000) - startTime;
            print(fadingDurationSoFar);

            float transparencyFactor = 1 - (fadingDurationSoFar / fadingTimeInMillis);
            objectToFade.SetNewAlphaForObjectAndChildren((int) (transparencyFactor*255));
            if (blockerOn) 
                blocker.GetComponent<Image>().SetNewAlphaForImage((int) (transparencyFactor*blockerAlpha));

            yield return null;
        }

        //while (currentAlpha > 0 && objectToFade.activeInHierarchy)
        //{
        //    objectToFade.SetNewAlphaForObjectAndChildren(currentAlpha);
        //    currentAlpha -= 5;
        //    if(blockerOn)
        //    {
        //        currentBlockerAlpha -= stepIteration;
        //        blocker.GetComponent<Image>().SetNewAlphaForImage(currentBlockerAlpha);
        //    }
        //    yield return new WaitForSecondsRealtime(timeInterval / 1000f);
        //}

        objectToFade.SetActive(false);
        objectToFade.SetNewAlphaForObjectAndChildren(255);

        if (blockerOn)
        {
            blocker.GetComponent<Image>().SetNewAlphaForImage(blockerAlpha);
            blocker.SetActive(false);
        }
        //if (blockerOn)
        //{
        //    blocker.GetComponent<Image>().SetNewAlphaForImage(blockerAlpha);
        //    blocker.SetActive(false);
        //}
        else if (blocker)
            blocker.SetActive(false);
        if(objectsToAvtivate != null)
            foreach (GameObject g in objectsToAvtivate)
                g.SetActive(true);
    }

    /// <summary>
    /// Stops the invoke which will close a UI object automatically. (Only if it's currently running.)
    /// Akso stops the couroutines. (Resets the transparency of the blocker if an image object is attached.
    /// </summary>
    public void StopClosingUIObjectAutomatically()
    {
        CancelInvoke();
        StopAllCoroutines();

        if(objectToFade)
            objectToFade.SetNewAlphaForObjectAndChildren(255);

        if (blocker)
        {
            if (blockerOn)
                blocker.GetComponent<Image>().SetNewAlphaForImage(blockerAlpha);
        }

        //if (blocker)
        //{
        //    Image g = blocker.GetComponent<Image>();
        //    if (g)
        //        g.SetNewAlphaForImage(blockerAlpha);
        //}
    }
}
