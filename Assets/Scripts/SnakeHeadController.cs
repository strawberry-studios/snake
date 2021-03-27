using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeHeadController : MonoBehaviour
{
    public GameObject gameModeManager;
    public GameObject snakeBodyPrefab;
    private GameObject end; //references the end of the snake

    public GameObject directionManager; //references the directionManager
    public enum DIRECTION {up, down, left, right, none};
    private DIRECTION direction;
    private int framesUntilMove; //Determines after how many frames a movement should occur, the speed is set by the player in the settings scene
    private float timeCount; //The frames since the last movement (at the beginning it is as big as framesUntilMove)

    // Start is called before the first frame update
    void Start()
    {
        SetSize();
        end = gameObject;
        GetComponent<SnakeBlockController>().SetPosition(3, 3);
        SetFramesUntilMove();
        timeCount = framesUntilMove * 0.015f;
        direction = DIRECTION.none;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    /// <summary>
    /// If the snake´s head touches a collectable object, the snake grows bigger by one block.
    /// If it touches anoother block of the snake the player loses.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Collectable"))
        {
            Destroy(other.gameObject);
            CreateNewBlock();
            gameModeManager.GetComponent<SpawnCollectablesManager>().CreateNewCollectable();
        }
        if(other.gameObject.CompareTag("SnakeBlock"))
        {
            Lose();
        }
    }

    /// <summary>
    /// The snake´s head is moved into a direction which is determined by the enum direction.
    /// The players actions (pressing the move buttons) set a new state of the direction variable, yet the player can never move 
    /// the opposite direction of their current direction.
    /// The speed set in the settings determines after how many frames a new move should be executed.
    /// If no button was pressed yet (i.e. the player just started the game) there is no movement.
    /// Note: If screen margins as boundaries is true the player loses if the snake´s head touches one of the screens margins,
    /// elsewise it will reappear in the same column/row of the gaming area on the other side.
    /// IMPORTANT: The position of all other blocks of the snake is adjusted recursively. The Move() function is called for the snake´s head´s successor
    /// </summary>
    private void Move()
    {
        int currentRow = GetComponent<SnakeBlockController>().GetCurrentRow();
        int currentColumn = GetComponent<SnakeBlockController>().GetCurrentColumn();

        int maximalRow = gameModeManager.GetComponent<CreateWorld>().GetRows();
        int maximalColumn = gameModeManager.GetComponent<CreateWorld>().GetColumns();

        bool noWorldBoundary;
        if(DataSaver.Instance.GetWorldBoundariesState())
        {
            noWorldBoundary = false;
        }
        else
        {
            noWorldBoundary = true;
        }

        if(timeCount >= framesUntilMove * 0.015)
        {
            //print(Time.deltaTime * framesCount);
            //print(framesCount);
            if(direction == DIRECTION.up)
            { 
                if(currentRow - 1 < 1)
                {
                    if(noWorldBoundary)
                    {
                        currentRow = maximalRow + 1;
                    }
                    else
                    {
                        Lose();
                    }
                }
                GetComponent<SnakeBlockController>().SetPosition(currentRow - 1, currentColumn);
            }
            else if(direction == DIRECTION.down)
            {
                if(currentRow + 1 > maximalRow)
                {
                    if(noWorldBoundary)
                    {
                        currentRow = 0;
                    }
                    else
                    {
                        Lose();
                    }
                }
                GetComponent<SnakeBlockController>().SetPosition(currentRow + 1, currentColumn);
            }
            else if(direction == DIRECTION.right)
            {
                if(currentColumn + 1 > maximalColumn)
                {
                    if(noWorldBoundary)
                    {
                        currentColumn = 0;
                    }
                    else
                    {
                        Lose();
                    }
                }
                GetComponent<SnakeBlockController>().SetPosition(currentRow, currentColumn + 1);
            }
            else if(direction == DIRECTION.left)
            {
                if(currentColumn - 1 < 1)
                {
                    if(noWorldBoundary)
                    {
                        currentColumn = maximalColumn + 1;
                    }
                    else
                    {
                        Lose();
                    }
                }
                GetComponent<SnakeBlockController>().SetPosition(currentRow, currentColumn - 1);
            }
            timeCount = 0.0f;
            directionManager.GetComponent<SetDirectionManager>().SetDirectionChanged(false); 
            //the direction in the SetDirectionManager can be changed again

            GetComponent<SnakeBlockController>().MoveSuccessor();
        }
        if(timeCount < framesUntilMove * 0.015 && Time.timeScale != 0.0f)
        { 
            timeCount += Time.deltaTime;
        }
    }

    /// <summary>
    /// If the snake touches itself again the game is over. If the worldBoundaries toggle in the settings scene is toggled on
    /// the player also loses a life if he leaves the gaming area.
    /// </summary>
    public void Lose()
    {
        GetComponent<SnakeBlockController>().SetSnakeInactive();
        gameModeManager.GetComponent<GameOverManager>().GameOver();
    }

    /// <summary>
    /// The frames which need to pass until the snake moves are set.
    /// The value depends on the speed PlayerPref which is set by the player int settings scene.
    /// </summary>
    private void SetFramesUntilMove()
    {
        framesUntilMove = Mathf.RoundToInt(100 / DataSaver.Instance.GetPlayerSpeed());
    }

    /// <summary>
    /// Another block is created.
    /// It is added to the snake at its end.
    /// </summary>
    private void CreateNewBlock()
    {
        GameObject snakeBody;
        int previousEndColumn, previousEndRow; //the previous row and column (positions) of the end of the snake
        previousEndRow = end.GetComponent<SnakeBlockController>().GetPreviousRow();
        previousEndColumn = end.GetComponent<SnakeBlockController>().GetPreviousColumn();
        snakeBody = Instantiate(snakeBodyPrefab, GetComponent<SnakeBlockController>().ConvertIntsIntoPosition(previousEndRow, previousEndColumn), Quaternion.identity);
        snakeBody.GetComponent<SnakeBlockController>().SetPosition(previousEndRow, previousEndColumn);
        end.GetComponent<SnakeBlockController>().SetSuccessor(snakeBody);
        end = snakeBody;
        gameModeManager.GetComponent<GameOverManager>().AddToScore(1);
        gameModeManager.GetComponent<GameOverManager>().SetNewHighscore();
    }

    /// <summary>
    /// The size of one of the blocks of the snake is set.
    /// It matches the size of a square in the gaming area.
    /// </summary>
    private void SetSize()
    {
        float cubeLength = gameModeManager.GetComponent<CreateWorld>().GetSquareLength();
        transform.localScale = new Vector3(cubeLength, .4f, cubeLength);
    }

    /// <summary>
    /// The direction in which the snake can move is set.
    /// It can never move in the opposite direction of its previous direction.
    /// </summary>
    public void SetDirection(DIRECTION dir)
    {
        direction = dir;
    }

    /// <summary>
    /// Returns the direction of the snake´s head
    /// </summary>
    /// <returns>Returns athe direction as a DIRECTION.</returns>
    public DIRECTION GetDirection()
    {
        return direction;
    }
}
