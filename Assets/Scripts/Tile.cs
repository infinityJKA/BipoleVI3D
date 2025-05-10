using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public MapIcon mapIcon;  // icon to show on the minimap
    public bool walkable = true, // the player can walk onto this tile
    eventOnWalk = false;  // if an event will be triggered when the player walks onto this tile
    public GameObject objectDisableOnWalk; // this object will be disabled when you are on this tile (for visibility reasons)
    public Sprite spriteShowOnWalk; // this sprite will be shown when on the canvas you are on this tile (ex. talking to an npc)

    public int x, y; // used for getting position

    void Awake(){
        x = (int)(transform.position.x/10);
        y = (int)(transform.position.z/10); // using z because 3D
    }



}

public enum MapIcon{
    Empty,
    Exit,
    Interactable
}
