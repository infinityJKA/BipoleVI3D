
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

[System.Serializable]
public class SaveData
{
    public int day, month, year, dayofWeek, eyePhase, stepsSinceEyeChange, stepsSinceDayChange,daysSinceMoonChange;
    public MoonPhase moonPhase;
    public List<PartyMemberSaveData> party = new List<PartyMemberSaveData>();
    public List<InventorySlotSaveData> inventory = new List<InventorySlotSaveData>();

    public CurrentDungeon currentDungeon;
    public List<DungeonSaveData> dungeons = new List<DungeonSaveData>();

    public String currentSceneName;

    

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

    public void SavePartyMemberData(PartyMember pm)
    {
        PartyMemberSaveData sd = new PartyMemberSaveData();
        sd.partyMemberInternalID = pm.partyMemberInternalID;
        sd.ATK = pm.ATK;
        sd.INT = pm.INT;
        sd.DEF = pm.DEF;
        sd.RES = pm.RES;
        sd.AGL = pm.AGL;
        sd.ACR = pm.ACR;
        sd.SPD = pm.SPD;
        sd.LCK = pm.LCK;
        sd.EDR = pm.EDR;
        sd.maxHP = pm.maxHP;
        sd.currentHP = pm.currentHP;
        sd.maxMP = pm.maxMP;
        sd.currentMP = pm.currentMP;
        sd.EXP = pm.EXP;
        sd.LV = pm.LV;

        sd.currentlyEquipped = new String[pm.currentlyEquipped.Length];
        foreach (ItemObject i in pm.currentlyEquipped)
        {
            if (i != null)
            {
                sd.currentlyEquipped[Array.IndexOf(pm.currentlyEquipped, i)] = i.saveId;
            }
            else
            {
                sd.currentlyEquipped[Array.IndexOf(pm.currentlyEquipped, i)] = null;
            }
        }

        party.Add(sd);
    }

    public void SaveInventory(inventoryObject inv)
    {
        if(inv.Container.Count > 0)
        {
            foreach(InventorySlot s in inv.Container)
            {
                Debug.Log("Saving inventory slot with item "+s.item.name+" and amount "+s.amount);
                InventorySlotSaveData sd = new InventorySlotSaveData();
                sd.itemID = s.item.saveId;
                sd.amount = s.amount;
                inventory.Add(sd);
            }
        }
    }
}

[System.Serializable]
public class CurrentDungeon
{
    public string dungeonSceneName;
    public Vector3 playerPosition;
    public Vector3 playerRotation;
    public PlayerFacing playerFacing;
    public int playerX, playerY;
    
}

[System.Serializable]
public class DungeonSaveData
{
    public string dungeonSceneName;
    public List<TileSaveData> tileData;

}

[System.Serializable]
public class PartyMemberSaveData
{
    public string partyMemberInternalID;
    public int
    ATK, // physical strength
    INT, // magical strength
    DEF, // physical defense
    RES, // magical defense
    AGL, // makes it easier to dodge
    ACR, // makes attacks more accurate
    SPD, // makes it so you act faster/more often
    LCK, // slight influences dodging/landing attacks AND affects critical hit chance
    EDR, // how many times you can get hit in the same body part without BREAKing, doesn't increase with level
    maxHP, currentHP, // health points
    maxMP, currentMP, // used for spells and abilities
    EXP, LV;

    public String[] currentlyEquipped;

}

[System.Serializable]
public class InventorySlotSaveData
{
    public String itemID;
    public int amount;
}

[System.Serializable]
public class TileSaveData
{
    public int x, y; // used for getting position
    public bool playerHasDiscovered; // if the player has previously walked on this tile before, used for minimap discovery
    public String objectID; // generated when object is initialized, used to save/load data when the scene is loaded.
    public bool hasChild;
    public String childSpriteName;

    [Header("Graphics")]
    public MapIcon mapIcon;  // icon to show on the minimap
    public GameObject objectDisableOnWalk; // this object will be disabled when you are on this tile (for visibility reasons)
    public String minimapSpriteName; // the minimap tile tied to this tile
    public Sprite minimapBg;

    [Header("Interaction Logic")]
    public bool walkable = true, // the player can walk onto this tile
    eventOnWalk = false;  // if an event will be triggered when the player walks onto this tile
    public InteractType interactType = InteractType.None; // used for displaying the interaction popup
    public List<DungeonDialogue> dialogue; // dialogue read from when event is triggered (from walking or from interacting)
    public bool noEncounter; // if you can get a random encounter here
}