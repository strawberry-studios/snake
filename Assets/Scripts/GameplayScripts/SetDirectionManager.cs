using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Public class which references the 4 buttons of a keypad (if the keypad control is used).
/// </summary>
[Serializable]
public class KeypadButtons
{
    public GameObject buttonLeft, buttonRight, buttonUp, buttonDown;
    public Image buttonLeftImage, buttonRightImage, buttonDownImage, buttonUpImage;
    public PolygonCollider2D buttonLeftCollider, buttonRightCollider, buttonUpCollider, buttonDownCollider;
    public GameObject keypadClickAreaButton;
    public Collider2D pauseButtonCollider;

    /// <summary>
    /// Retruns the edge points of all Colliders to the console.
    /// </summary>
    public void GetColliderSizes()
    {
        for(int i = 0; i < buttonLeftCollider.points.Length; i++)
            Debug.Log("Left Collider " + buttonLeftCollider.points[i].x + "  " + buttonLeftCollider.points[i].y);

        for (int i = 0; i < buttonRightCollider.points.Length; i++)
            Debug.Log("Right Collider " + buttonRightCollider.points[i].x + "  " + buttonRightCollider.points[i].y);

        for (int i = 0; i < buttonUpCollider.points.Length; i++)
            Debug.Log("Upper Collider " + buttonUpCollider.points[i].x + "  " + buttonUpCollider.points[i].y);

        for (int i = 0; i < buttonDownCollider.points.Length; i++)
            Debug.Log("Lower Collider " + buttonDownCollider.points[i].x + "  " + buttonDownCollider.points[i].y);
    }
    
    /// <summary>
    /// Sets the transparency (alpha-value) of all keypad-button images to 80 and hence makes them quite transparent.
    /// </summary>
    public void SetImagesSlightlyTransparent()
    {
        buttonLeftImage.color = buttonLeftImage.color.GetColorWithNewA(80);
        buttonRightImage.color = buttonRightImage.color.GetColorWithNewA(80);
        buttonUpImage.color = buttonUpImage.color.GetColorWithNewA(80);
        buttonDownImage.color = buttonDownImage.color.GetColorWithNewA(80);
    }

    /// <summary>
    /// Toggles the keypad controllers active or inactive.
    /// </summary>
    /// <param name="newActivity">The new activity state of the keypad controllers.</param>
    public void ToggleKeypadControllersActive(bool newActivity)
    {
        buttonLeft.SetActive(newActivity);
        buttonRight.SetActive(newActivity);
        buttonUp.SetActive(newActivity);
        buttonDown.SetActive(newActivity);
        keypadClickAreaButton.SetActive(newActivity);
    }
}

public class SetDirectionManager : MonoBehaviour
{
    public GameObject snakeHead, gameModeManager; //references the snakeHead/gameModeManager
    private bool directionChanged; //set false when direction was changed, assures that direction can only be changed once per movement
    /// <summary>
    /// If true a direction-changing swipe is still expected to happen, elsewhise it already occurred.
    /// If it already occurred a new one can't be recognized before the touch count reached zero again.
    /// </summary>
    bool awaitingSwipe;
    /// <summary>
    /// The touch count of the previous frame.
    /// </summary>
    int previousTouchCount;
    /// <summary>
    /// The touch count of the current frame.
    /// </summary>
    int currentTouchCount;
    /// <summary>
    /// The position where the movement started.
    /// </summary>
    Vector2 movementStartPosition;
    /// <summary>
    /// Width/height of the screen in pixels.
    /// </summary>
    int screenWidth, screenHeight;
    /// <summary>
    /// The minimal distance in pixel coordinates that the finger needs to cover until a swipe is recognized.
    /// </summary>
    float deltaPixelLength;
    /// <summary>
    /// The current controls mode. Whether the snake can be controlled with buttons/swipes/both.
    /// </summary>
    ControlsMode currentControlsMode;
    /// <summary>
    /// The current direction of the snake.
    /// </summary>
    SnakeHeadController.DIRECTION currentSnakeDirection;
    /// <summary>
    /// All Buttons of the keypad with which the Snake can be navigated. (Only if the keypad control was selected by the player)
    /// </summary>
    public KeypadButtons keypadButtons;

    /// <summary>
    /// Whether the game was already started or not.
    /// </summary>
    public bool GameStarted
    { get; set; }

