using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateWorld : MonoBehaviour
{
    public GameObject gridHorizontalPole, gridVerticalPolePrefab; //references to the prefabs that need to be cloned to set up the grid
    private GameObject gridVerticalPole; //a properly scaled modification of the gridVerticalPolePrefab
    private Camera mainCamera; //the camera in the scene
    int rows; //variable which determines how many rows will exist in which the snake can move
    Quaternion horizontalRotation; //rotation of the horizontalGridPoles
    int columns;
    float distanceBetweenSqaures; // distance between rows or columns, i.e. length or bridth of a square on the playing area
    float heightScreenProportion; //the factor which determines which part of the screen (vertically) will be comsumed by the gaming area
    public bool GridLinesOn { get; set; } //whether the grid lines are activated or not
    /// <summary>
    /// Game object parenting all poles
    /// </summary>
    private GameObject polesHolder; 
    /// <summary>
    /// Parents all objects (cuboids) that are used for creating the illusion of pixeled apples and snake blocks
    /// </summary>
    public GameObject PixelModeObjects { get; set; } 
    /// <summary>
    /// Game object which parents all components that are atually visible during gameplay.
    /// </summary>
    public GameObject actualGameComponents;
    /// <summary>
    /// The center of the world on the z-axis
    /// </summary>
    public float ZOrigin { get; set; }


    /// <summary>
    /// How thick the gridLines in the scene should be
    /// </summary>
    public float GridLinesFactor
    { get; set; }  

    // Start is called before the first frame update
    void Awake()
    {
        SceneInteractionData d = new SceneInteractionData();
        heightScreenProportion = .66f;

        mainCamera = Camera.main;
        columns = DataSaver.Instance.GetWorldSize();
        //print(columns);
        distanceBetweenSqaures = 2 * Camera.main.orthographicSize * Camera.main.aspect / columns;
        //print(distanceBetweenSqaures);
        horizontalRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);

        float rowsAsFloat = 2 * Camera.main.orthographicSize * heightScreenProportion / distanceBetweenSqaures;
        if(Mathf.Round(rowsAsFloat) > rowsAsFloat)
        {
            rows = Mathf.RoundToInt(rowsAsFloat - .5f);
        }
        else
        {
            rows = Mathf.RoundToInt(rowsAsFloat);
        }

        GridLinesOn = DataSaver.Instance.GetShowGridLines();
        gridVerticalPole = gridVerticalPolePrefab;
        gridVerticalPole.transform.localScale = new Vector3(gridVerticalPole.transform.localScale.x, gridVerticalPole.transform.localScale.y, rows * distanceBetweenSqaures);
        polesHolder = actualGameComponents.GetOrAddEmptyGameObject("gridComponentsParent");
        PixelModeObjects = actualGameComponents.GetOrAddEmptyGameObject("pixelModeComponents");

        ZOrigin = Camera.main.orthographicSize - ((float)(rows / 2.0f) * distanceBetweenSqaures);
        //the grid lines are set up if activated:
        CreateGridLines();

        //the pixel mode is set up if activated:
        if(DataSaver.Instance.GetShowPixels())
            CreatePixelModeObjects();
        }

    /// <summary>
    /// Creates the grid lines of the world matrix if the option gridLinesOn was selected by the player. The bool value is retrieved from 
    /// an external file where the player's preferences are saved. The margins of the world matrix are always marked.
    /// </summary>
    void CreateGridLines()
    {
        LoadGridLinesThickness();

        if (GridLinesOn)
        {
            ColumnsSetup();
            RowsSetup();
            RescaleGridLines();
        }
        else
            CreateMargins();
        }

    /// <summary>
    /// The grid lines are scaled depending on the size of the world. If the size is minimal, gridLinesFactor is 0.3; 
    /// if the size is maximal, gridLinesFactor is 1. The function describing which gridLinesFactor belongs to which
    /// worldSize develops linearly.
    /// </summary>
    void LoadGridLinesThickness()
    {
        //columns has the same value as the world size, min world size is 6 and max world size is 30, 
        //in this case the gridlines are scales to 2/5 of their original thickness:
        GridLinesFactor =  47/40f -.7f/24 * columns; // 1.15f - 0.025f * columns;
    }

    /// <summary>
    /// The rows in which the snake can move are created
    /// </summary>
    private void RowsSetup()
    {
        if(rows%2 == 0) //scenario for even number of rows
        {
            GameObject horizontalPoleCenter = 
                Instantiate(gridHorizontalPole, new Vector3(0.0f, 3.0f, ZOrigin), horizontalRotation);
            horizontalPoleCenter.transform.SetParent(polesHolder.transform);

            for(int i = 1; i <= rows/2; i += 1)
            {
                GameObject horizontalPoleRight = 
                    Instantiate(gridHorizontalPole, new Vector3(0.0f, 3.0f, i*distanceBetweenSqaures + ZOrigin), horizontalRotation);
                GameObject horizontalPoleLeft =
                    Instantiate(gridHorizontalPole, new Vector3(0.0f, 3.0f, -i*distanceBetweenSqaures + ZOrigin), horizontalRotation);
                horizontalPoleLeft.transform.SetParent(polesHolder.transform);
                horizontalPoleRight.transform.SetParent(polesHolder.transform);
            }
        }
        else //scenario for odd number of rows
        {
            for(int i = 0; i <= (rows-1)/2; i += 1)
            {
                //double f = (double)i + 0.5;
                //print(f);
                GameObject horizontalPoleRight = 
                    Instantiate(gridHorizontalPole, new Vector3(0.0f, 3.0f, ((float)i + 0.5f)*distanceBetweenSqaures + ZOrigin), horizontalRotation);
                GameObject horizontalPoleLeft = 
                    Instantiate(gridHorizontalPole, new Vector3(0.0f, 3.0f, -((float)i + 0.5f)*distanceBetweenSqaures + ZOrigin), horizontalRotation);
                horizontalPoleRight.transform.SetParent(polesHolder.transform);
                horizontalPoleLeft.transform.SetParent(polesHolder.transform);
            }
        }
    }

    /// <summary>
    /// The columns in which the snake can move are created
    /// </summary>
    private void ColumnsSetup()
    {
        if(columns%2 == 0) //scenario for even number of columns
        {
            GameObject verticalPoleCenter = 
                Instantiate(gridVerticalPole, new Vector3(0.0f, 3.0f, ZOrigin), Quaternion.identity);
            verticalPoleCenter.transform.SetParent(polesHolder.transform);

            for(int i = 1; i <= columns/2; i += 1)
            {
                GameObject verticalPoleRight = 
                    Instantiate(gridVerticalPole, new Vector3(i * distanceBetweenSqaures, 3.0f, ZOrigin), Quaternion.identity);
                GameObject verticalPoleLeft = 
                    Instantiate(gridVerticalPole, new Vector3(-i * distanceBetweenSqaures, 3.0f, ZOrigin), Quaternion.identity);
                verticalPoleRight.transform.SetParent(polesHolder.transform);
                verticalPoleLeft.transform.SetParent(polesHolder.transform);
            }
        }
        else //scenario for odd number of columns
        {
            for(int i = 0; i <= (columns-1)/2; i += 1)
            {
                GameObject verticalPoleRight =
                    Instantiate(gridVerticalPole, new Vector3(((float)i + 0.5f)*distanceBetweenSqaures, 3.0f, ZOrigin), Quaternion.identity);
                GameObject verticalPoleLeft =
                    Instantiate(gridVerticalPole, new Vector3(-((float)i + 0.5f)*distanceBetweenSqaures, 3.0f, ZOrigin), Quaternion.identity);
                verticalPoleRight.transform.SetParent(polesHolder.transform);
                verticalPoleLeft.transform.SetParent(polesHolder.transform);
            }
        }
        //CreateMargins(columns/2 * distanceBetweenSqaures);
    }

    /// <summary>
    /// Rescales all of the gridLine objects which were created so that their thickness corresponds to the 'gridLinesThickness'.
    /// </summary>
    void RescaleGridLines()
    {
        foreach (Transform t in polesHolder.transform)
            t.localScale = new Vector3(t.localScale.x * GridLinesFactor, t.localScale.y, t.localScale.z);
    }

    /// <summary>
    /// Only created gridLines for the margins of the world matrix. (One at a time for bottom, top, left and right of the world matrix)
    /// </summary>
    private void CreateMargins()
    {
        //setup of top and bottom margin:
        float topOfScreen = Camera.main.orthographicSize;
        GameObject horizontalPoleTop =
                Instantiate(gridHorizontalPole, new Vector3(0.0f, 3.0f, topOfScreen), horizontalRotation);
        GameObject horizontalPoleBottom =
                Instantiate(gridHorizontalPole, new Vector3(0.0f, 3.0f, topOfScreen - rows * distanceBetweenSqaures), horizontalRotation);
        horizontalPoleBottom.transform.localScale = new Vector3(horizontalPoleBottom.transform.localScale.x / 2,
                    horizontalPoleBottom.transform.localScale.y, horizontalPoleBottom.transform.localScale.z);
        horizontalPoleTop.transform.SetParent(polesHolder.transform);
        horizontalPoleBottom.transform.SetParent(polesHolder.transform);

        //setup of left and right margin:
        float columnsZ = Camera.main.orthographicSize - (rows / 2.0f * distanceBetweenSqaures);
        GameObject verticalPoleRight =
                    Instantiate(gridVerticalPole, new Vector3(columns/2f * distanceBetweenSqaures, 3.0f, columnsZ), Quaternion.identity);
        GameObject verticalPoleLeft =
            Instantiate(gridVerticalPole, new Vector3(-(columns / 2f) * distanceBetweenSqaures, 3.0f, columnsZ), Quaternion.identity);
        verticalPoleRight.transform.SetParent(polesHolder.transform);
        verticalPoleLeft.transform.SetParent(polesHolder.transform);
    }

    /// <summary>
    /// Black cuboids are created which create the illusion of pixeled snake blocks and apples.
    /// Depending on the world size and gridLinesOn state the positions, scales and quantity of the objects varies.
    /// </summary>
    private void CreatePixelModeObjects()
    {
        SpawnPixeledBlocks spawner = new SpawnPixeledBlocks();
    }

    /// <summary>
    /// Returns the length of a square of the gaming area
    /// </summary>
    /// <returns>Returns a float</returns>
    public float GetSquareLength()
    {
        return distanceBetweenSqaures;
    }

    /// <summary>
    /// Returns the number of a columns of the gaming area
    /// </summary>
    /// <returns>Returns an integer</returns>
    public int GetColumns()
    {
        return columns;
    }

    /// <summary>
    /// Returns the number of a rows of the gaming area
    /// </summary>
    /// <returns>Returns an integer</returns>
    public int GetRows()
    {
        return rows;
    }

    /// <summary>
    /// Returns the left margin of the screen in world coordinates
    /// </summary>
    /// <returns>left margin of the screen as a float</returns>
    public float GetLeftMarginOfScreen()
    {
        return -(mainCamera.orthographicSize * mainCamera.aspect);
    }

    /// <summary>
    /// Returns the upper margin of the screen in world coordinates
    /// </summary>
    /// <returns>upper margin of the screen as a float</returns>
    public float GetUpperMarginOfScreen()
    {
        return mainCamera.orthographicSize;
    }
}
