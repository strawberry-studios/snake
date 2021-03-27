using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDirectionManager : MonoBehaviour
{
    public GameObject snakeHead; //refernces the snakeHead
    private bool directionChanged; //set false when direction was changed, assures that direction can only be changed once per movement
    public enum DIRECTION {up, down, left, right};

    private void Start()
    {
        directionChanged = false;
    }

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
