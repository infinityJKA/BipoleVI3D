
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<PartyMember> party;
    public inventoryObject inventory;

    public List<DungeonSaveData> dungeons;

    public bool ContainsDungeon(string n)
    {
        if(dungeons == null)
        {
            return false;
        }
        foreach (DungeonSaveData d in dungeons)
        {
            if (d.dungeonSceneName == n)
            {
                return true;
            }
        }

        return false;
    }

    public DungeonSaveData GetDungeon(string n)
    {
        foreach (DungeonSaveData d in dungeons)
        {
            if (d.dungeonSceneName == n)
            {
                return d;
            }
        }
        Debug.Log("Dungeon not found while trying to get dungeon data");
        return null;
    }


}

[System.Serializable]
public class DungeonSaveData
{
    public string dungeonSceneName;
    public Vector3 playerPosition;
    public Vector3 playerRotation;
    public PlayerFacing playerFacing;
    public List<MinimapTile> mapObjects;
    public List<Tile> dungeonTiles;



    
}

//foreach (GameObject obj in objects)
//{
//    data.objName = obj.name;
//    data.posX = obj.transform.position.x;
//    data.posY = obj.transform.position.y;
//    data.posZ = obj.transform.position.z;
//    string json = JsonUtility.ToJson(data, true);
//    Debug.Log(json.ToString());
//    sb.AppendLine(json);
//}