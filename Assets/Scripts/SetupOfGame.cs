using UnityEngine;


/// <summary>
/// This class controlls what should happen when the game is loaded for the very first time or when a new session is started.
/// The PlayerData is initialized, the default settings are applied
/// </summary>
public class SetupOfGame : MonoBehaviour
{

    private void Awake()
    {
        SetUIWidthFactor();
    }


    void Start()
    {
        if (PlayerProgress.Instance.IsGameFirstLoaded == GameFirstLoaded.firstLoaded) //creates playerProgress.dat (if not existent yet)
        {
            print("Game was loaded for the first time!");

            //create files that persist information if they don't exist yet (they may already exist if the app is only updated):
            DataSaver.Instance.RetrievePlayerDataFromFile(); //creates playerinfo.dat and difficulty.dat
            SceneInteraction.Instance.RetrieveSceneInteractionDataFromFile(); //creates sceneInteraction.dat

            PlayerProgress.Instance.IsGameFirstLoaded = GameFirstLoaded.notFirstLoaded;
        }
    }


    /// <summary>
    /// This method sets the UI width factor. The UIs were concepted for a screen aspect ratio of 16:9, if the actual aspect ratio is different,
    /// the width (x-scale) of all UIs is modified (multiplied by this factor). This method is only called once per loading of the game (at the very
    /// beginning).
    /// </summary>
    void SetUIWidthFactor()
    {
        //the default aspect ratio is 16:9, if the aspect ratio of a device is different, the UIs which were created for the ratio 16:9 must be 
        //modified: this is the factor the x-scale of the UIs needs to be multiplied by:
        float actualAspectRatio = Camera.main.aspect;
        float defaultAspectRatio = 9 / 16f;
        float factor = actualAspectRatio / defaultAspectRatio;
        StaticValues.UIWidthFactor = factor;
        print(factor);
    }

}
