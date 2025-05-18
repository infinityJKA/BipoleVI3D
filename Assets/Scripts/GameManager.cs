using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager gm;
	public PlayerController dungeonPlayer;
	public List<PartyMember> partyMembers;
	public inventoryObject inventory;


	void Awake()
	{
		if (gm != null)
			Destroy(gm);
		else
			gm = this;

		DontDestroyOnLoad(this);
	}

    private void OnApplicationQuit()
    {
		//inventory.Container.Clear();
    }



}