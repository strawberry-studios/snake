using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the spawning of new collectables (blocks that can be collected). The public method CreateNewCollectable() can be 
/// called to spawn a new collectable at a free position (on a field that isn't 'occupied' by the snake). The method 
/// CreateNewCollectableDelayed() chooses any position where a new collectable will be spawned, yet the actual spawning is delayed until the 
/// field isn't occupied by a snake-block any more.
/// </summary>
public class SpawnCollectablesSimplified : MonoBehaviour
{
    private int squares; //the number of squares/fields of the world
    public GameObject snakeHead; //references the head of the snake
    public GameObject collectablesPrefab; //references the prefab which is instantiated when a new collectable is created

    /// <summary>
    /// These bools symbolize the fields of the world. The fields that are currently occupied by snake-blocks are allocated false,
    // each other field is allocated true. The first index means the row of the field (where the block is located), 
    // the second one the column.
    /// </summary>
    public bool[,] CurrentlyOccupiedFields
    {
        get;
        set;
    }

    /// <summary>
    /// The number of rows of the world.
    /// </summary>
    public int Rows
    { get; set; }

    /// <summary>
    /// The number of columns of the world.
    /// </summary>
    public int Columns
    { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Rows = GetComponent<CreateWorld>().GetRows();
        Columns = GetComponent<CreateWorld>().GetColumns();
        squares = GetSquares();
        CreateNewCollectable();
    }