    /// <summary>
    /// Sets 'awaiting swipes' to the passed bool.
    /// </summary>
    /// <param name="state">The new state of 'awaiting swipes'.</param>
    public void SetAwaitingSwipes(bool state)
    {
        awaitingSwipe = state;
    }

    private void Awake()
    {
    }

    /// <summary>
    /// This method is called when the scene is exited.
    /// </summary>
    /// <param name="thisScene"></param>
    void OnSceneExit(Scene thisScene)
    {
        if (currentControlsMode == ControlsMode.keypad)
            keypadButtons.SetImagesSlightlyTransparent();
    }

    private void Start()
    {
        GameStarted = false;
        directionChanged = false;


        SceneManager.sceneUnloaded += OnSceneExit;

        PlayerData currentData = DataSaver.Instance.RetrievePlayerDataFromFile();

        currentControlsMode = currentData.ControlsModeActivated;

        if (currentControlsMode == ControlsMode.keypad)
        {
            keypadButtons.SetImagesSlightlyTransparent();
            //keypadButtons.keypadClickAreaButton.OnPointerDown CheckForKeypadInput;
        }

        awaitingSwipe = true;
        currentTouchCount = 0;
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        deltaPixelLength = currentData.SwipesSensitivity * screenWidth * 0.28f + 0.02f * screenWidth;
    }

    //methods for controlling the snake via swipes:

    /// <summary>
    /// Changes the direction of the snake when a swipe is recognized. The sensitivity depends on the 'swipeSensitivity' (see: 'DataSaver').
    /// The first swipe can move the snake in any direction, afterwards it can only turn left or right relative to its current direction.
    /// </summary>
    public void ChangeDirectionViaSwipe()
    {
        if (!directionChanged && !gameModeManager.GetComponent<PauseGameManager>().GamePaused)
        {
            previousTouchCount = currentTouchCount;
            currentTouchCount = Input.touchCount;

            if (currentTouchCount == 1)
            {
                if (previousTouchCount != 1) //the finger just started touching the screen
                {
                    awaitingSwipe = true;
                    movementStartPosition = Input.GetTouch(0).position;
                }
                else if(awaitingSwipe) //the finger has already started moving across the screen, checking for swipes:
                {
                    Vector2 currentTouchPosition = Input.GetTouch(0).position;
                    float deltaX = currentTouchPosition.x - movementStartPosition.x;
                    float deltaY = currentTouchPosition.y - movementStartPosition.y;

                    if (!GameStarted)
                        CheckForFirstSwipe(deltaX, deltaY);
                    else
                    {
                        currentSnakeDirection = snakeHead.GetComponent<SnakeHeadController>().GetDirection();
                        if (currentSnakeDirection == SnakeHeadController.DIRECTION.up || currentSnakeDirection == SnakeHeadController.DIRECTION.down)
                            MoveInXDirectionIfSwipe(deltaX);
                        else
                            MoveInYDirectionIfSwipe(deltaY);
                    }
                }
            }
            else //too many/no finger is touching the screen, not checking for swipes/current checks are cancelled:
            {
                awaitingSwipe = false;
            }
        }
    }

    ///// <summary>
    ///// Changes the direction of the snake when a keypad button is pressed. 
    ///// Only if one of the buttons is pressed (touchCount must be 1) the direction is changed.
    ///// </summary>
    //public void ChangeDirectionWithKeypad()
    //{
    //    if (!directionChanged && !gameModeManager.GetComponent<PauseGameManager>().GamePaused)
    //    {
    //        previousTouchCount = currentTouchCount;
    //        currentTouchCount = Input.touchCount;

    //        if (currentTouchCount == 1 && previousTouchCount != 1)
    //        {
    //            CheckForKeypadKeyPressed(Input.GetTouch(0).position);
    //        }
    //    }
    //}

