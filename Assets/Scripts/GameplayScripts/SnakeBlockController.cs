using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBlockController : MonoBehaviour
{
    ///To refer to the position of a 2 int values are used.
    ///The first int always stands for a row (starting with row 1), the second int stands for a column (starting with column 1)
    ///The field of intersection is the current position.
    ///By using the provided methods the field as two ints (int a,b]) can be converted into an actual position in coordinates
    ///Note: the leftmost column is column zero, the topmost row is row 0
    /// => the rightmost column is the column with the highest int value, the bottommost row is the row with the highest int value
    private GameObject gameModeManager; //references the gameModeManager
    int previousPositionRow, previousPositionColumn; //determines the previous position of the block (row and column)
    int currentPositionRow, currentPositionColumn; //determines the current position of the block (row and column)
    bool positionChanged; //is true if the position of the block was changed at least once so far (if previousPositionColumn and previousPositionRow exist)
    Color snakeColor, collectablesColor;

    public GameObject Successor
        { get; set; }

    private void Awake()
    {
        //DataSaver.Instance.SetSnakeColor(23, 244, 244, 255);
        gameModeManager = GameObject.FindGameObjectWithTag("GameModeManager");
        currentPositionColumn = 0;
        currentPositionRow = 0;
        Successor = null;
        positionChanged = false; //shows that there is no previous column position yet
        SetBlockColor();
    }

    //public void Start()
    //{
    //    SetBlockColor();
    //}

    /// <summary>
    /// Sets the color of the blocks. The color is retrieved from an external file ('snakeColor').
    /// </summary>
    public void SetBlockColor()
    {
        int[] c = DataSaver.Instance.GetSnakeColor();
        Color thisColor = new Color(c[0]/255f, c[1]/255f, c[2]/255f, c[3]/255f);
        gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", thisColor);
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", thisColor);
    }

    /// <summary>
    /// If the block has any children, they're colored. The color is retrieved from an external file ('snakeColor').
    /// </summary>
    //public void SetColorChildren()
    //{
    //    foreach(Transform t in gameObject.transform)
    //    {
    //        t.gameObject.AddComponent<SetSnakeColor>();
    //    }
    //}

    ///// <summary>
    ///// Moves the successor of a block and returns true once all of the blocks have moved.
    ///// </summary>
    //public bool MoveSuccessor()
    //{
    //    // print("current row" + GetCurrentRow());
    //    // print("previous row" + GetPreviousRow());

    //    if (Successor != null)
    //    {
    //        Successor.GetComponent<SnakeBlockController>().SetPosition(GetComponent<SnakeBlockController>().GetPreviousRow(), GetComponent<SnakeBlockController>().GetPreviousColumn());
    //        return Successor.GetComponent<SnakeBlockController>().MoveSuccessor();
    //    }
    //    else
    //        return true;
            
    //}

    /// <summary>
    /// Converts two int values which describe a position of a block into an actual position in coordinates.
    /// The method returns the position of the field in world coordinates.
    /// </summary>
    /// <param name="row">int that determines the row of the object</param>
    /// <param name="column">int that determines the column of the object</param>
    /// <returns>Returns the position of the field as a Vector3</returns>
    public Vector3 ConvertIntsIntoPosition(int row, int column)
    {
        float minPositionZ = gameModeManager.GetComponent<CreateWorld>().GetUpperMarginOfScreen();  
        float minPositionX = gameModeManager.GetComponent<CreateWorld>().GetLeftMarginOfScreen(); 
        float fieldWidth = gameModeManager.GetComponent<CreateWorld>().GetSquareLength();

        return new Vector3(minPositionX + (column - .5f) * fieldWidth, .2f, minPositionZ - (row - .5f) * fieldWidth);
    }

    /// <summary>
    /// Returns the previous row position of the block.
    /// </summary>
    /// <returns>Returns the previous row as a float</returns>
    public int GetPreviousRow()
    {
        return previousPositionRow;
    }

    /// <summary>
    /// Returns the current row position of the block.
    /// </summary>
    /// <returns>Returns the current row as a float</returns>
    public int GetCurrentRow()
    {
        return currentPositionRow;
    }

    /// <summary>
    /// Returns the previous position of the block.
    /// </summary>
    /// <returns>Returns the previous position as a Vector3</returns>
    public Vector3 GetPreviousPosition()
    {
        return new Vector3(previousPositionColumn, .2f, previousPositionRow);
    }

    /// <summary>
    /// Returns the previous column position of the block.
    /// </summary>
    /// <returns>Returns the previous column as a float</returns>
    public int GetPreviousColumn()
    {
        return previousPositionColumn;
    }

    /// <summary>
    /// Returns the current column position of the block.
    /// </summary>
    /// <returns>Returns the current column as a float</returns>
    public int GetCurrentColumn()
    {
        return currentPositionColumn;
    }

    /// <summary>
    /// Sets the current column position of the block.
    /// </summary>
    /// <param name="newColumn">the new column as int to pass</param>
    public void SetCurrentColumn(int newColumn)
    {
        currentPositionColumn = newColumn;
    }

    /// <summary>
    /// Sets the current row position of the block.
    /// </summary>
    /// <param name="newRow">the new column as int to pass</param>
    public void SetCurrentRow(int newRow)
    {
        currentPositionRow = newRow;
    }

    /// <summary>
    /// The position and previous position of a block is set (as column and row).
    /// The position of the block in world coordinates is also reset. 
    /// </summary>
    /// <param name="row">int that determines the new row position of the block</param>
    /// <param name="column">int that determines the new column position of the block</param>
    public void SetPositionAndPreviousPosition(int row, int column)
    {
        if(positionChanged) //there is no current position yet at the beginning, so the previous position is only assigned from the second call of this method on
        {
            previousPositionRow = currentPositionRow;
            previousPositionColumn = currentPositionColumn;
        }
        else
        {
            positionChanged = true;
        }
        
        currentPositionRow = row;
        currentPositionColumn = column;
        transform.position = ConvertIntsIntoPosition(currentPositionRow, currentPositionColumn);
    }

    /// <summary>
    /// The position of a block in the world scene is set.
    /// The position of the block in world coordinates is also reset. 
    /// </summary>
    /// <param name="row">int that determines the new row position of the block</param>
    /// <param name="column">int that determines the new column position of the block</param>
    public void SetPositionInWorld(int row, int column)
    {
        transform.position = ConvertIntsIntoPosition(currentPositionRow, currentPositionColumn);
    }

    /// <summary>
    /// The position and previous position of a block in world coordinates is set. The actual in-world position isn't altered.
    /// The method is recursively called for all other blocks of the list.
    /// </summary>
    /// <param name="row">int that determines the new row position of the block</param>
    /// <param name="column">int that determines the new column position of the block</param>
    public void SetPositionInCoordinates(int row, int column)
    {
        currentPositionRow = row;
        currentPositionColumn = column;

        if (positionChanged) //there is no current position yet at the beginning, so the previous position is only assigned from the second call of this method on
        {
            previousPositionRow = currentPositionRow;
            previousPositionColumn = currentPositionColumn;
        }
        else
        {
            positionChanged = true;
        }

        if (Successor != null)
            Successor.GetComponent<SnakeBlockController>().SetPositionInCoordinates(previousPositionRow, previousPositionColumn);
    }

    /// <summary>
    /// Sets the second last block as the new end of the snake. Returns the previous end. (This method is needed for moving the snake, 
    /// it should only be called by the snake head.)
    /// </summary>
    /// <returns>Returns the current last block of the snake (which won't be the end of the snake any more after the execution of the method).</returns>
    public GameObject SetNewLastBlock()
    {
        GameObject successorOfSuccessor = Successor.GetComponent<SnakeBlockController>().Successor;
        if (successorOfSuccessor == null)
        {
            GameObject currentSuccessor = Successor;
            Successor = null;
            return currentSuccessor;
        }
        else
        {
            return Successor.GetComponent<SnakeBlockController>().SetNewLastBlock();
        }
    }

    /// <summary>
    /// The different components of the snake are recursively set inactive.
    /// At first the method needs to be called by the head of the snake. 
    /// </summary>
    public void SetSnakeInactive()
    {
        if(Successor != null)
        {
            Successor.GetComponent<SnakeBlockController>().SetSnakeInactive();
        }
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Returns the current amount of blocks (of the snake)
    /// </summary>
    /// <returns>Returns the current amount of blocks as an integer</returns>
    public int CountCurrentBlocks()
    {
        if(Successor != null)
        {
            return Successor.GetComponent<SnakeBlockController>().CountCurrentBlocks() + 1;
        }
        else
        {
            return 1;
        }
        
    }

    /// <summary>
    /// Returns true if the snake length is greater than 2. When the method is called for the snake head the passed int 
    /// MUST BE 1 and the method needs to be called by the snake head!
    /// </summary>
    /// <param name="currentLength">The current measured length.</param>
    /// <returns></returns>
    public bool IsSnakeLengthGreaterThan2(int currentLength)
    {
        if (currentLength > 2)
            return true;

        if (Successor != null)
            return Successor.GetComponent<SnakeBlockController>().IsSnakeLengthGreaterThan2(currentLength + 1);
        else
            return false;
    }

    //methods that are needed for spawning a new collectible

    /// <summary>
    /// Determines all of the fields that are currently occupied by blocks of the snakes and allocates true to the corresponding array-field
    /// of 'occupiedFields'. Note: works recursively, needs to be called for the snake's head at first to function properly
    /// </summary>
    /// <param name="occupiedFields">The 2-D array holding the occupied fields.</param>
    /// <returns></returns>
    public bool[,] DetermineOccupiedFields(bool[,] occupiedFields)
    {
        occupiedFields[currentPositionRow - 1, currentPositionColumn - 1] = true;
        return Successor != null ? Successor.GetComponent<SnakeBlockController>().DetermineOccupiedFields(occupiedFields) : occupiedFields;
    }

    /// <summary>
    /// This coroutine spawns a new collectable which should be spawned at the current position of the head of the snake. 
    /// The collectable is spawned once the snake made a movement (i.e. the collectable won't be spawned 'in/under' the head of the snake any more)
    /// It won't immidiately be possible to collect the newly created collectable, the spawning is delayed.
    /// The new collectable object is also pixeled if pixelsMode is on.
    /// </summary>
    /// <param name="collectablesPrefab">The prefab which should be instantiated for creating a new collectable.</param>
    /// <param name="position">The position in as Vector3 (where the snake should be spawned).</param>
    public IEnumerator CreateNewCollectableDelayed(GameObject collectablesPrefab, Vector3 position)
    {
        int rowsIndex = currentPositionRow;
        int columnsIndex = currentPositionColumn;
        while (currentPositionColumn == columnsIndex && currentPositionRow == rowsIndex)
            yield return null;
        GameObject collectible = Instantiate(collectablesPrefab, position, Quaternion.identity);
        collectible.SetActive(true);
        //GameObject.FindGameObjectWithTag("SnakeHead").GetComponent<SnakeHeadController>().PixelCollectable();
    }
}
/// <summary>
    /// Returns 1 if passed true or -1 if passed false.
    /// </summary>
    /// <param name="myBool">Parameter value to pass.</param>
    /// <returns>Returns an integer based on the passed value.</returns>
