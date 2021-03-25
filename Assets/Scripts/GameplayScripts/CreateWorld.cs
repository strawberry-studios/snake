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
        spawner.ImplementFields(this);
        spawner.SetUpPixelMode(this);
    }
    //if (DataSaver.Instance.GetShowPixels())
    //{
    //int pixelRowsPerSquare; //number of rows of pixels per square which simulate the pixel-objects
    //float startXCoordinate = -Camera.main.orthographicSize * Camera.main.aspect; //start x coordinate
    //pixelRowsPerSquare = 2 * ((int)(distanceBetweenSqaures / 0.1f)); //number should be even
    //float distance = distanceBetweenSqaures / pixelRowsPerSquare; //the distance between 2 pixel columns

    ////Vertical pixel-poles are created:
    //float columnsZ = Camera.main.orthographicSize - (rows / 2.0f * distanceBetweenSqaures); //z position of the pixel-poles
    //GameObject verticalPixelPole = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //verticalPixelPole.transform.localScale = new Vector3(0.05f, 0.05f, rows * distanceBetweenSqaures);
    //verticalPixelPole.GetComponent<MeshRenderer>().material.color = Color.black;

    //for (int i = 1; i <= columns * pixelRowsPerSquare; i += 2)
    //{
    //    Instantiate(verticalPixelPole, new Vector3(startXCoordinate + i * distance, .5f, columnsZ), Quaternion.identity);
    //}

    ////Horizontal pixel-poles are created:
    //float rowsZOrigin = Camera.main.orthographicSize; //start z position of the horizontal pixel-poles
    //GameObject horizontalPixelPole = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //horizontalPixelPole.transform.localScale = new Vector3(Camera.main.orthographicSize*Camera.main.aspect*2, 0.05f, 0.05f);
    //horizontalPixelPole.GetComponent<MeshRenderer>().material.color = Color.black;

    //for(int i = 1; i <= rows * pixelRowsPerSquare; i +=2)
    //{
    //    Instantiate(horizontalPixelPole, new Vector3(0, .5f, rowsZOrigin - i * distance), Quaternion.identity);
    //}
    //}

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

/// <summary>
/// This class provides methods for spawning pixeled blocks. None of these methods are executed on their won, 
/// they're only called by other scripts. The snake blocks and apples are not really pixeled, they only seem to be, as all of the objects created 
/// by this class form a grid which makes the blocks appear as pixeled.
/// </summary>
public class SpawnPixeledBlocks
{
    /// <summary>
    /// The size of the pixels, is approximately equal to the thickness of gridLines, if activated
    /// </summary>
    float pixelSize;
    /// <summary>
    /// The diameter of the grid lines, is approximately equal to the size of pixels
    /// </summary>
    float gridLinesDiameter;
    /// <summary>
    /// The length of a square of the world matrix (minus the thickness of the gridLines, if activated)
    /// </summary>
    float length;
    /// <summary>
    /// The number of pixels which one snake block will consist of.
    /// </summary>
    int pixelsNumber;
    /// <summary>
    /// Whether the grid lines are activated or not.
    /// </summary>
    bool gridLinesOn;
    /// <summary>
    /// The number of columns.
    /// </summary>
    int columns;
    /// <summary>
    /// The number of rows.
    /// </summary>
    int rows;
    /// <summary>
    /// The center of the screen on the z-axis
    /// </summary>
    float zOrigin;
    /// <summary>
    /// A pole (cuboid) which is vertical and is needed for creating the illusion of a pixel mode.
    /// </summary>
    GameObject verticalPixelPole;
    /// <summary>
    /// A pole (cuboid) which is horizontal and is needed for creating the illusion of a pixel mode.
    /// </summary>
    GameObject horizontalPixelPole;
    /// <summary>
    /// The parent of all pixel-mode creating objects
    /// </summary>
    GameObject verticalPixelModeObjects, horizontalPixelModeObjects;
    /// <summary>
    /// Left margin of the screen.
    /// </summary>
    float minXPosition;
    /// <summary>
    /// Upper margin of the screen.
    /// </summary>
    float maxZPosition;