    public void CheckForKeypadInput()
    {
        // How it works: Each image that should function as a button has a 2D-Polygon Collider attached to it.
        // The polygon collider determines the click area. Since 'ScaleWithScreenSize' is used for all UIs, the screen and world position of the UIs
        // are equal. Therefore the position of the touch doesn't need to and MUSTN'T be converted from screen to world coordinates.
        // By using the overlap method one can now check whether the user clicked one of the images or not.

        //print("Screen and converted World Position 2D: " + currentTouchPosition.x + "  " + currentTouchPosition.y);
        if (Input.touchCount == 1 && !directionChanged)
        {
            SnakeHeadController.DIRECTION currentDirection = snakeHead.GetComponent<SnakeHeadController>().GetDirection();

            Vector2 currentTouchPosition = Input.GetTouch(0).position;

            // alternative: keypadButtons.buttonUpCollider == Physics2D.OverlapPoint(currentTouchPosition)
            if (keypadButtons.buttonUpCollider.OverlapPoint(currentTouchPosition))
            {
                if (currentDirection != SnakeHeadController.DIRECTION.down)
                {
                    if (currentDirection != SnakeHeadController.DIRECTION.up)
                    {
                        directionChanged = true;
                        snakeHead.GetComponent<SnakeHeadController>().SetDirection(SnakeHeadController.DIRECTION.up);
                    }
                    StartCoroutine(HighlightUntilNotPressedAnyMore(keypadButtons.buttonUpCollider, keypadButtons.buttonUpImage));
                }
            }
            else if (keypadButtons.buttonRightCollider.OverlapPoint(currentTouchPosition))
            {
                if (currentDirection != SnakeHeadController.DIRECTION.left)
                {
                    if (currentDirection != SnakeHeadController.DIRECTION.right)
                    {
                        directionChanged = true;
                        snakeHead.GetComponent<SnakeHeadController>().SetDirection(SnakeHeadController.DIRECTION.right);
                    }
                    StartCoroutine(HighlightUntilNotPressedAnyMore(keypadButtons.buttonRightCollider, keypadButtons.buttonRightImage));
                }
            }
            else if (keypadButtons.buttonDownCollider.OverlapPoint(currentTouchPosition))
            {
                if (currentDirection != SnakeHeadController.DIRECTION.up)
                {
                    if (currentDirection != SnakeHeadController.DIRECTION.down)
                    {
                        directionChanged = true;
                        snakeHead.GetComponent<SnakeHeadController>().SetDirection(SnakeHeadController.DIRECTION.down);
                    }
                    StartCoroutine(HighlightUntilNotPressedAnyMore(keypadButtons.buttonDownCollider, keypadButtons.buttonDownImage));
                }
            }
            else if (keypadButtons.buttonLeftCollider.OverlapPoint(currentTouchPosition))
            {
                if (currentDirection != SnakeHeadController.DIRECTION.right)
                {
                    if (currentDirection != SnakeHeadController.DIRECTION.left)
                    {
                        directionChanged = true;
                        snakeHead.GetComponent<SnakeHeadController>().SetDirection(SnakeHeadController.DIRECTION.left);
                    }
                    StartCoroutine(HighlightUntilNotPressedAnyMore(keypadButtons.buttonLeftCollider, keypadButtons.buttonLeftImage));
                }
            }
        }
    }

    ///// <summary>
    ///// Checks whether touch(0) (the first recognized touch) overlaps with the collider of any of the kaypad buttons. If so, the direction is changed.
    ///// </summary>
    //void CheckForKeypadKeyPressed(Vector2 currentTouchPosition)
    //{
    //    // How it works: Each image that should function as a button has a 2D-Polygon Collider attached to it.
    //    // The polygon collider determines the click area. Since 'ScaleWithScreenSize' is used for all UIs, the screen and world position of the UIs
    //    // are equal. Therefore the position of the touch doesn't need to and MUSTN'T be converted from screen to world coordinates.
    //    // By using the overlap method one can now check whether the user clicked one of the images or not.

    //    //print("Screen and converted World Position 2D: " + currentTouchPosition.x + "  " + currentTouchPosition.y);
    //    SnakeHeadController.DIRECTION currentDirection = snakeHead.GetComponent<SnakeHeadController>().GetDirection();

