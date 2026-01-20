using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombatItemSelectButton : MonoBehaviour, ISelectHandler
{
    public CombatUI combatUI;
    public InventorySlot inventorySlot;
    public TMP_Text nameText;


    public void OnClick()
    {
        combatUI.gm.itemToUse = inventorySlot;
        combatUI.HideMenusForDialogue();
        combatUI.gm.dungeonPlayer.StartDialogueCombat(combatUI.gm.currentAction.attackDialogue);

    }

    public void OnSelect(BaseEventData eventData)
    {
        combatUI.itemDescription.text = inventorySlot.item.itemName + "\n" + inventorySlot.item.itemDescription;
    }


}
