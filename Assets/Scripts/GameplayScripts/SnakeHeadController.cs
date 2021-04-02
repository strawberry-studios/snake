using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeHeadController : MonoBehaviour
{
    //READ THIS BEFORE YOU MAKE CHANGES TO THE SPAWNING OR MOVING:
    // Order of events: each frame a check is made whether the snake should move, (depending on the speed, it is more or less often true)
    // If it moves the movement of each block of the snake is completed at first: only the first and last block move. The second last block becomes 
    // the last block and the former last block becomes the second block. The direction can be changed at any time (via pressing the buttons or 
    // swipes, but either way always in fixed update). After the moving was completed, a  check is made whether the snake head now touches a collectable
    // If so, it is collected and a new one is spawned. The length of the snake is always one block longer than you can see, the last block being inactive.
    // When a new block is collected, the last inactive block is set active. Afterwards a new block is spawned (to prevent inefficency)

    /// <summary>
    /// another script that provides functionality for spawning pixeled blocks
    /// </summary>
    public SpawnPixeledBlocks SpawnPixeledBlocks { get; set; }

    public GameObject gameModeManager;
    public GameObject snakeBodyPrefab;
    private GameObject end; //references the end of the snake

    public GameObject directionManager; //references the directionManager
    public GameObject actualGameComponents; //game objects which should parent all components that are actually visible during gameplay
    GameObject snakeBlocksParent; //parents all snake blocks except for the head of the snake
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

    bool pixeled; //whether the blocks of the snake are pixeled
    bool gridLinesOn; //whether the grid lines should be visible or not

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
        SpawnPixeledBlocks = new SpawnPixeledBlocks();
        PlayerData data = DataSaver.Instance.RetrievePlayerDataFromFile();
        pixeled = data.GetShowPixels();
        gridLinesOn = data.GetShowGridLines();

        snakeBlocksParent = actualGameComponents.GetOrAddEmptyGameObject("SnakeBlocksParent");

        if (pixeled)
        {
            if (gridLinesOn)
                SpawnPixeledBlocks.SetupPixelsAndGridLines(snakeBodyPrefab, snakeBlocksParent);
            else
                SpawnPixeledBlocks.SetupPixelsNoGridLines(snakeBodyPrefab, snakeBlocksParent);

            SpawnPixeledBlocks.ModifySnakeHeadPixeled(gameObject);
        }
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
        GetComponent<SnakeBlockController>().SetPositionAndPreviousPosition(3, 3);
        SetFramesUntilMove();
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

            //SETS THE NEW POSITION OF THE SNAKE HEAD; IF WORLD BOUNDS ARE ON, TOUCHING THE WORLD MARGINS MEANS DEATH
            switch(direction)
            {
                case DIRECTION.up:
                    if (currentRow - 1 < 1)
                    {
                        if (noWorldBoundaries)
                            currentRow = rows + 1;
                        else
                            Lose(false);
                    }
                    GetComponent<SnakeBlockController>().SetPositionAndPreviousPosition(currentRow - 1, currentColumn);
                    break;
                case DIRECTION.down:
                    if (currentRow + 1 > rows)
                    {
                        if (noWorldBoundaries)
                            currentRow = 0;
                        else
                            Lose(false);
                    }
                    GetComponent<SnakeBlockController>().SetPositionAndPreviousPosition(currentRow + 1, currentColumn);
                    break;
                case DIRECTION.right:
                    if (currentColumn + 1 > columns)
                    {
                        if (noWorldBoundaries)
                            currentColumn = 0;
                        else
                            Lose(false);
                    }
                    GetComponent<SnakeBlockController>().SetPositionAndPreviousPosition(currentRow, currentColumn + 1);
                    break;
                case DIRECTION.left:
                    if (currentColumn - 1 < 1)
                    {
                        if (noWorldBoundaries)
                            currentColumn = columns + 1;
                        else
                            Lose(false);
                    }
                    GetComponent<SnakeBlockController>().SetPositionAndPreviousPosition(currentRow, currentColumn - 1);
                    break;
            }

            timeCount = 0.0f;
            directionManager.GetComponent<SetDirectionManager>().SetDirectionChanged(false);

            //MOVES THE REST OF THE SNAKE; ONLY THE LAST BLOCK IS MOVED AND BECOMES THE SECOND BLOCK (to make the moving more efficient)

            MovementCompleted = MoveRestOfSnake();//GetComponent<SnakeBlockController>().MoveSuccessor(); //the movement of the snake is set to completed once all
                                                                                      //blocks were moved
        }
        if (timeCount < framesUntilMove * 0.015 && Time.timeScale != 0.0f)
        {
            timeCount += Time.deltaTime;
        }
    }

    /// <summary>
    /// Moves the rest of the snake. The last block becomes the second one, the other blocks aren't moved.
    /// </summary>
    /// <returns>Returns true as soon as all of the movements (of all blocks) were completed.</returns>
    bool MoveRestOfSnake()
    {
        GameObject successor = GetComponent<SnakeBlockController>().Successor;
        int previousRow = GetComponent<SnakeBlockController>().GetPreviousRow();
        int previousColumn = GetComponent<SnakeBlockController>().GetPreviousColumn();

        if (GetComponent<SnakeBlockController>().IsSnakeLengthGreaterThan2(1))
        {
            //the snake length is greater than 2 and the last block of the snake will become the second one:

            GameObject newSecondBlock = GetComponent<SnakeBlockController>().SetNewLastBlock();
            GetComponent<SnakeBlockController>().Successor = newSecondBlock;
            newSecondBlock.GetComponent<SnakeBlockController>().Successor = 
                successor;

            //changes the position of the new second block, the 'visually visible' moving is thereby done;
            newSecondBlock.GetComponent<SnakeBlockController>().SetPositionInWorld(previousRow, previousColumn);

            //changes the coordinates-positions of all snakeBlocks:
            GetComponent<SnakeBlockController>().Successor.GetComponent<SnakeBlockController>().SetPositionInCoordinates(previousRow, previousColumn);
        }
        else
        {
            //the snake length is smaller or equal to 2, only the second block of the snake needs to be moved, if it exists:
            if (successor != null)
                successor.GetComponent<SnakeBlockController>().SetPositionAndPreviousPosition(previousRow, previousColumn);
        }
        return true;
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
        snakeBody.GetComponent<SnakeBlockController>().SetPositionAndPreviousPosition(previousEndRow, previousEndColumn);
        snakeBody.transform.SetParent(snakeBlocksParent.transform);
        end.GetComponent<SnakeBlockController>().Successor = snakeBody;
        end = snakeBody;
        gameModeManager.GetComponent<GameOverManager>().AddToScore(1);
        //gameModeManager.GetComponent<GameOverManager>().SetNewHighscore();

        //gameModeManager.GetComponent<SpawnCollectablesManager>().CreateNewCollectable();  //not used because of complexity and 
        // unpredicatability. But works.

        if (delayedSpawnings)
            gameModeManager.GetComponent<SpawnCollectablesSimplified>().CreateNewCollectableDelayed();
        else
            gameModeManager.GetComponent<SpawnCollectablesSimplified>().CreateNewCollectable();

        //PixelCollectable();
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
        if(pixeled)
        {
            return SpawnPixeledBlocks.CreateNewBlockPixeled(GetComponent<SnakeBlockController>().ConvertIntsIntoPosition(positionAsRow, positionAsColumn));
        }
        //if the pixel mode isn't activated:
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

/// <summary>
/// This class provides methods for spawning pixeled blocks. None of these methods are executed on their won, 
/// they're only called by other scripts (mainly 'SnakeHeadController')
/// </summary>
public class SpawnPixeledBlocks
{
    /// <summary>
    /// The game object which all game-managing scripts are attached to
    /// </summary>
    GameObject gameModeManager;
    /// <summary>
    /// The size of the pixels, is approximately equal to the thickness of gridLines, if activated
    /// </summary>
    float pixelSize;
    /// <summary>
    /// The length of a square of the world matrix (minus the thickness of the gridLines, if activated)
    /// </summary>
    float length;
    /// <summary>
    /// The number of pixels which one snake block will consist of.
    /// </summary>
    int pixelsNumber;
    /// <summary>
    /// The parent of the pixel objects which form a snake block as template to be instantiated.
    /// </summary>
    GameObject snakeBlockParentTemplate;
    /// <summary>
    /// The minimal position where the first pixel is spawned. Required for the pixel spawning iteration.
    /// </summary>
    float minPosition;
    /// <summary>
    /// The pixel template that can be duplicated for creating the single pixels of a snake block.
    /// </summary>
    GameObject pixel;
    /// <summary>
    /// The parent of the snake blocks.
    /// </summary>
    GameObject parentOfSnakeBlocks;

    /// <summary>
    /// Returns the pixel size.
    /// </summary>
    public float GetPixelSize()
    {
        return pixelSize;
    }

    //methods for creating the pixeled objects if gridLines and pixelMode are on:

    /// <summary>
    /// Sets up the necessary fields etc. for spawning snake blocks when pixel mode and the grid lines are activated.
    /// </summary>
    public void SetupPixelsAndGridLines(GameObject snakeBlockPrefab, GameObject newParentOfSnakeBlocks)
    {
        parentOfSnakeBlocks = newParentOfSnakeBlocks;

        //the length of a block and pixel size are determined:

        gameModeManager = GameObject.FindGameObjectWithTag("GameModeManager");
        CreateWorld cW = gameModeManager.GetComponent<CreateWorld>();
        pixelSize = cW.gridHorizontalPole.transform.lossyScale.x * cW.GridLinesFactor;
        length = cW.GetSquareLength() - pixelSize;

        //the number of pixels per snake block and the actual, accurate pixel size are determined:

        pixelsNumber = UnityEngineExtensions.RoundToUnevenNumber(length / pixelSize);
        pixelSize = length / pixelsNumber;

        //Creates the template that can be instantiated for the creation of the snake block parents:

        snakeBlockParentTemplate = GameObject.Instantiate(snakeBlockPrefab, new Vector3(20, 20, 20), Quaternion.identity) as GameObject;
        snakeBlockParentTemplate.transform.localScale = Vector3.one;
        snakeBlockParentTemplate.GetComponent<BoxCollider>().size = new Vector3(length, .4f, length);
        snakeBlockParentTemplate.SetActive(false);
        snakeBlockParentTemplate.transform.SetParent(parentOfSnakeBlocks.transform);

        //Creates the template that can be duplicated for creating the actual pixel objects:

        pixel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //Color colorOfTheSnake = DataSaver.Instance.GetSnakeColor().ConvertIntArrayIntoColor(); 
        //pixel.GetComponent<Renderer>().material.SetColor("_EmissionColor", colorOfTheSnake);
        //pixel.GetComponent<Renderer>().material.SetColor("_Color", colorOfTheSnake);
        //pixel.AddComponent<SetSnakeColor>();
        pixel.SetActive(false);
        pixel.transform.SetParent(parentOfSnakeBlocks.transform);
        pixel.transform.localScale = new Vector3(pixelSize, 1, pixelSize) ;
        pixel.GetComponent<Collider>().enabled = false;

        //sets the minimal position for the spawning iteration:

        minPosition = (-(int)(pixelsNumber / 2) + 1) * pixelSize;

        //optional print statements for debugging:
        //print("PixelSize:" + pixelSize); print("LengthOfASquare:" + length);
        //print("PixelsNumber" + pixelsNumber); print("minPosition:" + minPosition);
    }

    //methods for creating the pixeled objects if gridLines are off, but pixels on:

    /// <summary>
    /// Sets up all of the attributes regulating the spawning of pixeled objects that accord with each other graphically when the gridLines are off.
    /// </summary>
    public void SetupPixelsNoGridLines(GameObject snakeBlockPrefab, GameObject newParentOfSnakeBlocks)
    {
        parentOfSnakeBlocks = newParentOfSnakeBlocks;

        //the length of a block and pixel size are determined:

        gameModeManager = GameObject.FindGameObjectWithTag("GameModeManager");
        CreateWorld cW = gameModeManager.GetComponent<CreateWorld>();
        pixelSize = cW.GridLinesFactor * cW.gridHorizontalPole.transform.lossyScale.x;
        length = cW.GetSquareLength();

        //the number of pixels per snake block and the actual, accurate pixel size is determined:

        pixelsNumber = UnityEngineExtensions.RoundToEvenNumber(length / pixelSize);
        pixelSize = length / pixelsNumber;

        //Creates the template that can be instantiated for the creation of the snake block parents:

        snakeBlockParentTemplate = GameObject.Instantiate(snakeBlockPrefab, new Vector3(20, 20, 20), Quaternion.identity) as GameObject;
        snakeBlockParentTemplate.transform.localScale = Vector3.one;
        snakeBlockParentTemplate.GetComponent<BoxCollider>().size = new Vector3(length, .4f, length);
        snakeBlockParentTemplate.SetActive(false);
        snakeBlockParentTemplate.transform.SetParent(parentOfSnakeBlocks.transform);

        //Creates the template that can be duplicated for creating the actual pixel objects:

        pixel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //Color colorOfTheSnake = DataSaver.Instance.GetSnakeColor().ConvertIntArrayIntoColor(); 
        //pixel.GetComponent<Renderer>().material.SetColor("_EmissionColor", colorOfTheSnake);
        //pixel.GetComponent<Renderer>().material.SetColor("_Color", colorOfTheSnake);
        //pixel.AddComponent<SetSnakeColor>();
        pixel.SetActive(false);
        pixel.transform.SetParent(parentOfSnakeBlocks.transform);
        pixel.transform.localScale = new Vector3(pixelSize, 1, pixelSize);
        pixel.GetComponent<Collider>().enabled = false;

        //sets the minimal position for the spawning iteration:

        minPosition = (-(pixelsNumber / 2) + 1) * pixelSize;

        //optional print statements for debugging:
        //Debug.Log("PixelSize:" + pixelSize); Debug.Log("LengthOfASquare:" + length);
        //Debug.Log("PixelsNumber" + pixelsNumber); Debug.Log("minPosition:" + minPosition);
    }

    //methods for creating the pixeled objects independent of the gridLinesState:

    /// <summary>
    /// A new snake block is instantiated at the passed position. It is created as a pixeled object, the pixels are adapted to
    /// the grid lines.
    /// </summary>
    /// <param name="position">The position as Vector3 where the new snake block should be spawned.</param>
    /// <returns></returns>
    public GameObject CreateNewBlockPixeled(Vector3 position)
    {
        //the parent of the pixels is created
        //the parent will be referenced as snake block and execute all of the funtions, the pixels only move along

        GameObject snakeBlockParent = GameObject.Instantiate(snakeBlockParentTemplate, position, Quaternion.identity) as GameObject;
        snakeBlockParent.GetComponent<MeshRenderer>().enabled = false;
        snakeBlockParent.transform.localScale = Vector3.one;
        snakeBlockParent.GetComponent<Collider>().transform.localScale = new Vector3(length, .4f, length);
        snakeBlockParent.SetActive(true);

        //the actual pixel objects are created:

        for (int i = 0; i < pixelsNumber / 2; i++)
        {
            for (int j = 0; j < pixelsNumber / 2; j++)
            {
                GameObject smallPixel = GameObject.Instantiate(pixel, Vector3.zero, Quaternion.identity) as GameObject;
                smallPixel.transform.SetParent(snakeBlockParent.transform);
                smallPixel.transform.localPosition = new Vector3(minPosition + 2 * i * pixelSize, 1,
                                minPosition + 2 * j * pixelSize);
                smallPixel.SetActive(true);
                smallPixel.AddComponent<SetSnakeColor>();
            }
        }
        return snakeBlockParent;
    }

    /// <summary>
    /// Modifies the snake head object. It will be displayed pixeled.
    /// </summary>
    /// <param name="snakeHead">The snake head as game object.</param>
    /// <param name="cubeLength">The length of a snake head block as flaot.</param>
    public void ModifySnakeHeadPixeled(GameObject snakeHead)
    {
        //modify the existing snake-head object:

        snakeHead.GetComponent<MeshRenderer>().enabled = false;
        snakeHead.transform.localScale = Vector3.one;
        snakeHead.GetComponent<BoxCollider>().size = new Vector3(length * 0.9f, 1, length * 0.9f);

        //create pixel-children:

        for (int i = 0; i < pixelsNumber / 2; i++)
        {
            for (int j = 0; j < pixelsNumber / 2; j++)
            {
                GameObject smallPixel = GameObject.Instantiate(pixel, Vector3.zero, Quaternion.identity) as GameObject;
                smallPixel.transform.SetParent(snakeHead.transform);
                smallPixel.transform.localPosition = new Vector3(minPosition + 2 * i * pixelSize, 1,
                                minPosition + 2 * j * pixelSize);
                smallPixel.SetActive(true);
                smallPixel.AddComponent<SetSnakeColor>();
            }
        }
        //snakeHead.GetComponent<SnakeBlockController>().SetColorChildren();
    }

    /// <summary>
    /// Modifies the collecable objects. The collactable objects will be pixeled.
    /// <param name="collectableParent">The collectable object that will be pixeled (and becomes the parent of the pixels).</param>
    /// </summary>
    public void ModifyCollectablePixeled(GameObject collectableParent)
    {
        collectableParent.GetComponent<MeshRenderer>().enabled = false;
        collectableParent.transform.localScale = Vector3.one;
        collectableParent.GetComponent<BoxCollider>().size = new Vector3(length, 1, length);
        collectableParent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        //create pixel-children:

        for (int i = 0; i < pixelsNumber / 2; i++)
        {
            for (int j = 0; j < pixelsNumber / 2; j++)
            {
                GameObject smallPixel = GameObject.Instantiate(pixel, Vector3.zero, Quaternion.identity) as GameObject;
                smallPixel.transform.SetParent(collectableParent.transform);
                smallPixel.transform.localPosition = new Vector3(minPosition + 2 * i * pixelSize, .9f,
                                minPosition + 2 * j * pixelSize);
                smallPixel.SetActive(true);
                smallPixel.AddComponent<SetCollectablesColor>();
            }
        }
    }
}