    //    // alternative: keypadButtons.buttonUpCollider == Physics2D.OverlapPoint(currentTouchPosition)
    //    if (keypadButtons.buttonUpCollider.OverlapPoint(currentTouchPosition))
    //    {
    //        if (currentDirection != SnakeHeadController.DIRECTION.down && !keypadButtons.pauseButtonCollider.OverlapPoint(currentTouchPosition))
    //        {
    //            snakeHead.GetComponent<SnakeHeadController>().SetDirection(SnakeHeadController.DIRECTION.up);
    //            StartCoroutine(HighlightUntilNotPressedAnyMore(keypadButtons.buttonUpCollider, keypadButtons.buttonUpImage));
    //        }
    //    }
    //    else if (keypadButtons.buttonRightCollider.OverlapPoint(currentTouchPosition))
    //    {
    //        if (currentDirection != SnakeHeadController.DIRECTION.left && !keypadButtons.pauseButtonCollider.OverlapPoint(currentTouchPosition))
    //        {
    //            snakeHead.GetComponent<SnakeHeadController>().SetDirection(SnakeHeadController.DIRECTION.right);
    //            StartCoroutine(HighlightUntilNotPressedAnyMore(keypadButtons.buttonRightCollider, keypadButtons.buttonRightImage));
    //        }
    //    }
    //    else if (keypadButtons.buttonDownCollider.OverlapPoint(currentTouchPosition))
    //    {
    //        if (currentDirection != SnakeHeadController.DIRECTION.up)
    //        {
    //            snakeHead.GetComponent<SnakeHeadController>().SetDirection(SnakeHeadController.DIRECTION.down);
    //            StartCoroutine(HighlightUntilNotPressedAnyMore(keypadButtons.buttonDownCollider, keypadButtons.buttonDownImage));
    //        }
    //    }
    //    else if (keypadButtons.buttonLeftCollider.OverlapPoint(currentTouchPosition))
    //    {
    //        if (currentDirection != SnakeHeadController.DIRECTION.right)
    //        {
    //            snakeHead.GetComponent<SnakeHeadController>().SetDirection(SnakeHeadController.DIRECTION.left);
    //            StartCoroutine(HighlightUntilNotPressedAnyMore(keypadButtons.buttonLeftCollider, keypadButtons.buttonLeftImage));
    //        }
    //    }

    //    directionChanged = true;
    //}


    /// <summary>
    /// This coroutine makes the passed image fully intransparent when called. As long as the passed collider is touched by exactly one finger the 
    /// coroutine yields null, if that condition isn't fulfilled any more, the passed image is set slightly transparent again.
    /// </summary>
    /// <param name="thisKeypadCollider">The color that should be touched while the image is intransparent.</param>
    /// <param name="toBeAltered">The image whose transparency should be changed.</param>
    /// <returns></returns>
    IEnumerator HighlightUntilNotPressedAnyMore(PolygonCollider2D thisKeypadCollider, Image toBeAltered)
    {
        toBeAltered.color = toBeAltered.color.GetColorWithNewA(255);

        while (Input.touchCount == 1)
        {
            if (thisKeypadCollider.OverlapPoint(Input.GetTouch(0).position))
                yield return null;
            else
                break;
        }

        toBeAltered.color = toBeAltered.color.GetColorWithNewA(80);
    }

    /// <summary>
    /// Moves the snake in x direction. (Assigns 'left' or 'right' to 'Direction'.)
    /// If deltaX is positive 'right' is assigned, elsewhise 'left'.
    /// </summary>
    /// <param name="deltaX">The distance in x coordinates between the spot where the finger started touching the screen and where it is now.</param>
    void MoveInXDirection(float deltaX)
    {
        if (deltaX > 0)
            snakeHead.GetComponent<SnakeHeadController>().SetDirection(SnakeHeadController.DIRECTION.right);
        else
            snakeHead.GetComponent<SnakeHeadController>().SetDirection(SnakeHeadController.DIRECTION.left);
        directionChanged = true;
        awaitingSwipe = false;
    }

    /// <summary>
    /// Moves the snake in x direction if 'deltaX' is big enough so that a swipe could be recognized. 
    /// ('Moves' the snkae by assigning 'left' or 'right' to 'Direction'.)
    /// If deltaX is positive 'right' is assigned, elsewhise 'left'.
    /// </summary>
    /// <param name="deltaX">The distance in x coordinates between the spot where the finger started touching the screen and where it is now.</param>
    void MoveInXDirectionIfSwipe(float deltaX)
    {
        if (Mathf.Abs(deltaX) >= deltaPixelLength)
            MoveInXDirection(deltaX);
    }

    /// <summary>
    /// Moves the snake in y direction. (Assigns 'up' or 'down' to 'Direction'.)
    /// If deltaY is positive 'up' is assigned, elsewhise 'down'.
    /// </summary>
    /// <param name="deltaY">The distance in y coordinates between the spot where the finger started touching the screen and where it is now.</param>
    void MoveInYDirection(float deltaY)
    {
        if (deltaY > 0)
            snakeHead.GetComponent<SnakeHeadController>().SetDirection(SnakeHeadController.DIRECTION.up);
        else
            snakeHead.GetComponent<SnakeHeadController>().SetDirection(SnakeHeadController.DIRECTION.down);
        directionChanged = true;
        awaitingSwipe = false;
    }

