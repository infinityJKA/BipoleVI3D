using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System;

// this is the button used to select the equipment to equip in the equipment menu
public class EquipSelectMenuUIButton : MonoBehaviour, ISelectHandler
{
    public TMP_Text nameText, numberText;
    public Button button;
    public ItemObject equipment;
    public EquipInMenuUI equipUI;
    public RectTransform rectTransform;
    public int itemQuantity;
    public int itemNumber; // index of this button

    public void OnSelect(BaseEventData eventData)
    {
        equipUI.SnapToEquip(rectTransform, itemNumber);
        UpdateDescription();
    }

    public void OnClickFunction()
    {
        if (equipment == null) // this means you are unequipping an item and putting it back into your inventory
        {
            GameManager.gm.inventory.AddItem(equipUI.selectedCharacter.currentlyEquipped[equipUI.selectedEquipmentIndex], 1);
            equipUI.selectedCharacter.currentlyEquipped[equipUI.selectedEquipmentIndex] = null;
        }
        else    // remove item from inventory and add it to the equipment slot
        {
            if (equipUI.selectedCharacter.currentlyEquipped[equipUI.selectedEquipmentIndex] == null)
            {
                Debug.Log("Adding to slot that is empty");
                equipUI.selectedCharacter.currentlyEquipped[equipUI.selectedEquipmentIndex] = equipment;
                GameManager.gm.inventory.RemoveItem(equipUI.selectedCharacter.currentlyEquipped[equipUI.selectedEquipmentIndex], 1);
            }
            else {
                Debug.Log("Adding to slot that isn't empty");
                GameManager.gm.inventory.AddItem(equipUI.selectedCharacter.currentlyEquipped[equipUI.selectedEquipmentIndex], 1);
                equipUI.selectedCharacter.currentlyEquipped[equipUI.selectedEquipmentIndex] = equipment;
                GameManager.gm.inventory.RemoveItem(equipUI.selectedCharacter.currentlyEquipped[equipUI.selectedEquipmentIndex], 1);
            }
        }

        GameManager.gm.dungeonPlayer.eventSystem.SetSelectedGameObject(equipUI.equipmentButtons[equipUI.selectedEquipmentIndex].gameObject);
        GameManager.gm.dungeonPlayer.buttonSelectOnDecline = equipUI.selectedCharacterButton.gameObject;
        equipUI.CreateDisplayEquip();
        equipUI.CreateCurrentlyEquippedDisplay();
    }

    public void UpdateDescription()
    {
        if (equipment != null)
        {
            nameText.text = equipment.itemName;
            numberText.text = "x"+itemQuantity;

            equipUI.equipStats.text = equipment.itemName + "\n"
            + equipment.itemDescription + "\n\n"
            + equipment.equipmentAction.actionDescription;
        }
        else
        {
            nameText.text = "<UNEQUIP>";
            numberText.text = "";

            equipUI.equipStats.text = "[Unequip item]";
        }
    }


}
