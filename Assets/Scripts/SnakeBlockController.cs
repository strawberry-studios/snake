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
    public GameObject successor;

    private void Awake()
    {
        gameModeManager = GameObject.FindGameObjectWithTag("GameModeManager");
        currentPositionColumn = 0;
        currentPositionRow = 0;
        successor = null;
    } 

    // Start is called before the first frame update
    void Start()
    {
        positionChanged = false; //shows that there is no previous column position yet
    }

    public void MoveSuccessor()
    {
        // print("current row" + GetCurrentRow());
        // print("previous row" + GetPreviousRow());

        if(successor != null)
        {
            successor.GetComponent<SnakeBlockController>().SetPosition(GetComponent<SnakeBlockController>().GetPreviousRow(), GetComponent<SnakeBlockController>().GetPreviousColumn());
            successor.GetComponent<SnakeBlockController>().MoveSuccessor();
        }
    }

    /// <summary>
    /// The successor of the block is returned
    /// </summary>
    /// <returns>Returns the successor of the block as a GameObject</returns>
    public GameObject GetSuccessor()
    {
        return successor;
    }

    /// <summary>
    /// The successor of the block is set
    /// </summary>
    /// <param name="block">GameObject to be passed as successor</param>
    public void SetSuccessor(GameObject block)
    {
        successor = block;
    }

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
        currentPositionColumn = newColumn;;
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
    public void SetPosition(int row, int column)
    {
        if(positionChanged)
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
    /// The different components of the snake are recursively set inactive.
    /// At first the method needs to be called by the head of the snake. 
    /// </summary>
    public void SetSnakeInactive()
    {
        if(successor != null)
        {
            successor.GetComponent<SnakeBlockController>().SetSnakeInactive();
        }
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Returns the current amount of blocks (of the snake)
    /// </summary>
    /// <returns>Returns the current amount of blocks as an integer</returns>
    public int CountCurrentBlocks()
    {
        if(successor != null)
        {
            return successor.GetComponent<SnakeBlockController>().CountCurrentBlocks() + 1;
        }
        else
        {
            return 1;
        }
        
    }

    public void CreatePositionHolders(int index)
    {
        gameModeManager.GetComponent<SpawnCollectablesManager>().SetCurrentOccupiedColumn(index, currentPositionColumn);
        gameModeManager.GetComponent<SpawnCollectablesManager>().SetCurrentOccupiedRow(index, currentPositionRow);
        // print(gameModeManager.GetComponent<SpawnCollectablesManager>().gC(index));
        // print(gameModeManager.GetComponent<SpawnCollectablesManager>().gR(index));

        if(successor != null)
        {
            successor.GetComponent<SnakeBlockController>().CreatePositionHolders(index + 1);
        }
    }
}
/// <summary>
    /// Returns 1 if passed true or -1 if passed false.
    /// </summary>
    /// <param name="myBool">Parameter value to pass.</param>
    /// <returns>Returns an integer based on the passed value.</returns>
