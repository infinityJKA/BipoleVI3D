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
    }

    public void CharacterSwitchPressed()
    {
        equipUI.selectedCharacter = partyMember;
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
