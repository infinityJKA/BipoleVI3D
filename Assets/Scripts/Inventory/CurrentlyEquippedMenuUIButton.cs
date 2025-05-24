using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System;

// this is the button used to select a currently equipped item
public class CurrentlyEquippedMenuUIButton : MonoBehaviour, ISelectHandler
{
    public TMP_Text nameText;
    public Button button;
    public EquipInMenuUI equipUI;
    public int itemNumber; // index of this button

    public void OnSelect(BaseEventData eventData)
    {
        GameManager.gm.dungeonPlayer.buttonSelectOnDecline = equipUI.selectedCharacterButton.gameObject;
        UpdateDescription();
        equipUI.DescriptionText();
        //equipUI.ResetButtonSnapEquip();
        equipUI.CreateDisplayEquip(false);
        
        // equipUI.SnapToEquip(equipUI.firstButtonEquip.rectTransform, equipUI.firstButtonEquip.itemNumber);
    }

    public void OnClickFunction()
    {
        equipUI.selectedEquipmentIndex = itemNumber;
        GameManager.gm.dungeonPlayer.buttonSelectOnDecline = button.gameObject;
        equipUI.SendToDisplayButtonsEquip();
    }

    public void UpdateDescription()
    {
        ItemObject item = equipUI.selectedCharacter.currentlyEquipped[itemNumber];
        if (item != null)
        {
            equipUI.equipStats.text = item.itemName + "\n"
            + item.itemDescription + "\n\n"
            + item.equipmentAction.actionDescription;
        }
        else
        {
            equipUI.equipStats.text = "[No item equipped in this slot]";
        }
    }

}
