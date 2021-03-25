using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

/// <summary>
/// Provides methods for guaranteeing a proper scene interaction. I.e. for saving and loading the last opened options/graphics menu etc.
/// The information is saved to an external file. (One could also use PlayerPrefs for this information, if it is altered by hackers (-: 
/// it is fairly irrelevant.)
/// </summary>
public class SceneInteraction : Singleton<SceneInteraction>
{
    /// <summary>
    /// Get or set the graphics scene that was opened at last to/from an external file (return/assignment type: 'GraphicsSceneLastOpened')
    /// /// </summary>
    public GraphicsSceneLastOpened GraphicsSceneLastOpened
    { get
        {
            SceneInteractionData data = RetrieveSceneInteractionDataFromFile();
            if(data != null) 
                return data.GraphicsSceneLastOpened;
            else
            {
                print("The last opened graphics scene couldn't be retrieved from an external file. 'graphicsSettings' is returned by default.");
                return GraphicsSceneLastOpened.graphicsSettings;
            }
        }
      set
        {
            SceneInteractionData data = RetrieveSceneInteractionDataFromFile();
            data.GraphicsSceneLastOpened = value;
            SaveSceneInteractionDataToFile(data);
        }
    }

    /// <summary>
    /// Get or set the options scene that was opened at last to/from an external file (return/assignment type: 'OptionsSceneLastOpened')
    /// /// </summary>
    public OptionsSceneLastOpened OptionsSceneLastOpened
    {
        get
        {
            SceneInteractionData data = RetrieveSceneInteractionDataFromFile();
            if (data != null)
                return data.OptionsSceneLastOpened;
            else
            {
                print("The last opened graphics scene couldn't be retrieved from an external file. 'difficulty' is returned by default.");
                return OptionsSceneLastOpened.difficulty;
            }
        }
        set
        {
            SceneInteractionData data = RetrieveSceneInteractionDataFromFile();
            data.OptionsSceneLastOpened = value;
            SaveSceneInteractionDataToFile(data);
        }
    }

    /// <summary>
    /// This method retrieves the sceneInteraction data from an external file where it is saved.
    /// </summary>
    /// <returns>It returns an object of the type SceneInteractionData which holds all of the scene-interaction-specific data.</returns>
    public SceneInteractionData RetrieveSceneInteractionDataFromFile()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/sceneInteraction.dat"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/sceneInteraction.dat", FileMode.Open);
            SceneInteractionData data = (SceneInteractionData)bf.Deserialize(file);
            file.Close();

            return data;
        }
        else
        {
            Debug.Log("Scene interaction data couldn't be retrieved from the external file. Check whether the persistentDataPath " +
                "addresses the right directory");

            FileStream file = File.Create(Application.persistentDataPath + "/sceneInteraction.dat");
            SceneInteractionData newSceneInteractionData = new SceneInteractionData();

            bf.Serialize(file, newSceneInteractionData);
            file.Close();

            return newSceneInteractionData;
        }
    }

    /// <summary>
    /// This method saves scene interaction data to an external persistent file.
    /// </summary>
    /// <param name="toBeSaved">The object of the type SceneInteractionData (which holds the information 
    /// which should be saved) to pass.</param>
    public void SaveSceneInteractionDataToFile(SceneInteractionData toBeSaved)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/sceneInteraction.dat");

        bf.Serialize(file, toBeSaved);
        file.Close();
    }
}

public enum GraphicsSceneLastOpened { graphicsSettings, snakeColor, collectablesColor };
public enum OptionsSceneLastOpened { difficulty, preferences};

[Serializable]
public class SceneInteractionData
{
    public SceneInteractionData()
    {
        //Debug.Log(GraphicsSceneLastOpened);
        //Debug.Log(OptionsSceneLastOpened);
        //enum variables are never 0 and correspond to the first-declared option for their value, the following statements wouldn't really be necessary
        OptionsSceneLastOpened = OptionsSceneLastOpened.difficulty;
        GraphicsSceneLastOpened = GraphicsSceneLastOpened.graphicsSettings;
    }

    /// <summary>
    /// Get or set the graphics scene that was opened at last (return/assignment type: 'GraphicsSceneLastOpened')
    /// </summary>
    public GraphicsSceneLastOpened GraphicsSceneLastOpened
    { get; set; }

    /// <summary>
    /// References the options scene that was opened at last (saved as a string which corresponds to the name of the scene).
    /// </summary>
    public OptionsSceneLastOpened OptionsSceneLastOpened
    { get; set; }

}