    /// <summary>
    /// Functions like a start method of a MonoBehaviour, but needs to be called by another method to be exectuted.
    /// Implements all of the fields of this class.
    /// </summary>
    /// <param name="cw">The current 'CreateWorld' script.</param>
    public void ImplementFields(CreateWorld cW)
    {
        gridLinesOn = cW.GridLinesOn;

        GameObject pixelModeObjects = cW.PixelModeObjects;
        horizontalPixelModeObjects = pixelModeObjects.GetOrAddEmptyGameObject("horizontalPixelModeObjects");
        verticalPixelModeObjects = pixelModeObjects.GetOrAddEmptyGameObject("verticalPixelModeObjects");

        columns = cW.GetColumns();
        rows = cW.GetRows();
        zOrigin = cW.ZOrigin;

        gridLinesDiameter = cW.gridHorizontalPole.transform.lossyScale.x * cW.GridLinesFactor;

        if (gridLinesOn)
        {
            length = cW.GetSquareLength() - gridLinesDiameter;
            pixelsNumber = UnityEngineExtensions.RoundToUnevenNumber(length / gridLinesDiameter);
        }
        else
        {
            length = cW.GetSquareLength();
            pixelsNumber = UnityEngineExtensions.RoundToEvenNumber(length / gridLinesDiameter);
        }

        pixelSize = length / pixelsNumber;

        Camera c = Camera.main;
        minXPosition = c.orthographicSize * c.aspect; //left margin of the screen
        maxZPosition = c.orthographicSize; //upper margin of the 

        Material backgroundColor = Resources.Load("Background") as Material;

        horizontalPixelPole = GameObject.CreatePrimitive(PrimitiveType.Cube);
        horizontalPixelPole.GetComponent<Renderer>().material = backgroundColor;

        verticalPixelPole = GameObject.CreatePrimitive(PrimitiveType.Cube);
        verticalPixelPole.GetComponent<Renderer>().material = backgroundColor;

        horizontalPixelPole.transform.localScale = new Vector3(minXPosition*2, .2f, pixelSize);
        verticalPixelPole.transform.localScale = new Vector3(pixelSize, .2f, 2*(maxZPosition-zOrigin));
    }

    //methods for creating the pixeled objects if gridLines and pixelMode are on:

    /// <summary>
    /// Sets up the pixel mode by creating a grid of black cuboids which make the snake and apples appear as pixeled.
    /// </summary>
    /// <param name="cw">The current 'CreateWorld' script.</param>
    public void SetUpPixelMode(CreateWorld cW)
    {
        if (gridLinesOn)
        {
            float distance; //used for storing temporary data:
            float squareLength = cW.GetSquareLength();
            distance = -(squareLength * columns/2) + pixelSize/2 + gridLinesDiameter/2;

            for (int c = 0; c < columns; c++)
            { 
                for (int x = 0; x < pixelsNumber; x += 2)
                {
                    GameObject vPixelMode = GameObject.Instantiate(verticalPixelPole) as GameObject;
                    vPixelMode.transform.SetParent(verticalPixelModeObjects.transform);
                    vPixelMode.transform.localPosition = new Vector3(distance + x * pixelSize, 1, zOrigin);
                }
                distance += squareLength;
            }

            distance = (squareLength * rows/2 + zOrigin) - pixelSize / 2 - gridLinesDiameter / 2;
            for (int r = 0; r < rows; r++)
            {
                for (int z = 0; z < pixelsNumber; z += 2)
                {
                    GameObject hPixelMode = GameObject.Instantiate(horizontalPixelPole) as GameObject;
                    hPixelMode.transform.SetParent(horizontalPixelModeObjects.transform);
                    hPixelMode.transform.localPosition = new Vector3(0, 1, distance - z * pixelSize);
                }
                distance -= squareLength;
            }
        }
        else
        {
            int verticalPixelObjects = pixelsNumber * columns;
            int horizontalPixelObjects = pixelsNumber * rows;

            for(int x = 0; x <= verticalPixelObjects; x += 2)
            {
                GameObject vPixelMode = GameObject.Instantiate(verticalPixelPole) as GameObject;
                vPixelMode.transform.SetParent(verticalPixelModeObjects.transform);
                vPixelMode.transform.localPosition = new Vector3(-minXPosition + x * pixelSize, 1, zOrigin);
            }

            for (int z = 0; z <= horizontalPixelObjects; z += 2)
            {
                GameObject hPixelMode = GameObject.Instantiate(horizontalPixelPole) as GameObject;
                hPixelMode.transform.SetParent(horizontalPixelModeObjects.transform);
                hPixelMode.transform.localPosition = new Vector3(0, 1, maxZPosition - z * pixelSize);
            }
        }
    }
}
