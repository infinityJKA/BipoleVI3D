using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public Dictionary<Vector2, Tile> tiles = new Dictionary<Vector2, Tile>();

    public int lowestX,highestX,lowestY,highestY = 0;

    void Start()
    {
        foreach(Transform child in transform)
        {
            Tile t = child.GetComponent<Tile>();
            tiles.Add(new Vector2(t.x,t.y), t);
            Debug.Log("Added tile "+t.x+","+t.y+" to tiles dictionary");

            if(t.x < lowestX){lowestX = t.x;}
            else if(t.x > highestX){highestX = t.x;}
            if(t.y < lowestY){lowestY = t.y;}
            else if(t.y > highestY){highestY = t.y;}
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
}
