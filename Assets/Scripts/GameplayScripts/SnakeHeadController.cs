using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeHeadController : MonoBehaviour
{
    /// <summary>
    /// another script that provides functionality for spawning pixeled blocks
    /// </summary>
    //public SpawnPixeledBlocks SpawnPixeledBlocks { get; set; }

    public GameObject gameModeManager;
    public GameObject snakeBodyPrefab;
    private GameObject end; //references the end of the snake

    public GameObject directionManager; //references the directionManager
    /// <summary>
    /// Game object which parents all components that are atually visible during gameplay.
    /// </summary>
    public GameObject actualGameComponents; 
    GameObject snakeBlocksParent; //parents all snake blocks except for the head of the snake

    /// <summary>
    /// A pixeled snake block that is created in the start function. It is set inactive and can be duplicated for creating a new pixeled snake block
    /// without too big performance issues.
    /// </summary>
    private GameObject pixeledSnakeBlock;

    public enum DIRECTION { none, up, down, left, right };
    private DIRECTION direction;
    private int framesUntilMove; //Determines after how many frames a movement should occur, the speed is set by the player in the settings scene
    private float timeCount; //The frames since the last movement (at the beginning it is as big as framesUntilMove)
    float cubeLength; //the length of the snake head cube

    private int rows, columns; //the number of rows/columns of the world
    private bool delayedSpawnings; //whether the spawnings should be delayed or not
    /// <summary>
    /// True if no world boundaries exist, elsewhise false.
    /// </summary>
    bool noWorldBoundaries;

    /// <summary>
    /// Whether vibrations should be triggered when the player dies or not.
    /// </summary>
    private bool vibrationOn; 
    /// <summary>
    /// Whether the sound effects should be played or not. (apple collect sound, die sound)
    /// </summary>
    private bool soundsOn;

    //bool pixeled; //whether the blocks of the snake are pixeled
    //bool gridLinesOn; //whether the grid lines should be visible or not

    /// <summary>
    /// The current controls mode. Whether the snake can be controlled with buttons/swipes/both.
    /// </summary>
    ControlsMode currentControlsMode;
    /// <summary>
    /// The sound controller in the scene. A gameObject which isn't destroyed on load.
    /// </summary>
    GameObject soundController;

    /// <summary>
    /// Whether the player has already made the first move or not.
    /// </summary>
    public static bool GameStarted
    { get; set; }

    /// <summary>
    /// Returns whether the movement of all blocks of the snake was completed (true) or whether some of them are still moving (false)
    /// </summary>
    public bool MovementCompleted
    {
        get; set;
    }

    /// <summary>
    /// Sets up everything so that the snake blocks (also pixeled ones) can be spawned. 
    /// A new collectable is created.
    /// </summary>
    private void Startup()
    {
        SetSize();
        //SpawnPixeledBlocks = new SpawnPixeledBlocks();
        //PlayerData data = DataSaver.Instance.RetrievePlayerDataFromFile();
        //pixeled = data.GetShowPixels();
        //gridLinesOn = data.GetShowGridLines();

        snakeBlocksParent = actualGameComponents.GetOrAddEmptyGameObject("SnakeBlocksParent");

        //if (pixeled)
        //{
        //    if (gridLinesOn)
        //        SpawnPixeledBlocks.SetupPixelsAndGridLines(snakeBodyPrefab, snakeBlocksParent);
        //    else
        //        SpawnPixeledBlocks.SetupPixelsNoGridLines(snakeBodyPrefab, snakeBlocksParent);

        //    SpawnPixeledBlocks.ModifySnakeHeadPixeled(gameObject);
        //    pixeledSnakeBlock =  SpawnPixeledBlocks.CreateNewBlockPixeled(Vector3.zero);
        //    pixeledSnakeBlock.SetActive(false);
        //}
    }

    // Start is called before the first frame update
    void Start()
    {
        Startup();
        PlayerData currentData = DataSaver.Instance.RetrievePlayerDataFromFile();
        currentControlsMode = currentData.ControlsModeActivated;
        soundsOn = currentData.SoundOn;
        soundController = GameObject.FindGameObjectWithTag("SoundController");
        SetHeadColor();
        end = gameObject;
        SetFramesUntilMove();
        SetPositionOfSnakeHead();
        timeCount = framesUntilMove * 0.015f;
        direction = DIRECTION.none;
        StartCoroutine(CheckWhetherGameWasStarted());
        rows = gameModeManager.GetComponent<CreateWorld>().GetRows();
        columns = gameModeManager.GetComponent<CreateWorld>().GetColumns();
        MovementCompleted = true;
        delayedSpawnings = currentData.GetDelayedSpawningsState();
        vibrationOn = currentData.VibrationsOn;
        noWorldBoundaries = !DataSaver.Instance.GetWorldBoundariesState();
    }

    /// <summary>
    /// The position of the snake head is set. Currently the position is 3/3 in coordinates.
    /// </summary>
    void SetPositionOfSnakeHead()
    {
        GetComponent<SnakeBlockController>().SetPosition(StaticValues.PlayerStartX, StaticValues.PlayerStartY);
    }

    /// <summary>
    /// This coroutine is executed until the player made the first move. Once the player did, 'GameStarted' is set to true and 
    /// the coroutine ends.
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckWhetherGameWasStarted()
    {
        GameStarted = false;
        while (!GameStarted)
        {
            if (direction != DIRECTION.none)
                GameStarted = true;
            yield return null;
        }
        directionManager.GetComponent<SetDirectionManager>().GameStarted = true;
    }

    /// <summary>
    /// This method is only executed if the option 'mark snake head' in the settings is on.
    /// If so, it sets the color of the head of the snake. The color is slightly different from the other blocks of the snake.
    /// </summary>
    void SetHeadColor()
    {
        Color snakeHeadColor;
        if (DataSaver.Instance.GetSnakeHeadMarked())
            snakeHeadColor = DataSaver.Instance.GetSnakeHeadColor().ConvertIntArrayIntoColor();
        else
            snakeHeadColor = DataSaver.Instance.GetSnakeColor().ConvertIntArrayIntoColor();
        gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", snakeHeadColor);
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", snakeHeadColor);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentControlsMode != ControlsMode.buttonsOnly)
            directionManager.GetComponent<SetDirectionManager>().ChangeDirectionViaSwipe();
        Move();
    }

    /// <summary>
    /// If the snake´s head touches a collectable object, the snake grows bigger by one block.
    /// If it touches anoother block of the snake the player loses.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collectable"))
        {
            Destroy(other.gameObject);
            if (vibrationOn)
            {
                Vibration.Vibrate(500);
            }
            if (soundsOn)
                soundController.GetComponent<SoundController>().PlayAppleCollected();
            //IMPPRTANT: THE NEW BLOCK MUSTN'T BE CREATED BEFORE THE POSITION OF ALL OF THE BLOCKS OF THE SNAKE WAS UPDATED
            //Otherwise the block might spawn in the middle of the snake or not at all (in the middle: SpawnCollectablesManager, 
            //not at all: SpawnCollectiblesSimplified; but only when you collect 2 collectibles immidiately in a row
            StartCoroutine(CreateNewBlock());
        }
        if (other.gameObject.CompareTag("SnakeBlock"))
        {
            Lose(false);
        }
    }

    /// <summary>
    /// Plays a sound when the game is over and if the sounds are activated. Makes the device vibrate when the game
    /// is over and if the vibrations are activated. Whether the win or lose sound is played is determined within the method.
    /// </summary>
    void PlayLoseSoundAndVibrate()
    {
        if(soundsOn)
        {
            if (gameModeManager.GetComponent<GameOverManager>().GetScore() == 1000) //if the game was won
                soundController.GetComponent<SoundController>().PlayGameWonSound();
            else //if the game is only over, but wasn't won
                soundController.GetComponent<SoundController>().PlayGameOverSound();
        }
        if (vibrationOn)
            Vibration.Vibrate(800);
    }

    /// <summary>
    /// Plays a sound when the game is over and if the sounds are activated. Makes the device vibrate when the game
    /// is over and if the vibrations are activated. 
    /// </summary>
    /// <param name="won">Whether the game was won or not.</param>
    void PlayLoseSoundAndVibrate(bool won)
    {
        if (soundsOn)
        {
            if (won) //if the game was won
                soundController.GetComponent<SoundController>().PlayGameWonSound();
            else //if the game is only over, but wasn't won
                soundController.GetComponent<SoundController>().PlayGameOverSound();
        }
        if (vibrationOn)
            Vibration.Vibrate(800);
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
        if (timeCount >= framesUntilMove * 0.015)
        {
            MovementCompleted = false; //the snake is still moving

            int currentRow = GetComponent<SnakeBlockController>().GetCurrentRow();
            int currentColumn = GetComponent<SnakeBlockController>().GetCurrentColumn();

            switch(direction)
            {
                case DIRECTION.up:
                    if (currentRow - 1 < 1)
                    {
                        if (noWorldBoundaries)
                        {
                            currentRow = rows + 1;
                        }
                        else
                        {
                            Lose(false);
                        }
                    }
                    GetComponent<SnakeBlockController>().SetPosition(currentRow - 1, currentColumn);
                    break;
                case DIRECTION.down:
                        if (currentRow + 1 > rows)
                        {
                            if (noWorldBoundaries)
                            {
                                currentRow = 0;
                            }
                            else
                            {
                                Lose(false);
                            }
                        }
                        GetComponent<SnakeBlockController>().SetPosition(currentRow + 1, currentColumn);
                    break;
                case DIRECTION.right:
                    if (currentColumn + 1 > columns)
                    {
                        if (noWorldBoundaries)
                        {
                            currentColumn = 0;
                        }
                        else
                        {
                            Lose(false);
                        }
                    }
                    GetComponent<SnakeBlockController>().SetPosition(currentRow, currentColumn + 1);
                    break;
                case DIRECTION.left:
                    if (currentColumn - 1 < 1)
                    {
                        if (noWorldBoundaries)
                        {
                            currentColumn = columns + 1;
                        }
                        else
                        {
                            Lose(false);
                        }
                    }
                    GetComponent<SnakeBlockController>().SetPosition(currentRow, currentColumn - 1);
                    break;
            }
            
            timeCount = 0.0f;
            directionManager.GetComponent<SetDirectionManager>().SetDirectionChanged(false);
            //the direction in the SetDirectionManager can be changed again
            MovementCompleted = GetComponent<SnakeBlockController>().MoveSuccessor(); //the movement of the snake is set to completed once all
                                                                                      //blocks were moved
        }
        if (timeCount < framesUntilMove * 0.015 && Time.timeScale != 0.0f)
        {
            timeCount += Time.deltaTime;
        }
    }

    /// <summary>
    /// If the snake touches itself again the game is over. If the worldBoundaries toggle in the settings scene is toggled on
    /// the player also loses a life if he leaves the gaming area.
    /// </summary>
    /// <param name="won">Whether the game was won or not.</param>
    public void Lose(bool won)
    {
        GetComponent<SnakeBlockController>().SetSnakeInactive();
        gameModeManager.GetComponent<GameOverManager>().GameOver(won);
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
    /// After the creation of the new block a new collectible is spawned.
    /// </summary>
    private IEnumerator CreateNewBlock()
    {
        while (!MovementCompleted)
            yield return null;
        GameObject snakeBody;
        int previousEndColumn, previousEndRow; //the previous row and column (positions) of the end of the snake
        previousEndRow = end.GetComponent<SnakeBlockController>().GetPreviousRow();
        previousEndColumn = end.GetComponent<SnakeBlockController>().GetPreviousColumn();

        snakeBody = CreateANewSnakeBlock(previousEndRow, previousEndColumn) ;
        snakeBody.GetComponent<SnakeBlockController>().SetPosition(previousEndRow, previousEndColumn);
        snakeBody.transform.SetParent(snakeBlocksParent.transform);
        end.GetComponent<SnakeBlockController>().Successor = snakeBody;
        end = snakeBody;

        gameModeManager.GetComponent<GameOverManager>().AddToScore(1);

        if (delayedSpawnings)
            gameModeManager.GetComponent<SpawnCollectablesSimplified>().CreateNewCollectableDelayed();
        else
            gameModeManager.GetComponent<SpawnCollectablesSimplified>().CreateNewCollectable();
    }

    /// <summary>
    /// Pixels a collectable if the pixel mode is on.
    /// </summary>
    //public void PixelCollectable()
    //{
    //    if(pixeled)
    //    {
    //        if (gridLinesOn)
    //            foreach (Transform t in actualGameComponents.GetOrAddEmptyGameObject("collectablesHolder").transform)
    //                spawnPixeledBlocks.ModifyCollectablePixeledGridLines(t.gameObject);
    //        else
    //        { }
    //    }
    //}

    /// <summary>
    /// A new snake block is created. It depends on whether gridLines and pixelMode are on how the block will be created. (Pixeled or not)
    /// </summary>
    /// <param name="positionAsRow">The row-position of the new snake block in the world-matrix.</param>
    /// <param name="positionAsColumn">The column-position of the new snake block in the world-matrix.</param>
    /// <returns></returns>
    GameObject CreateANewSnakeBlock(int positionAsRow, int positionAsColumn)
    {
        return Instantiate(snakeBodyPrefab, GetComponent<SnakeBlockController>().ConvertIntsIntoPosition(positionAsRow, 
            positionAsColumn), Quaternion.identity);
    }

    /// <summary>
    /// The size of one of the blocks of the snake is set.
    /// It matches the size of a square in the gaming area.
    /// </summary>
    private void SetSize()
    {
        cubeLength = gameModeManager.GetComponent<CreateWorld>().GetSquareLength();
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

    //methods that are needed for spawning new collectibles:

    /// <summary>
    /// Returns all of the fields of the world that are currently occupied by the blocks of the snake as a 2-D array.
    /// </summary>
    /// <returns></returns>
    public bool[,] StartDeterminingOccupiedFields()
    {
        bool[,] currentlyOccupiedFields = new bool[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int k = 0; k < columns; k++)
            {
                currentlyOccupiedFields[i, k] = false;
            }
        }
        return GetComponent<SnakeBlockController>().DetermineOccupiedFields(currentlyOccupiedFields);
    }
}
