using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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

    [SerializeField] Tile currentTile;

    Vector3 targetGridPos;
    Vector3 prevTargetGridPos;
    Vector3 targetRotation;

    [SerializeField] Sprite up,down,left,right;

    private void Start(){
        targetGridPos = Vector3Int.RoundToInt(transform.position);
        currentTile = dm.GetTile(playerX,playerY);
        // currentTile.playerHasDiscovered = true;
        currentTile.EnterTile(PlayerMapSprite());
    }

    private void FixedUpdate()
    {
        MovePlayerObject();
    }

    private Sprite PlayerMapSprite(){
        if(playerFacing == PlayerFacing.North){
            return up;
        }
        else if(playerFacing == PlayerFacing.South){
            return down;
        }
        else if(playerFacing == PlayerFacing.East){
            return right;
        }
        else{
            return left;
        }
    }

    private void OLDMinimapSprite(Tile t){
        if(playerFacing == PlayerFacing.North){
            t.SetMiniMapSprite(up);
        }
        else if(playerFacing == PlayerFacing.South){
            t.SetMiniMapSprite(down);
        }
        else if(playerFacing == PlayerFacing.East){
            t.SetMiniMapSprite(right);
        }
        else{
            t.SetMiniMapSprite(left);
        }
    }

    void MovePlayerObject(){
        // if(true){ //if can move
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



        // }
        // else{
        //     targetGridPos = prevTargetGridPos;
        // }
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
                    
                    currentTile.UpdateMiniMapSprite(); // reset the minimap sprite before leaving
                    currentTile = t; // set new current tile
                    t.EnterTile(PlayerMapSprite()); // update minimap sprites

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
            
            dm.GetTile(playerX,playerY).EnterTile(PlayerMapSprite());
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

            dm.GetTile(playerX,playerY).EnterTile(PlayerMapSprite());
            // MinimapSprite(dm.GetTile(playerX,playerY));
        }
    }




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