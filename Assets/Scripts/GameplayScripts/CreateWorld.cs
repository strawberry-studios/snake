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
    private GameObject polesHolder; //a game object which will parent all poles
    public GameObject actualGameComponents; //an empty game object which parents all objects that are really visible during the gameplay

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

        gridVerticalPole = gridVerticalPolePrefab;
        gridVerticalPole.transform.localScale = new Vector3(gridVerticalPole.transform.localScale.x, gridVerticalPole.transform.localScale.y, rows * distanceBetweenSqaures);
        polesHolder = actualGameComponents.GetOrAddEmptyGameObject("gridComponentsParent");

        CreateGridLines();

        //ShowPixelsSetup();
        }

    /// <summary>
    /// Creates the grid lines of the world matrix if the option gridLinesOn was selected by the player. The bool value is retrieved from 
    /// an external file where the player's preferences are saved. The margins of the world matrix are always marked.
    /// </summary>
    void CreateGridLines()
    {
        LoadGridLinesThickness();

        if (DataSaver.Instance.GetShowGridLines())
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
        float rowsZOrigin = Camera.main.orthographicSize - ((float)(rows/2.0f) * distanceBetweenSqaures);
        if(rows%2 == 0) //scenario for even number of rows
        {
            GameObject horizontalPoleCenter = 
                Instantiate(gridHorizontalPole, new Vector3(0.0f, 3.0f, rowsZOrigin), horizontalRotation);
            horizontalPoleCenter.transform.SetParent(polesHolder.transform);

            for(int i = 1; i <= rows/2; i += 1)
            {
                GameObject horizontalPoleRight = 
                    Instantiate(gridHorizontalPole, new Vector3(0.0f, 3.0f, i*distanceBetweenSqaures + rowsZOrigin), horizontalRotation);
                GameObject horizontalPoleLeft =
                    Instantiate(gridHorizontalPole, new Vector3(0.0f, 3.0f, -i*distanceBetweenSqaures + rowsZOrigin ), horizontalRotation);
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
                    Instantiate(gridHorizontalPole, new Vector3(0.0f, 3.0f, ((float)i + 0.5f)*distanceBetweenSqaures + rowsZOrigin), horizontalRotation);
                GameObject horizontalPoleLeft = 
                    Instantiate(gridHorizontalPole, new Vector3(0.0f, 3.0f, -((float)i + 0.5f)*distanceBetweenSqaures + rowsZOrigin), horizontalRotation);
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
        float columnsZ = Camera.main.orthographicSize - (rows/2.0f * distanceBetweenSqaures);
        if(columns%2 == 0) //scenario for even number of columns
        {
            GameObject verticalPoleCenter = 
                Instantiate(gridVerticalPole, new Vector3(0.0f, 3.0f, columnsZ), Quaternion.identity);
            verticalPoleCenter.transform.SetParent(polesHolder.transform);

            for(int i = 1; i <= columns/2; i += 1)
            {
                GameObject verticalPoleRight = 
                    Instantiate(gridVerticalPole, new Vector3(i * distanceBetweenSqaures, 3.0f, columnsZ), Quaternion.identity);
                GameObject verticalPoleLeft = 
                    Instantiate(gridVerticalPole, new Vector3(-i * distanceBetweenSqaures, 3.0f, columnsZ), Quaternion.identity);
                verticalPoleRight.transform.SetParent(polesHolder.transform);
                verticalPoleLeft.transform.SetParent(polesHolder.transform);
            }
        }
        else //scenario for odd number of columns
        {
            for(int i = 0; i <= (columns-1)/2; i += 1)
            {
                GameObject verticalPoleRight =
                    Instantiate(gridVerticalPole, new Vector3(((float)i + 0.5f)*distanceBetweenSqaures, 3.0f, columnsZ), Quaternion.identity);
                GameObject verticalPoleLeft =
                    Instantiate(gridVerticalPole, new Vector3(-((float)i + 0.5f)*distanceBetweenSqaures, 3.0f, columnsZ), Quaternion.identity);
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
    /// Horizontal and vertical black cuboids are placed to manipulate the player into thinking
    /// the objects consisted of pixels.
    /// </summary>
    private void ShowPixelsSetup()
    {
        if (DataSaver.Instance.GetShowPixels())
        {
            int pixelRowsPerSquare; //number of rows of pixels per square which simulate the pixel-objects
            float startXCoordinate = -Camera.main.orthographicSize * Camera.main.aspect; //start x coordinate
            pixelRowsPerSquare = 2 * ((int)(distanceBetweenSqaures / 0.1f)); //number should be even
            float distance = distanceBetweenSqaures / pixelRowsPerSquare; //the distance between 2 pixel columns

            //Vertical pixel-poles are created:
            float columnsZ = Camera.main.orthographicSize - (rows / 2.0f * distanceBetweenSqaures); //z position of the pixel-poles
            GameObject verticalPixelPole = GameObject.CreatePrimitive(PrimitiveType.Cube);
            verticalPixelPole.transform.localScale = new Vector3(0.05f, 0.05f, rows * distanceBetweenSqaures);
            verticalPixelPole.GetComponent<MeshRenderer>().material.color = Color.black;

            for (int i = 1; i <= columns * pixelRowsPerSquare; i += 2)
            {
                Instantiate(verticalPixelPole, new Vector3(startXCoordinate + i * distance, .5f, columnsZ), Quaternion.identity);
            }

            //Horizontal pixel-poles are created:
            float rowsZOrigin = Camera.main.orthographicSize; //start z position of the horizontal pixel-poles
            GameObject horizontalPixelPole = GameObject.CreatePrimitive(PrimitiveType.Cube);
            horizontalPixelPole.transform.localScale = new Vector3(Camera.main.orthographicSize*Camera.main.aspect*2, 0.05f, 0.05f);
            horizontalPixelPole.GetComponent<MeshRenderer>().material.color = Color.black;

            for(int i = 1; i <= rows * pixelRowsPerSquare; i +=2)
            {
                Instantiate(horizontalPixelPole, new Vector3(0, .5f, rowsZOrigin - i * distance), Quaternion.identity);
            }
        }
    }

    ///// <summary>
    ///// Vertical margins are created for the gaming area.
    ///// They are not required for the mobile version of the game.
    ///// </summary>
    ///// <param name="absolutePostionZ">the z position of the vertical margins as a positive number</param>
    //private void CreateMargins(float absolutePositionZ)
    //{
    //    GameObject leftMargin, rightMargin;

    //    leftMargin = Instantiate(gridVerticalPole, new Vector3(-(absolutePositionZ + 2), 3.0f, 0.0f), Quaternion.identity);;
    //    leftMargin.transform.localScale = new Vector3(4.0f, leftMargin.transform.localScale.y, leftMargin.transform.localScale.z);
    //    rightMargin = Instantiate(gridVerticalPole, new Vector3(absolutePositionZ + 2, 3.0f, 0.0f), Quaternion.identity);;
    //    rightMargin.transform.localScale = new Vector3(4.0f, leftMargin.transform.localScale.y, leftMargin.transform.localScale.z);
    //}

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
