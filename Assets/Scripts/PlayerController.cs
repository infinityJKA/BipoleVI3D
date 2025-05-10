using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] DungeonManager dm;
    public bool animateMovement = false;
    public float moveSpeed = 10f;
    public float rotateSpeed = 500f;
    public int playerX,playerY = 0;
    public PlayerFacing playerFacing = PlayerFacing.North;

    Vector3 targetGridPos;
    Vector3 prevTargetGridPos;
    Vector3 targetRotation;

    private void Start(){
        targetGridPos = Vector3Int.RoundToInt(transform.position);
    }

    private void FixedUpdate()
    {
        MovePlayerObject();
    }

    void MovePlayerObject(){
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


    public void Walk(int x, int y){    // x is to the side, y is upwards
        if(DoneMoving){
            int oldX = x;
            int oldY = y;

            // adjust for rotation
            if(playerFacing == PlayerFacing.South){
                x *= -1;
                y *= -1;
            }
            else if(playerFacing == PlayerFacing.East){
                oldY = y;
                y = -1*x;
                x = oldY;
            }
            else if(playerFacing == PlayerFacing.West){
                oldY = y;
                y = x;
                x = -1*oldY;
            }
            Debug.Log("Old: "+oldX+","+oldY+"  New: "+x+","+y);
            Tile t = dm.GetTile(playerX+x,playerY+y);
            if(t != null){
                if(t.walkable){
                    playerX += x;  
                    playerY += y;
                    targetGridPos = t.transform.position;
                    // if(oldX > 0){    // walk forwards
                    //     targetGridPos += transform.forward*10;
                    // }
                    // else if(oldX < 0){    // walk backwards
                    //     targetGridPos -= transform.forward*10;
                    // }
                    // else if(oldY > 0){    // walk right
                    //     targetGridPos += transform.right*10;
                    // }
                    // else if(oldY < 0){    // walk left
                    //     targetGridPos -= transform.right*10;
                    // }
                    // else{
                    //     Debug.Log("wtf man");
                    // }
                }
                else{
                    Debug.Log("Trying to walk to a nonwalkable tile!");
                }
            }
            else{
                Debug.Log("Tile you are trying to walk to is NULL!");
            }
        }
    }


    public void RotateLeft(){
        if(DoneMoving){
            if(playerFacing == PlayerFacing.North){
                playerFacing = PlayerFacing.West;
            }
            else if(playerFacing == PlayerFacing.West){
                playerFacing = PlayerFacing.South;
            }
            else if(playerFacing == PlayerFacing.South){
                playerFacing = PlayerFacing.East;
            }
            else{
                playerFacing = PlayerFacing.North;
            }
            targetRotation -= Vector3.up*90f;
        }
    }

    public void RotateRight(){
        if(DoneMoving){
            if(playerFacing == PlayerFacing.North){
                playerFacing = PlayerFacing.East;
            }
            else if(playerFacing == PlayerFacing.West){
                playerFacing = PlayerFacing.North;
            }
            else if(playerFacing == PlayerFacing.South){
                playerFacing = PlayerFacing.West;
            }
            else{
                playerFacing = PlayerFacing.South;
            }
            targetRotation += Vector3.up*90f;
        }
    }

    // public void MoveForward(){
    //     if(DoneMoving){
    //         targetGridPos += transform.forward*10;
    //     }
    // }

    // public void MoveBack(){
    //     if(DoneMoving){
    //         targetGridPos -= transform.forward*10;
    //     }
    // }

    // public void MoveLeft(){
    //     if(DoneMoving){
    //         targetGridPos -= transform.right*10;
    //     }
    // }

    // public void MoveRight(){
    //     if(DoneMoving){
    //         targetGridPos += transform.right*10;
    //     }
    // }





    bool DoneMoving {
        get{
            if( (Vector3.Distance(transform.position, targetGridPos) < 0.00001f) && (Vector3.Distance(transform.eulerAngles, targetRotation) < 0.00001f) ){
                return true;
            }
            else{return false;}
        }
    }





}



public enum PlayerFacing{
    North,South,East,West
}