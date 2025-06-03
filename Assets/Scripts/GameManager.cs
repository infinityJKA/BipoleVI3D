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
	public int day, month, year, dayofWeek, stepsSinceDayChange;
	public MoonPhase moonPhase;
	public int daysSinceMoonChange;
	public int eyePhase, stepsSinceEyeChange;


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

	private void OnApplicationQuit()
	{
		//inventory.Container.Clear();
	}



}

public enum MoonPhase {
	NewMoon,WaxingCrescent,FirstQuarter,WaxingGibbous,
	FullMoon,WaningGibbous,ThirdQuarter,WaningCrescent
}