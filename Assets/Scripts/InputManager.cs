using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    [SerializeField] PlayerInput playerInput;
    public bool moveForward { get; private set; }
    public bool moveBackward { get; private set; }
    public bool turnRight { get; private set; }
    public bool turnLeft { get; private set; }
    public bool strafeRight { get; private set; }
    public bool strafeLeft { get; private set; }
    public bool interact { get; private set; }
    public bool decline { get; private set; }
    public bool options { get; private set; }
    public bool menu { get; private set; }
    private InputAction moveForwardAction, moveBackwardsAction, turnRightAction, turnLeftAction, strafeRightAction, strafeLeftAction, interactAction, declineAction, optionsAction, menuAction;
    public EventSystem eventSystem;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        SetInputActions();
    }

    private void Update()
    {
        moveForward = moveForwardAction.WasPressedThisFrame();
        moveBackward = moveBackwardsAction.WasPressedThisFrame();
        turnRight = turnRightAction.WasPressedThisFrame();
        turnLeft = turnLeftAction.WasPressedThisFrame();
        strafeRight = strafeRightAction.WasPressedThisFrame();
        strafeLeft = strafeLeftAction.WasPressedThisFrame();
        interact = interactAction.WasPressedThisFrame();
        decline = declineAction.WasPressedThisFrame();
        options = optionsAction.WasPressedThisFrame();
        menu = menuAction.WasPerformedThisFrame();
    }


    private void SetInputActions() // connects variables on awake
    {
        moveForwardAction = playerInput.actions["WalkForwardsDungeon"];
        moveBackwardsAction = playerInput.actions["WalkBackwardsDungeon"];
        turnRightAction = playerInput.actions["TurnRightDungeon"];
        turnLeftAction = playerInput.actions["TurnLeftDungeon"];
        strafeRightAction = playerInput.actions["StrafeRight"];
        strafeLeftAction = playerInput.actions["StrafeLeft"];
        interactAction = playerInput.actions["Interact"];
        declineAction = playerInput.actions["Decline"];
        optionsAction = playerInput.actions["Options"];
        menuAction = playerInput.actions["Menu"];
    }

}
