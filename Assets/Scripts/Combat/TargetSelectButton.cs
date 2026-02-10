using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class TargetSelectButton : MonoBehaviour, ISelectHandler
{
    public CombatUI combatUI;
    public PartyMember battler;
    public TMP_Text nameText;

    public void OnSelect(BaseEventData eventData)
    {
        if (battler.isEnemy) // show selection arrow for targeting an enemy
        {
            foreach (PartyMember enem in combatUI.gm.enemies)
            {
                if (enem != battler)
                {
                    enem.display.selectIcon.SetActive(false);
                }
                else
                {
                    enem.display.selectIcon.SetActive(true);
                }
            }
            
        }   
    }


    public void OnClick()
    {
        combatUI.gm.currentTarget = battler;
        combatUI.targetSelectedButton = gameObject;
        combatUI.HideMenusForDialogue();

        if (combatUI.gm.currentAction != null)
        {

            if (combatUI.gm.currentAction.PWR > 0 && combatUI.gm.currentAction.dontTargetBodyPart == false) // if damaging move, select body part
            {
                Debug.Log("pwr > 0");
                combatUI.GenerateBodyPartSelection();
            }
            else // otherwise just start the action
            {
                Debug.Log("pwr = 0");
                combatUI.gm.currentBodyPartIndex = -1;
                combatUI.HideMenusForDialogue();
                combatUI.gm.currentHitrates = combatUI.gm.CalculateHitRate();
                combatUI.gm.dungeonPlayer.StartDialogueCombat(combatUI.gm.currentAction.attackDialogue);
            }
        }
        else if(combatUI.gm.itemToUse != null)
        {
            combatUI.gm.currentBodyPartIndex = -1;
            combatUI.gm.dungeonPlayer.StartDialogueCombatItem(combatUI.gm.itemToUse.item);
        }
        
    }


}
