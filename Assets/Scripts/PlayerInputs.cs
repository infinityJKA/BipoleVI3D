using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInputs : MonoBehaviour{

    private PlayerController controller;

    public KeyCode forward = KeyCode.W,
    backward = KeyCode.S,
    strafeLeft = KeyCode.Q,
    strafeRight = KeyCode.E,
    turnLeft = KeyCode.A,
    turnRight = KeyCode.D;

    private void Awake(){
        controller = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(forward)) controller.MoveForward();
        if(Input.GetKeyDown(backward)) controller.MoveBack();
        if(Input.GetKeyDown(strafeLeft)) controller.MoveLeft();
        if(Input.GetKeyDown(strafeRight)) controller.MoveRight();
        if(Input.GetKeyDown(turnLeft)) controller.RotateLeft();
        if(Input.GetKeyDown(turnRight)) controller.RotateRight();

    }

}
