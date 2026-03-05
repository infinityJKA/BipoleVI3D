using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BodyPartSelectButton : MonoBehaviour
{
    public CombatUI combatUI;
    public int bodyPartIndex;
    public TMP_Text nameText;


    public void OnClick()
    {
        combatUI.gm.currentBodyPartIndex = bodyPartIndex;
        combatUI.HideMenusForDialogue();

        combatUI.gm.PayAction(combatUI.gm.currentAction, combatUI.gm.currentBattler);

        combatUI.gm.dungeonPlayer.StartDialogueCombat(combatUI.gm.currentAction.attackDialogue);
        
    }


}
