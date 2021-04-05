using UnityEngine;


/// <summary>
/// This class controlls what should happen when the game is loaded for the first time.
/// The PlayerData is initialized, the default settings are applied
/// </summary>
public class SetupOfGame : MonoBehaviour
{

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

}
