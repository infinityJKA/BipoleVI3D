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
        if(Input.GetKeyDown(forward)) controller.Walk(1,0);
        if(Input.GetKeyDown(backward)) controller.Walk(-1,0);
        if(Input.GetKeyDown(strafeLeft)) controller.Walk(0,-1);
        if(Input.GetKeyDown(strafeRight)) controller.Walk(0,1);
        if(Input.GetKeyDown(turnLeft)) controller.RotateLeft();
        if(Input.GetKeyDown(turnRight)) controller.RotateRight();

    }

}
