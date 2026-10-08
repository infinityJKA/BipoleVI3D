using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NpcInteractPopupType
{
    Talk,
    Interact,
    Shop
}

public class OverworldNpc : MonoBehaviour
{
    public List<DungeonDialogue> dialogue;
    public NpcInteractPopupType popupType;

    public void Interact(){
        GameManager.gm.dungeonPlayer.StartDialogue(dialogue);
    }

}
