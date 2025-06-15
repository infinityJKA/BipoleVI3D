using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

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
	public List<DungeonDialogue> partyMemberDiedDialogue, battleCompleteDialogue;
	
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

	void Awake()
	{
		if (gm != null)
			Destroy(gm);
		else
			gm = this;

		DontDestroyOnLoad(this);

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
		int userLCK = currentBattler.CalculateStat("LCK");
		int targetAGL = currentTarget.CalculateStat("AGL");

		float hitting = currentBattler.CalculateStat("ACR")*(currentAction.HIT/100f);
		float dodge =targetAGL+(targetLCK*0.25f);

		float hitrateNormal = hitting*2f/(hitting+dodge);
		Debug.Log("hitrate normal " + hitrateNormal);

		dodge = targetAGL+(targetLCK*0.5f);
		float hitrateBody = hitting*1.5f/(hitting+dodge);
		Debug.Log("hitrate body " + hitrateBody);

		float criticalChance = userLCK / (userLCK + (targetLCK * 5));
		Debug.Log("crit chance " + criticalChance);

		float[] result = new float[3];
		result[0] = hitrateNormal;
		result[1] = hitrateBody;
		result[2] = criticalChance;

		return result;
    }

	private void OnApplicationQuit()
	{
		//inventory.Container.Clear();
	}



}

public enum MoonPhase {
	NewMoon,WaxingCrescent,FirstQuarter,WaxingGibbous,
	FullMoon,WaningGibbous,ThirdQuarter,WaningCrescent
}