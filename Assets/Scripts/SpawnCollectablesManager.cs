using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCollectablesManager : MonoBehaviour
{
    private int rows, columns, squares;
    private int[] currentColumn, currentRow;
    private int[] freeColumns, freeRows;
    public GameObject snakeHead;
    public GameObject collectablePrefab;

    // Start is called before the first frame update
    void Start()
    {
        rows = GetComponent<CreateWorld>().GetRows();
        columns = GetComponent<CreateWorld>().GetColumns();
        squares = GetSqaures();
        CreateNewCollectable();
    }

    /// <summary>
    /// A new collectable is created.
    /// Its position is random and never overlaps with a square which is occupied by the snake. 
    /// </summary>
    public void CreateNewCollectable()
    {
        if(GetNumberOfFreeSqaures() != 0)
        {
            CreatePositionHolders();
            CreateListOfFreeSqaures();
            int index = Mathf.RoundToInt(Random.Range(.5f, GetNumberOfFreeSqaures() + .5f));
            Vector3 position = snakeHead.GetComponent<SnakeBlockController>().ConvertIntsIntoPosition(freeRows[index], freeColumns[index]);
            Instantiate(collectablePrefab, position, Quaternion.identity);
        }
        else
        {
            snakeHead.GetComponent<SnakeHeadController>().Lose();
        }
    }

    /// <summary>
    /// Creates two arrays of ints which hold the current column and row positions of the squares which aren´t occupied by the snake.
    /// One instance of the column array and the instance of the row array with the same index refer to one unoccupied position.
    /// </summary>
    private void CreateListOfFreeSqaures()
    {
        int squares = GetSqaures();
        int firstIndexCounter = 0;
        int indexCounter = 0;

        freeColumns = new int[squares];
        freeRows = new int[squares];

        for(int r = 1; r <= columns; r += 1)
        {
            for(int z = 1; z <= rows; z += 1)
            {
                freeColumns[firstIndexCounter] = r;
                freeRows[firstIndexCounter] = z;
                firstIndexCounter ++;
            }
        }

        for(int i = 1; i <= columns; i += 1)
        {
            for(int k = 1; k <= rows; k += 1)
            {
                if(!IsFieldOccupied(k, i))
                {
                    freeColumns[indexCounter] = i;
                    freeRows[indexCounter] = k;
                    indexCounter ++;
                }
            }
        }
    }

    /// <summary>
    /// Returns true, if the square with the passed row and column is occupied by a block of the snake.
    /// </summary>
    /// <param name="thisRow">row of the square as int to pass.</param>
    /// <param name="thisColumn">column of the square as int to pass.</param>
    /// <returns>Returns whether the field is occupied as bool</returns>
    private bool IsFieldOccupied(int thisRow, int thisColumn)
    {
        bool isOccupiedStatus = false;
        for(int i = 0; i < GetCurrentBlocks(); i += 1)
        {
            if(currentColumn[i] == thisColumn && currentRow[i] == thisRow)
            {
                isOccupiedStatus = true;
            }
        }
        return isOccupiedStatus;
    }

    /// <summary>
    /// Creates two arrays of ints which hold the current column and row positions of the blocks of the snake.
    /// One instance of the column array and the instance of the row array with the same index refer to one occupied position.
    /// </summary>
    private void CreatePositionHolders()
    {
        int occupiedSquares = GetCurrentBlocks();
        currentColumn = new int[occupiedSquares];
        currentRow = new int[occupiedSquares];
        snakeHead.GetComponent<SnakeBlockController>().CreatePositionHolders(0);
    }

    /// <summary>
    /// Returns the number of squares of the gaming area.
    /// </summary>
    /// <returns>Returns the number of squares as integer</returns>
    private int GetSqaures()
    {
        return rows*columns;
    }

    /// <summary>
    /// Returns the number of free squares of the gaming area.
    /// Free means that there is no block on that field.
    /// </summary>
    /// <returns>Returns the number of free squares as integer</returns>
    private int GetNumberOfFreeSqaures()
    {
        return squares - GetCurrentBlocks();
    }

    private void hey()
    {
        for(int i = 1; i <= rows; i += 1)
        {
            for(int k = 1; k <= columns; k += 1)
            {

            }
        }
        ///return 1;
    }

    /// <summary>
    /// Returns the number of blocks of the snake.
    /// </summary>
    /// <returns>Returns the number of blocks as an integer</returns>
    private int GetCurrentBlocks()
    {
        return snakeHead.GetComponent<SnakeBlockController>().CountCurrentBlocks();
    }

    /// <summary>
    /// Sets the column position of an occupied square (square with a block on it)
    /// </summary>
    /// <param name="index">index of the block (its position within the snake starting with zero) as integer to pass.</param>
    /// <param name="newColumnPosition">column position as integer of the object to pass</param>
    public void SetCurrentOccupiedColumn(int index, int newColumnPosition)
    {
        currentColumn[index] = newColumnPosition;
    }

    // public int gC(int index)
    // {
    //     return currentColumn[index];
    // }
    // public int gR(int index)
    // {
    //     return currentRow[index];
    // }

    /// <summary>
    /// Sets the row position of an occupied square (square with a block on it)
    /// </summary>
    /// <param name="index">index of the block (its position within the snake starting with zero) as integer to pass.</param>
    /// <param name="newRowPosition">row position as integer of the object to pass</param>
    public void SetCurrentOccupiedRow(int index, int newRowPosition)
    {
        currentRow[index] = newRowPosition;
    }
}

