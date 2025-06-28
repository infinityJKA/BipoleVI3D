using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class CheckSelectButton : MonoBehaviour, ISelectHandler
{
    public CombatUI combatUI;
    public PartyMember battler;
    public TMP_Text nameText;

    public void OnSelect(BaseEventData eventData)
    {
        combatUI.checkPortrait.sprite = battler.portrait;
        combatUI.checkMiniPortrait.sprite = battler.head;
        combatUI.checkSprite.sprite = battler.sprite;

        combatUI.checkName.text = battler.characterNameEn;
        combatUI.checkHP.text = "HP: " + battler.currentHP + "/" + battler.maxHP;
        combatUI.checkMP.text = "MP: " + battler.currentMP + "/" + battler.maxMP;

        if (battler.statusConditions.Count > 0)
        {
            combatUI.checkEffects.text = "Active Conditions: " + battler.statusConditions[0].amount + "x " + battler.statusConditions[0].stat + " " + battler.statusConditions[0].turns + "t";

            if (battler.statusConditions.Count > 1)
            {
                for (int i = 1; i < battler.statusConditions.Count; i++)
                {
                    combatUI.checkEffects.text = combatUI.checkEffects.text + ", " + battler.statusConditions[i].amount + "x " + battler.statusConditions[i].stat + " " + battler.statusConditions[i].turns + "t";
                }
            }
        }
        else
        {
            combatUI.checkEffects.text = "Active Conditions: None";
        }

    }

}
