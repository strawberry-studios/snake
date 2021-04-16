using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Sets the color of the snake logo.
/// </summary>
public class SnakeLogoManager : MonoBehaviour
{

    public Image snakeBody, snakeDot, appleDot;
    

    private void Start()
    {
        SetColorOfSnakeLogo();
    }

    /// <summary>
    /// Sets the color of the snake logo. The snake and collectables color are used.
    /// </summary>
    public void SetColorOfSnakeLogo()
    {
        PlayerData currentData = DataSaver.Instance.RetrievePlayerDataFromFile();
        snakeBody.color = currentData.GetSnakeColor().ConvertIntArrayIntoColor();
        snakeDot.color = currentData.GetSnakeHeadColor().ConvertIntArrayIntoColor();
        appleDot.color = currentData.GetCollectablesColor().ConvertIntArrayIntoColor();
    }

}
