using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System;

// this is the button used to select a party member to equip to
public class EquipToPartyInMenuUIButton : MonoBehaviour, ISelectHandler
{
    public TMP_Text nameText;
    public GameObject isActiveObject;
    public PartyMember partyMember;
    public Button button;
    public EquipInMenuUI equipUI;
    public RectTransform rectTransform;
    public int itemNumber; // index of this button

    public void OnSelect(BaseEventData eventData)
    {
        equipUI.SnapToParty(rectTransform, itemNumber);
        equipUI.selectedCharacter = partyMember;
        equipUI.CreateCurrentlyEquippedDisplay();
        GameManager.gm.dungeonPlayer.buttonSelectOnDecline = equipUI.sideBarEquipButton;
        equipUI.DescriptionText();
    }

    public void OnClickFunction()
    {
        GameManager.gm.dungeonPlayer.buttonSelectOnDecline = button.gameObject;
        GameManager.gm.dungeonPlayer.eventSystem.SetSelectedGameObject(equipUI.equipmentButtons[0].gameObject);        
    }



    public void UpdateGraphic()
    {
        nameText.text = partyMember.characterNameEn;

        if (itemNumber < 4)
        {
            Debug.Log(itemNumber + " is less than 4");
            isActiveObject.SetActive(true);
        }
        else
        {
            Debug.Log(itemNumber + " is not less than 4");
            isActiveObject.SetActive(false);
        }

    }


}
