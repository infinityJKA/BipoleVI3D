using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager gm;
	public PlayerController dungeonPlayer;
	public List<PartyMember> partyMembers;
	public List<PartyMember> partyMembersInit;
	public inventoryObject inventory, inventoryPrefab;

	[Header("Calendar")]
	public int day;
	public int month, year, dayofWeek, stepsSinceDayChange;
	public MoonPhase moonPhase;
	public int daysSinceMoonChange;
	public int eyePhase, stepsSinceEyeChange;

	[Header("Combat")]
	public List<DungeonDialogue> battleStartDialogue;
	public List<DungeonDialogue> partyMemberDiedDialogue, battleCompleteDialogue, gameOverDialogue;
	public int BP, enemyBP;
	public bool usingUlt, performedUltTrigger;

	[Header("Databases")]
	public List<ItemObject> allItems;
	public List<Sprite> allDialoguePortraits;
	public List<PartyMember> allPartyMembers;

	[Header("Automatic, don't edit")]
	public bool isLoadingSave;
	public int loadSaveNum;

	[Header("Combat (automatic don't edit)")]
	public List<PartyMember> enemies; // the enemies you are currently fighting
	public PartyMember currentBattler; // whoever's turn it is
	public EquipmentAction currentAction; // the currently selected action
	public PartyMember currentTarget; // the target of the current single-target action
	public int currentBodyPartIndex; // the target's body part that is being targeted, null if no specific target
	public List<ItemObject> itemsDropped; // what items to recieve at the end of the battle
	public float[] currentHitrates; // hitrates for the current target (normal, body, critical)
	public int expEarned; // how much exp to earn at the end of battle
	public GameObject currentAttackAnim; // will despawn when finished
	public bool inCombat;
	public InventorySlot itemToUse;

	void Awake()
	{
		if (gm != null)
			Destroy(this);
		else{
			gm = this;
			DontDestroyOnLoad(this);
		}

		

		day = 1;
		month = 1;
		year = 1;
		dayofWeek = 1;


		// scriptable objects save values onto the object itself so I have to create clones
		for (int p = 0; p < partyMembersInit.Count; p++)
		{
			var memberClone = Instantiate(partyMembersInit[p]);
			partyMembers.Add(memberClone);
		}

		var invenotryClone = Instantiate(inventoryPrefab);
		inventory = invenotryClone;


	}

	public void Save(int num)
	{

		SaveData data = new SaveData();
		data.currentSceneName = SceneManager.GetActiveScene().name;

		// SAVE PARTY MEMBERS

		foreach(PartyMember pm in partyMembers)
		{
			data.SavePartyMemberData(pm);
		}

		// SAVE CALENDAR
		data.day = day;
		data.month = month;
		data.year = year;
		data.dayofWeek = dayofWeek;
		data.moonPhase = moonPhase;
		data.eyePhase = eyePhase;
		data.stepsSinceEyeChange = stepsSinceEyeChange;
		data.stepsSinceDayChange = stepsSinceDayChange;
		data.daysSinceMoonChange = daysSinceMoonChange;

		// SAVE INVENTORY

		data.SaveInventory(inventory);

		// SAVE DUNGEON

		DungeonSaveData dungeonSaveData = new DungeonSaveData();
		dungeonSaveData.dungeonSceneName = SceneManager.GetActiveScene().name;

		data.currentDungeon = new CurrentDungeon();
		data.currentDungeon.playerPosition = dungeonPlayer.transform.position;
		data.currentDungeon.playerRotation = dungeonPlayer.transform.eulerAngles;
		data.currentDungeon.playerFacing = dungeonPlayer.playerFacing;
		data.currentDungeon.playerX = dungeonPlayer.playerX;
		data.currentDungeon.playerY = dungeonPlayer.playerY;

		// SAVE TILE IN DUNGEON

        dungeonSaveData.tileData = new List<TileSaveData>();

        foreach (Transform tr in dungeonPlayer.dm.transform)
		{
			Tile original = tr.GetComponent<Tile>();
			TileSaveData t = new TileSaveData();
			t.x = original.x;
			t.y = original.y;
			t.playerHasDiscovered = original.playerHasDiscovered;
			t.objectID = original.objectID;

			t.mapIcon = original.mapIcon;
			t.objectDisableOnWalk = original.objectDisableOnWalk;
			t.minimapTile = original.minimapTile;
			t.minimapSprite = original.minimapSprite;
			t.minimapBg = original.minimapBg;

			t.walkable = original.walkable;
			t.eventOnWalk = original.eventOnWalk;
			t.interactType = original.interactType;
			t.dialogue = original.dialogue;
			if(t.dialogue.Count > 0)
			{
				foreach(DungeonDialogue d in t.dialogue)
				{
					if (d.portrait != null)
					{
						d.spriteName = d.portrait.name;

                        d.portrait = null;
					}
				}
			}


			t.noEncounter = original.noEncounter;

			dungeonSaveData.tileData.Add(t);   
		}

		if (data.ContainsDungeon(dungeonSaveData.dungeonSceneName))
		{
			int ind = data.dungeons.IndexOf(data.GetDungeon(dungeonSaveData.dungeonSceneName));
			data.dungeons[ind] = dungeonSaveData;
		}
		else
		{
			if (data.dungeons == null)
			{
				data.dungeons = new List<DungeonSaveData>();
			}
			data.dungeons.Add(dungeonSaveData);
		}

		string json = JsonUtility.ToJson(data, true);
		string path = Application.persistentDataPath + "/save" + num + ".json";
		System.IO.File.WriteAllText(path, json);

		Debug.Log("Game saved to " + path);

	}

	public void LoadSaveData(int num)
	{
		Debug.Log("Loading save data from "+Application.persistentDataPath + "/save" + num + ".json");
		
        string jsonData = System.IO.File.ReadAllText(Application.persistentDataPath + "/save" + num + ".json");
        SaveData data = JsonUtility.FromJson<SaveData>(jsonData);
		
		// LOAD CALENDAR
		day = data.day;
		month = data.month;
		year = data.year;
		dayofWeek = data.dayofWeek;
		moonPhase = data.moonPhase;
		eyePhase = data.eyePhase;
		stepsSinceEyeChange = data.stepsSinceEyeChange;
		stepsSinceDayChange = data.stepsSinceDayChange;
		daysSinceMoonChange = data.daysSinceMoonChange;

		// LOAD PARTY MEMBERS

		gm.partyMembers = new List<PartyMember>();

		foreach (PartyMemberSaveData sd in data.party)
		{
			PartyMember pm;
			bool foundID = false;
			foreach (PartyMember m in allPartyMembers)
			{
				if (m.partyMemberInternalID == sd.partyMemberInternalID)
				{
					foundID = true;
					Debug.Log("Found matching party member with ID " + sd.partyMemberInternalID);

					pm = Instantiate(m);

                    pm.ATK = sd.ATK;
					pm.INT = sd.INT;
					pm.DEF = sd.DEF;
					pm.RES = sd.RES;
					pm.AGL = sd.AGL;
					pm.ACR = sd.ACR;
					pm.SPD = sd.SPD;	
					pm.LCK = sd.LCK;
					pm.EDR = sd.EDR;
					pm.maxHP = sd.maxHP;
					pm.currentHP = sd.currentHP;
					pm.maxMP = sd.maxMP;
					pm.currentMP = sd.currentMP;
					pm.EXP = sd.EXP;
					pm.LV = sd.LV;

					pm.currentlyEquipped = new ItemObject[4];
					for (int i = 0; i < 4; i++)
					{
						pm.currentlyEquipped[i] = GetItemByID(sd.currentlyEquipped[i]);
					}
					
					partyMembers.Add(pm);
                }
			}
			if(foundID == false)
			{
				Debug.Log("No matching party member found for ID " + sd.partyMemberInternalID);
			}
		}

		// LOAD INVENTORY
		inventory.Container.Clear();
		foreach (InventorySlotSaveData sd in data.inventory)
		{
			InventorySlot s = new InventorySlot(GetItemByID(sd.itemID), sd.amount);
			inventory.Container.Add(s);
		}

    }

	public void LoadDungeon(int num)
	{
        string jsonData = System.IO.File.ReadAllText(Application.persistentDataPath + "/save" + num + ".json");
        SaveData data = JsonUtility.FromJson<SaveData>(jsonData);

		DungeonSaveData sd;
		foreach(DungeonSaveData d in data.dungeons)
		{
			if(d.dungeonSceneName == SceneManager.GetActiveScene().name)
			{
				sd = d;

				dungeonPlayer = FindObjectOfType<PlayerController>();

				dungeonPlayer.transform.position = data.currentDungeon.playerPosition;
				dungeonPlayer.transform.eulerAngles = data.currentDungeon.playerRotation;
				dungeonPlayer.playerFacing = data.currentDungeon.playerFacing;
				dungeonPlayer.playerX = data.currentDungeon.playerX;
				dungeonPlayer.playerY = data.currentDungeon.playerY;
				dungeonPlayer.currentTile = dungeonPlayer.dm.GetTile(dungeonPlayer.playerX, dungeonPlayer.playerY);
				foreach (TileSaveData t in sd.tileData)
				{
					foreach (Transform tr in dungeonPlayer.dm.transform)
					{
						Tile original = tr.GetComponent<Tile>();
						if (original.objectID == t.objectID)
						{
							original.playerHasDiscovered = t.playerHasDiscovered;
							original.mapIcon = t.mapIcon;
							original.objectDisableOnWalk = t.objectDisableOnWalk;
							//original.minimapTile = t.minimapTile;
							original.minimapSprite = t.minimapSprite;
							original.minimapBg = t.minimapBg;
							original.walkable = t.walkable;
							original.eventOnWalk = t.eventOnWalk;
							original.interactType = t.interactType;
							original.dialogue = t.dialogue;
							if (original.dialogue.Count > 0)
							{
								foreach (DungeonDialogue di in original.dialogue)
								{
									if (di.spriteName != "")
									{
										foreach (Sprite s in allDialoguePortraits)
										{
											if (s.name == di.spriteName)
											{
												di.portrait = s;
											}
										}
									}
								}
							}
							original.noEncounter = t.noEncounter;
						}
					}
				}
            }	
        }
    }

    public void Load(int num)
    {

		LoadSaveData(num); // loads inventory and party data

		isLoadingSave = true; // dungeon save data will be loaded on load
		loadSaveNum = num;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
        
		
		//SceneManager.LoadSceneAsync(data.savedScene);
    }

	public ItemObject GetItemByID(string id)
	{
		foreach (ItemObject item in allItems)
		{
			if (item.saveId.Equals(id))
			{
				return item;
			}
		}

		Debug.Log("Error: Item with ID \"" + id + "\" not found in database (GameManager).");
		return null;
	}

    public void PartySwap(int a, int b)
	{

		PartyMember temp = partyMembers[a];

		Debug.Log("a is " + a);
		Debug.Log("b is " + b);



		partyMembers[a] = partyMembers[b];
		partyMembers[b] = temp;
	}

	public float[] CalculateHitRate()
	{
		int targetLCK = currentTarget.CalculateStat("LCK");
		Debug.Log("targetLCK " + targetLCK);
		int userLCK = currentBattler.CalculateStat("LCK");
		Debug.Log("userLCK: " + userLCK);
		int targetAGL = currentTarget.CalculateStat("AGL");

		float hitting = currentBattler.CalculateStat("ACR") * ((100 + currentAction.HIT) / 100f);
		Debug.Log("ACR*(Hit/100) = " + hitting);
		float dodge = targetAGL + (targetLCK * 0.25f);
		Debug.Log("targetAGL+(Lck*0.25) = " + dodge);

		float hitrateNormal = hitting * 2f / (hitting + dodge);
		Debug.Log("hitrate normal " + hitrateNormal);

		dodge = targetAGL + (targetLCK * 0.5f);
		float hitrateBody = hitting * 1.5f / (hitting + dodge);
		Debug.Log("hitrate body " + hitrateBody);

		float criticalChance = 1f * userLCK / (userLCK + (targetLCK * 9));
		Debug.Log("crit chance " + criticalChance);

		float[] result = new float[3];
		result[0] = hitrateNormal;
		result[1] = hitrateBody;
		result[2] = criticalChance;

		return result;
	}

	public bool PartyAlive()
	{
		bool isAlive = false;
		for (int i = 0; i < 4; i++)
		{
			if (partyMembers.Count > i)
			{
				if (partyMembers[i].currentHP > 0)
				{
					isAlive = true;
				}
			}
		}

		return isAlive;
	}

	private void OnApplicationQuit()
	{
		//inventory.Container.Clear();
	}


	public bool CanAffordAction(EquipmentAction action, PartyMember user)
	{
		if (action.costHP > 0)
		{
			if (action.setCost)
			{
				if (user.currentHP - action.costHP <= 0)
				{
					return false;
				}
			}
			else
			{
				if (user.currentHP - ((float)user.maxHP) * (action.costHP / 100f) <= 0)
				{
					return false;
				}
			}
		}
		else if (action.costMP > 0)
		{
			if (action.setCost)
			{
				if (user.currentMP - action.costMP < 0)
				{
					return false;
				}
			}
			else
			{
				if (user.currentMP - ((float)user.maxMP) * (action.costMP / 100f) < 0)
				{
					return false;
				}
			}
		}

		if (usingUlt)
		{
			if (action.isUlt)
			{
				if (user.isEnemy == false)
				{
					if (BP < action.costBP)
					{
						Debug.Log("BP ("+BP+") < action cost ("+action.costBP+")");
						return false;
					}
				}
				else
				{
                    if (enemyBP < action.costBP)
                    {
                        return false;
                    }
                }
			}
			else
			{
                if (user.isEnemy == false)
                {
                    if (BP < 15)
                    {
                        Debug.Log("BP (" + BP + ") < action cost (15)");
                        return false;
                    }
                }
                else
                {
                    if (enemyBP < 15)
                    {
                        return false;
                    }
                }
            }
		}

		return true;
	}

	public void PayAction(EquipmentAction action, PartyMember user)
	{
		if (action.costHP > 0)
		{
			if (action.setCost)
			{
				user.currentHP -= action.costHP;
			}
			else
			{
				user.currentHP -= (int)(((float)user.maxHP) * (action.costHP / 100f));
			}
            Debug.Log("Paid HP");
        }
		else if (action.costMP > 0)
		{
			if (action.setCost)
			{
				user.currentMP -= action.costMP;
			}
			else
			{
				user.currentMP -= (int)(((float)user.maxMP) * (action.costMP / 100f));
			}
			Debug.Log("Paid MP");
		}

        if (usingUlt)
        {
            if (action.isUlt)
            {
                if (user.isEnemy == false)
                {
					BP -= action.costBP;
                }
                else
                {
                    enemyBP -= action.costBP;
                }
            }
            else
            {
                if (user.isEnemy == false)
                {
                    BP -= 15;
                }
                else
                {
                    enemyBP -= 15;
                }
            }
        }

    }
}

[Serializable]
public enum MoonPhase {
	NewMoon,WaxingCrescent,FirstQuarter,WaxingGibbous,
	FullMoon,WaningGibbous,ThirdQuarter,WaningCrescent
}