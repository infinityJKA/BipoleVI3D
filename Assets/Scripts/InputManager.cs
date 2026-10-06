using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    public bool debug1 { get; private set; }
    public bool debug2 { get; private set; }

    public Vector2 overworldMove { get; private set; }
    public Vector2 overworldRotation { get; private set; }

    private InputAction moveForwardAction, moveBackwardsAction, turnRightAction, turnLeftAction, strafeRightAction, strafeLeftAction, interactAction, declineAction, optionsAction, menuAction,debug1Action, debug2Action, overworldMoveAction, overworldRotateAction;
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
        eventSystem = GameObject.FindObjectOfType<EventSystem>();
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
        debug1 = debug1Action.WasPressedThisFrame();
        debug2 = debug2Action.WasPressedThisFrame();
        overworldMove = overworldMoveAction.ReadValue<Vector2>();
        overworldRotation = overworldRotateAction.ReadValue<Vector2>();
    }


    private void SetInputActions() // connects variables on awake
    {
        moveForwardAction = playerInput.actions["Walk Forwards"];
        moveBackwardsAction = playerInput.actions["Walk Backwards"];
        turnRightAction = playerInput.actions["Turn Right"];
        turnLeftAction = playerInput.actions["Turn Left"];
        strafeRightAction = playerInput.actions["Strafe Right"];
        strafeLeftAction = playerInput.actions["Strafe Left"];
        interactAction = playerInput.actions["Interact"];
        declineAction = playerInput.actions["Decline"];
        optionsAction = playerInput.actions["Options"];
        menuAction = playerInput.actions["Menu"];
        debug1Action = playerInput.actions["Debug1"];
        debug2Action = playerInput.actions["Debug2"];
        overworldMoveAction = playerInput.actions["Overworld Movement"];
        overworldRotateAction = playerInput.actions["Overworld Rotation"];
    }

}
