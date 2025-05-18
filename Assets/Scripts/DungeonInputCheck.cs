using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class DungeonInputCheck : MonoBehaviour
{

    private PlayerController controller;

    // public KeyCode forward = KeyCode.W,
    // backward = KeyCode.S,
    // strafeLeft = KeyCode.Q,
    // strafeRight = KeyCode.E,
    // turnLeft = KeyCode.A,
    // turnRight = KeyCode.D,
    // interact = KeyCode.Space,
    // retract = KeyCode.LeftShift;


    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (controller.inputState == DungeonInputControlState.FreeMove)
        {
            if (InputManager.instance.moveForward) controller.Walk(0, 1);
            else if (InputManager.instance.moveBackward) controller.Walk(0, -1);
            else if (InputManager.instance.strafeRight) controller.Walk(1, 0);
            else if (InputManager.instance.strafeLeft) controller.Walk(-1, 0);
            else if (InputManager.instance.turnRight) controller.RotateRight();
            else if (InputManager.instance.turnLeft) controller.RotateLeft();
            else if (InputManager.instance.interact) controller.Interact();
            else if (InputManager.instance.options) controller.OpenOptions();
            else if (InputManager.instance.menu) controller.OpenMenu();
        }
        
        else if (controller.inputState == DungeonInputControlState.Dialogue)
        {
            if (InputManager.instance.interact) controller.ProgressDialogue();
        }

        else if (controller.inputState == DungeonInputControlState.Menu)
        {
            if (InputManager.instance.decline) controller.DeclineInMenu();
        }

    }
}

public enum DungeonInputControlState
{
    FreeMove,Dialogue,Combat,Menu
}