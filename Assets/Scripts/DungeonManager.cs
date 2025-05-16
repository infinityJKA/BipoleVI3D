using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public String dungeonName; // name of the dungeon that is displayed to the player
    public String dungeonID; // used for load/saving data, including the ids of tiles
    public Dictionary<Vector2, Tile> tiles = new Dictionary<Vector2, Tile>();

    public int lowestX,highestX,lowestY,highestY = 0;
    [SerializeField] GameObject minimapParent, minimapCamera;
    [SerializeField] MinimapTile minimapTilePrefab;


    void Start()
    {
        foreach(Transform child in transform)
        {
            // adds tile to tile dictonary
            Tile t = child.GetComponent<Tile>();
            tiles.Add(new Vector2(t.x,t.y), t);
            Debug.Log("Added tile "+t.x+","+t.y+" to tiles dictionary");
            t.dm = this;

            // sets tileID
            t.objectID = dungeonID+"_TILE_"+t.x+","+t.y;

            // updates highest/lowest xy values
            if (t.x < lowestX) { lowestX = t.x; }
            else if (t.x > highestX) { highestX = t.x; }
            if(t.y < lowestY){lowestY = t.y;}
            else if(t.y > highestY){highestY = t.y;}

            // creates minimap tile
            MinimapTile mmT = Instantiate(minimapTilePrefab, new Vector3(minimapParent.transform.position.x+t.x,minimapParent.transform.position.y+t.y), minimapParent.transform.rotation, minimapParent.transform);
            t.minimapTile = mmT;
            t.UpdateMiniMapSprite();
        }
    }

    public Tile GetTile(int x, int y){
        Debug.Log("Trying to get tile "+x+","+y);
        try{
            Tile t = tiles[new Vector2(x,y)];
            return t;
        }
        catch{
            return null;
        }
        
    }

    public void MoveMinimapCamera(float x, float y){
        minimapCamera.transform.position = new Vector3(x,y,-11);
    }
}
