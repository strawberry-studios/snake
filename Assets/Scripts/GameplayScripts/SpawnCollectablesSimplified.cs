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

    /// <summary>
    /// Instead of Start(), this script uses StartUp, it is called by the 'SnakeHeadController', after the snake head was positioned, to make sure
    /// that the order of events is correct.
    /// </summary>
    public void Start()
    {
        Rows = GetComponent<CreateWorld>().GetRows();
        Columns = GetComponent<CreateWorld>().GetColumns();
        squares = GetSquares();
        CreateNewCollectable();
    }

    /// <summary>
    /// The first collectable is created. It isn't created under the snake, even if 'delayedSpawnings' is toggled on.
    /// </summary>
    public void CreateFirstCollectable()
    {
        bool[,] currentlyOccupiedFields = new bool[Rows, Columns];
        for(int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
                currentlyOccupiedFields[r, c] = false;
        }
        currentlyOccupiedFields[StaticValues.PlayerStartX, StaticValues.PlayerStartY] = true;

        int freeSquares = squares - 1;

        //one of the free fields is chosen randomly and a new collectible is spawned there:
        InstantiateCollectable(currentlyOccupiedFields, freeSquares);
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
            InstantiateCollectable(currentlyOccupiedFields, freeSquares);
        }
    }

    /// <summary>
    /// Chooses any position randomly at which a new collectable will be spawned. The actual spawning takes place once the square is free (no longer
    /// occupied by one of the snake blocks).
    /// </summary>
    public void CreateNewCollectableDelayed()
    {
        int snakeHeadColumn = snakeHead.GetComponent<SnakeBlockController>().GetCurrentColumn() - 1;
        int snakeHeadRow = snakeHead.GetComponent<SnakeBlockController>().GetCurrentRow() - 1;

        bool[,] currentlyOccupiedFields = new bool[Rows, Columns];
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
                currentlyOccupiedFields[r, c] = false;
        }
        currentlyOccupiedFields[snakeHeadRow, snakeHeadColumn] = true;

        int freeSquares = squares - snakeHead.GetComponent<SnakeBlockController>().CountCurrentBlocks();

        if (freeSquares == 0)
        {
            snakeHead.GetComponent<SnakeHeadController>().Lose(true);
        }
        else
        {
            InstantiateCollectable(currentlyOccupiedFields, freeSquares);
        }
    }

    /// <summary>
    /// Instantiates a new collectable at a randomly chosen position. The passed 2D-bool holds the squares where it can be spawned.
    /// </summary>
    /// <param name="freeSquares">The number of free squares in the world (where a new snake block can be spawned)</param>
    /// <param name="currentlyOccupiedFields">A 2D-array that holds the information whether a square of the world is occupied or not (for 
    /// each square of the world). The first index stands for the row, the second one for the column of the square.</param>
    void InstantiateCollectable(bool[,] currentlyOccupiedFields, int freeSquares)
    {
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

    ///// <summary>
    ///// Chooses any position randomly at which a new collectable will be spawned. The actual spawning takes place once the square is free (no longer
    ///// occupied by one of the snake blocks).
    ///// </summary>
    //public void CreateNewCollectableDelayed()
    //{
    //    int rowsIndex = (int)Random.Range(0, Rows - 0.001f) + 1;  
    //    int columnsIndex = (int)Random.Range(0, Columns - 0.001f) + 1;
    //    Vector3 position = snakeHead.GetComponent<SnakeBlockController>().ConvertIntsIntoPosition(rowsIndex, columnsIndex);

    //    //spawn the new collectible if it doesn't have the same position as the snake head, if it has the same position the spawning should
    //    //be delayed until the snake moved (block can also spawn under the snake, yet can only be collected by its head:
    //    int snakeHeadColumn = snakeHead.GetComponent<SnakeBlockController>().GetCurrentColumn();
    //    int snakeHeadRow = snakeHead.GetComponent<SnakeBlockController>().GetCurrentRow();
    //    if (snakeHeadColumn == columnsIndex && snakeHeadRow == rowsIndex)
    //    {
    //        //Instantiate the new collectable once the snakeHead moved:
    //        StartCoroutine(snakeHead.GetComponent<SnakeBlockController>().CreateNewCollectableDelayed(collectablesPrefab, position));
    //    }
    //    else
    //    {
    //        //Instantiate the new collectable immidiately:
    //        GameObject collectable = Instantiate(collectablesPrefab, position, Quaternion.identity);
    //        collectable.SetActive(true);
    //    }
    //}

    /// <summary>
    /// Returns the number of squares/fields of the world.
    /// </summary>
    int GetSquares()
    {
        return Columns * Rows;
    }
}