    /// <summary>
    /// A new collectable is immediately created at any position that is not occupied by a snake-block (the position is randomly chosen)
    /// </summary>
    public void CreateNewCollectable()
    {
        bool[,] currentlyOccupiedFields = snakeHead.GetComponent<SnakeHeadController>().StartDeterminingOccupiedFields(); //holds the information
                             // whether a field is occupied by the snake or not for each field (first index =^ rows, second index =^ columns)
        //checks whether the game is won and therefore over:
        int freeSquares = squares - snakeHead.GetComponent<SnakeBlockController>().CountCurrentBlocks();
        if (freeSquares == 0)
        {
            snakeHead.GetComponent<SnakeHeadController>().Lose(true);
        }
        else
        {
            //one of the free fields is chosen randomly and a new collectible is spawned there:
            int spawnRow, spawnColumn;
            int randomlyChosenField = 1 + (int)(Random.Range(0, freeSquares - 0.001f));
            int counter = 0;
            for (int i = 0; i < Rows; i++)
            {
                for (int k = 0; k < Columns; k++)
                {
                    counter += currentlyOccupiedFields[i, k] == false ? 1 : 0;
                    if (randomlyChosenField == counter)
                    {
                        spawnRow = i + 1;
                        spawnColumn = k + 1;
                        counter++;
                        GameObject collectable = Instantiate(collectablesPrefab, snakeHead.GetComponent<SnakeBlockController>().ConvertIntsIntoPosition(
                                                    spawnRow, spawnColumn), Quaternion.identity);
                        collectable.SetActive(true);
                        break;
                    }
                    else if (counter > randomlyChosenField)
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Chooses any position randomly at which a new collectable will be spawned. The actual spawning takes place once the square is free (no longer
    /// occupied by one of the snake blocks).
    /// </summary>
    public void CreateNewCollectableDelayed()
    {
        int rowsIndex = (int)Random.Range(0, Rows - 0.001f) + 1;  
        int columnsIndex = (int)Random.Range(0, Columns - 0.001f) + 1;
        Vector3 position = snakeHead.GetComponent<SnakeBlockController>().ConvertIntsIntoPosition(rowsIndex, columnsIndex);
        
        //spawn the new collectible if it doesn't have the same position as the snake head, if it has the same position the spawning should
        //be delayed until the snake moved (block can also spawn under the snake, yet can only be collected by its head:
        int snakeHeadColumn = snakeHead.GetComponent<SnakeBlockController>().GetCurrentColumn();
        int snakeHeadRow = snakeHead.GetComponent<SnakeBlockController>().GetCurrentRow();
        if (snakeHeadColumn == columnsIndex && snakeHeadRow == rowsIndex)
        {
            //Instantiate the new collectable once the snakeHead moved:
            StartCoroutine(snakeHead.GetComponent<SnakeBlockController>().CreateNewCollectableDelayed(collectablesPrefab, position));
        }
        else
        {
            //Instantiate the new collectable immidiately:
            GameObject collectable = Instantiate(collectablesPrefab, position, Quaternion.identity);
            collectable.SetActive(true);
        }
    }
    
    /// <summary>
    /// Returns the number of squares/fields of the world.
    /// </summary>
    int GetSquares()
    {
        return Columns * Rows;
    }
}


/// <summary>
/// This class provides methods for spawning pixeled blocks. None of these methods are executed on their won, 
/// they're only called by other scripts (mainly 'SnakeHeadController')
/// </summary>
//public static class SpawnPixeledCollectables
//{
//    /// <summary>
//    /// The game object which all game-managing scripts are attached to
//    /// </summary>
//    static GameObject gameModeManager;
//    /// <summary>
//    /// The size of the pixels, is approximately equal to the thickness of gridLines, if activated
//    /// </summary>
//    static float pixelSize;
//    /// <summary>
//    /// The length of a square of the world matrix (minus the thickness of the gridLines, if activated)
//    /// </summary>
//    static float length;
//    /// <summary>
//    /// The number of pixels which one snake block will consist of.
//    /// </summary>
//    static int pixelsNumber;
//    /// <summary>
//    /// The parent of the pixel objects which form a snake block as template to be instantiated.
//    /// </summary>
//    static GameObject snakeBlockParentTemplate;
//    /// <summary>
//    /// The minimal position where the first pixel is spawned. Required for the pixel spawning iteration.
//    /// </summary>
//    static float minPosition;
//    /// <summary>
//    /// The pixel template that can be duplicated for creating the single pixels of a snake block.
//    /// </summary>
//    static GameObject pixel;

//    /// <summary>
//    /// Sets up the necessary fields etc. for spawning snake blocks when pixel mode and the grid lines are activated.
//    /// </summary>
//    public void SetupPixelsAndGridLines(GameObject snakeBlockPrefab, GameObject parentOfSnakeBlocks)
//    {
//        //the length of a block and pixel size are determined:

//        gameModeManager = GameObject.FindGameObjectWithTag("GameModeManager");
//        CreateWorld cW = gameModeManager.GetComponent<CreateWorld>();
//        pixelSize = cW.gridHorizontalPole.transform.lossyScale.x * cW.GridLinesFactor;
//        length = cW.GetSquareLength() - pixelSize;

//        //the number of pixels per snake block and the actual, accurate pixel size are determined:

//        pixelsNumber = UnityEngineExtensions.RoundToUnevenNumber(length / pixelSize);
//        pixelSize = length / pixelsNumber;

//        //Creates the template that can be instantiated for the creation of the snake block parents:

//        snakeBlockParentTemplate = GameObject.Instantiate(snakeBlockPrefab) as GameObject;
//        snakeBlockParentTemplate.SetActive(false);
//        snakeBlockParentTemplate.transform.SetParent(parentOfSnakeBlocks.transform);

//        //Creates the template that can be duplicated for creating the actual pixel objects:

//        pixel = GameObject.CreatePrimitive(PrimitiveType.Cube);
//        //Color colorOfTheSnake = DataSaver.Instance.GetSnakeColor().ConvertIntArrayIntoColor(); 
//        //pixel.GetComponent<Renderer>().material.SetColor("_EmissionColor", colorOfTheSnake);
//        //pixel.GetComponent<Renderer>().material.SetColor("_Color", colorOfTheSnake);
//        //pixel.AddComponent<SetSnakeColor>();
//        pixel.SetActive(false);
//        pixel.transform.SetParent(parentOfSnakeBlocks.transform);
//        pixel.transform.localScale = new Vector3(pixelSize, 1, pixelSize);
//        pixel.GetComponent<Collider>().isTrigger = true;

//        //sets the minimal position for the spawning iteration:

//        minPosition = (-(int)(pixelsNumber / 2) + 1) * pixelSize;

//        //optional print statements for debugging:
//        //print("PixelSize:" + pixelSize); print("LengthOfASquare:" + length);
//        //print("PixelsNumber" + pixelsNumber); print("minPosition:" + minPosition);
//    }


//    /// <summary>
//    /// Modifies the snake head object. If pixel mode is activated and if grid lines are on this method properly 'converts' the
//    /// snake head into a pixeled snake head.
//    /// </summary>
//    /// <param name="snakeHead">The snake head as game object.</param>
//    /// <param name="cubeLength">The length of a snake head block as flaot.</param>
//    public void ModifySnakeHeadPixeledGridLines(GameObject snakeHead)
//    {
//        //modify the existing snake-head object:

//        snakeHead.GetComponent<MeshRenderer>().enabled = false;
//        snakeHead.transform.localScale = Vector3.one;
//        snakeHead.GetComponent<BoxCollider>().size = new Vector3(length, 1, length);

//        //create pixel-children:

//        for (int i = 0; i < pixelsNumber / 2; i++)
//        {
//            for (int j = 0; j < pixelsNumber / 2; j++)
//            {
//                GameObject smallPixel = GameObject.Instantiate(pixel, Vector3.zero, Quaternion.identity) as GameObject;
//                smallPixel.transform.SetParent(snakeHead.transform);
//                smallPixel.transform.localPosition = new Vector3(minPosition + 2 * i * pixelSize, 1,
//                                minPosition + 2 * j * pixelSize);
//                smallPixel.SetActive(true);
//                smallPixel.AddComponent<SetSnakeColor>();
//            }
//        }
//        //snakeHead.GetComponent<SnakeBlockController>().SetColorChildren();
//    }
//}