    /// <summary>
    /// Moves the snake in y direction if 'deltaY' is big enough so that a swipe could be recognized. 
    /// ('Moves' the snkae by assigning 'down' or 'up' to 'Direction'.)
    /// If deltaY is positive 'up' is assigned, elsewhise 'down'.
    /// </summary>
    /// <param name="deltaY">The distance in y coordinates between the spot where the finger started touching the screen and where it is now.</param>
    void MoveInYDirectionIfSwipe(float deltaY)
    {
        if (Mathf.Abs(deltaY) >= deltaPixelLength)
            MoveInYDirection(deltaY);
    }

    /// <summary>
    /// Checks for the very first swipe, the new direction of the snake can be any direction.
    /// The 'swipe to start' text is deactivated.
    /// </summary>
    /// <param name="deltaX">The distance in x coordinates between the spot where the finger started touching the screen and where it is now.</param>
    /// <param name="deltaY">The distance in y coordinates between the spot where the finger started touching the screen and where it is now.</param>
    void CheckForFirstSwipe(float deltaX, float deltaY)
    {
        //the absolute amounts of delta x and y:
        float deltaYAbs = Mathf.Abs(deltaY);
        float deltaXAbs = Mathf.Abs(deltaX);
        bool firstSwipeExecuted = true; //whether the first swipe was made or not

        if (deltaXAbs >= deltaPixelLength)
        {
            if (deltaYAbs >= deltaPixelLength)
            {
                if (deltaXAbs > deltaYAbs)
                    MoveInXDirection(deltaX);
                else
                    MoveInYDirection(deltaY);
            }
            else
                MoveInXDirection(deltaX);
        }
        else
        {
            if (deltaYAbs >= deltaPixelLength)
                MoveInYDirection(deltaY);
            else
                firstSwipeExecuted = false; //only scenario where the first swipe hasn't been made yet
        }

        if (firstSwipeExecuted)  //if the first swipe was made:
            gameModeManager.GetComponent<GameOverManager>().ToggleSwipeControllersActive(false);
    }

    //methods for controlling the snake via buttons:

    /// <summary>
    /// Changes the direction of the snake. Should be attached to the buttons controlling the direction of the snake.
    /// </summary>
    /// <param name="dir"></param>
    public void SetDirection(int dir)
    {
        if (!directionChanged)
        {
            SnakeHeadController.DIRECTION currentDir = snakeHead.GetComponent<SnakeHeadController>().GetDirection();

            switch(dir)
            {
                case 1:
                    if (currentDir != SnakeHeadController.DIRECTION.down && currentDir != SnakeHeadController.DIRECTION.up)
                    {
                        snakeHead.GetComponent<SnakeHeadController>().SetDirection(SnakeHeadController.DIRECTION.up);
                        directionChanged = true;
                    }
                        break;
                case 2:
                    if (currentDir != SnakeHeadController.DIRECTION.down && currentDir != SnakeHeadController.DIRECTION.up)
                    { 
                        snakeHead.GetComponent<SnakeHeadController>().SetDirection(SnakeHeadController.DIRECTION.down);
                        directionChanged = true;
                    }
                    break;
                case 3:
                    if (currentDir != SnakeHeadController.DIRECTION.left && currentDir != SnakeHeadController.DIRECTION.right)
                    { 
                        snakeHead.GetComponent<SnakeHeadController>().SetDirection(SnakeHeadController.DIRECTION.right);
                        directionChanged = true;
                    }
                    break;
                case 4:
                    if (currentDir != SnakeHeadController.DIRECTION.left && currentDir != SnakeHeadController.DIRECTION.right)
                    { 
                        snakeHead.GetComponent<SnakeHeadController>().SetDirection(SnakeHeadController.DIRECTION.left);
                        directionChanged = true;
                    }
                    break;
                default: 
                    //should never occur, nothing happens
                    break; 
            }
        }
    }

    public void SetDirectionChanged(bool newDirectionState)
    {
        directionChanged = newDirectionState;
    }

    /// <summary>
    /// Method which prints "Something just happened" on the console, can be used for testing purposes.
    /// </summary>
    public void ConfirmThatSomethingHappens()
    {
        Debug.Log("Something just happened");
    }
}
