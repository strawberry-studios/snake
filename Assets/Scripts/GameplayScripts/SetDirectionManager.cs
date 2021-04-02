using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    /// Whether the game was already started or not.
    /// </summary>
    public bool GameStarted
    { get; set; }

    private void Start()
    {
        GameStarted = false;
        directionChanged = false;
        currentControlsMode = DataSaver.Instance.ControlsModeActivated;
        awaitingSwipe = true;
        currentTouchCount = 0;
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        deltaPixelLength = DataSaver.Instance.SwipesSensitivity * screenWidth * 0.28f + 0.02f * screenWidth;

        //print(deltaPixelLength);
    }

    //methods for controlling the snake via swipes:

    /// <summary>
    /// Changes the direction of the snake when a swipe is recognized. The sensitivity depends on the 'swipeSensitivity' (see: 'DataSaver').
    /// The first swipe can move the snake in any direction, afterwards it can only turn left or right relative to its current direction.
    /// </summary>
    public void ChangeDirectionViaSwipe()
    {
        if (!directionChanged)
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
            if (dir == 1 && snakeHead.GetComponent<SnakeHeadController>().GetDirection() != SnakeHeadController.DIRECTION.down)
            {
                snakeHead.GetComponent<SnakeHeadController>().SetDirection(SnakeHeadController.DIRECTION.up);
            }
            if (dir == 2 && snakeHead.GetComponent<SnakeHeadController>().GetDirection() != SnakeHeadController.DIRECTION.up)
            {
                snakeHead.GetComponent<SnakeHeadController>().SetDirection(SnakeHeadController.DIRECTION.down);
            }
            if (dir == 3 && snakeHead.GetComponent<SnakeHeadController>().GetDirection() != SnakeHeadController.DIRECTION.left)
            {
                snakeHead.GetComponent<SnakeHeadController>().SetDirection(SnakeHeadController.DIRECTION.right);
            }
            if (dir == 4 && snakeHead.GetComponent<SnakeHeadController>().GetDirection() != SnakeHeadController.DIRECTION.right)
            {
                snakeHead.GetComponent<SnakeHeadController>().SetDirection(SnakeHeadController.DIRECTION.left);
            }
            directionChanged = true;
        }
    }

    public void SetDirectionChanged(bool newDirectionState)
    {
        directionChanged = newDirectionState;
    }
}
