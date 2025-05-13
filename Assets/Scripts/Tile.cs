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

    public MinimapTile minimapTile; // the minimap tile tied to this tile
    public Sprite minimapSprite, minimapBg;
    public bool playerHasDiscovered; // if the player has previously walked on this tile before, used for minimap discovery

    public int x, y; // used for getting position

    public DungeonManager dm;

    void Awake(){
        x = (int)(transform.position.x/10);
        y = (int)(transform.position.z/10); // using z because 3D
    }

    public void EnterTile(Sprite s){ // called when the player walks onto a tile
        if(!playerHasDiscovered){
            DiscoverTile(s);
        }
        else{
            SetMiniMapSprite(s);
        }

        dm.MoveMinimapCamera(minimapTile.transform.position.x,minimapTile.transform.position.y); // move camera to this tile
    }

    public void SetMiniMapSprite(Sprite s){
        Debug.Log("Setting minimap sprite to "+s.name);
        // minimapTile.gameObject.SetActive(playerHasDiscovered);
        minimapTile.spriteRenderer.sprite = s;
    }

    public void UpdateMiniMapSprite(){
        Debug.Log("Setting minimap sprite to default");
        minimapTile.spriteRenderer.sprite = minimapSprite;
        minimapTile.spriteRendererBg.sprite = minimapBg;
        minimapTile.gameObject.SetActive(playerHasDiscovered); // hide if the tile hasn't been discovered yet
    }

    private void DiscoverTile(Sprite s){  // called when the tile is discovered for the first time
        playerHasDiscovered = true;
        minimapTile.gameObject.SetActive(true);
        // set wall sprites if there is a wall
        if(dm.GetTile(x,y+1) == null || dm.GetTile(x,y+1).walkable != true){
            minimapTile.wallUp.SetActive(true);
        }
        if(dm.GetTile(x,y-1) == null || dm.GetTile(x,y-1).walkable != true){
            minimapTile.wallDown.SetActive(true);
        }
        if(dm.GetTile(x+1,y) == null || dm.GetTile(x+1,y).walkable != true){
            minimapTile.wallRight.SetActive(true);
        }
        if(dm.GetTile(x-1,y) == null || dm.GetTile(x-1,y).walkable != true){
            minimapTile.wallLeft.SetActive(true);
        }

        SetMiniMapSprite(s);
    }

}

public enum MapIcon{
    Empty,
    Exit,
    Interactable
}
