using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool animateMovement = false;
    public float moveSpeed = 10f;
    public float rotateSpeed = 500f;

    Vector3 targetGridPos;
    Vector3 prevTargetGridPos;
    Vector3 targetRotation;

    private void Start(){
        targetGridPos = Vector3Int.RoundToInt(transform.position);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer(){
        if(true){ //if can move
            prevTargetGridPos = targetGridPos;
            Vector3 targetPosition = targetGridPos;

            if(targetRotation.y > 270f && targetRotation.y < 361){
                targetRotation.y = 0f;
            }
            if(targetRotation.y < 0f){
                targetRotation.y = 270f;
            }

            if(!animateMovement){
                transform.position = targetPosition;
                transform.rotation = Quaternion.Euler(targetRotation);
            }
            else{
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * rotateSpeed);
            }



        }
        else{
            targetGridPos = prevTargetGridPos;
        }
    }




    public void RotateLeft(){
        if(DoneMoving){
            targetRotation -= Vector3.up*90f;
        }
    }

    public void RotateRight(){
        if(DoneMoving){
            targetRotation += Vector3.up*90f;
        }
    }

    public void MoveForward(){
        if(DoneMoving){
            targetGridPos += transform.forward*10;
        }
    }

    public void MoveBack(){
        if(DoneMoving){
            targetGridPos -= transform.forward*10;
        }
    }

    public void MoveLeft(){
        if(DoneMoving){
            targetGridPos -= transform.right*10;
        }
    }

    public void MoveRight(){
        if(DoneMoving){
            targetGridPos += transform.right*10;
        }
    }





    bool DoneMoving {
        get{
            if( (Vector3.Distance(transform.position, targetGridPos) < 0.05f) && (Vector3.Distance(transform.eulerAngles, targetRotation) < 0.05f) ){
                return true;
            }
            else{return false;}
        }
    }





}
