using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System;
using UnityEditor;

public class PartyInMenuUIButton : MonoBehaviour, ISelectHandler
{
    public TMP_Text nameText, levelText;
    public GameObject isDefeatedObject, isActiveObject; 
    public PartyMember partyMember;
    public Button button;
    public PartyInMenuUI partyUI;
    public RectTransform rectTransform;
    public int itemNumber;

    public void OnSelect(BaseEventData eventData)
    {
        UpdateDescription();

        Debug.Log("selected button worldPos: " + rectTransform.position.y);
        Debug.Log("selected button localPos: " + rectTransform.localPosition.y);

        partyUI.SnapTo(rectTransform, itemNumber);
    }

    public void CharacterSwitchPressed()
    {
        if (partyUI.charToSwitch1 == -1)
        {
            partyUI.charToSwitch1 = itemNumber;
            partyUI.characterSwitchPopup.SetActive(true);
            partyUI.characterSwitchText.text = "Select character to switch places with " + partyMember.characterNameEn + "\n\nSelect " + partyMember.characterNameEn + " again to exit";
            GameManager.gm.dungeonPlayer.buttonSelectOnDecline = button.gameObject;
        }
        else
        {
            if (partyUI.charToSwitch1 == itemNumber)
            {
                partyUI.charToSwitch1 = -1;
                partyUI.characterSwitchPopup.SetActive(false);
                GameManager.gm.dungeonPlayer.buttonSelectOnDecline = partyUI.sidebarPartyButton;
            }
            else
            {
                GameManager.gm.PartySwap(partyUI.charToSwitch1, itemNumber);
                partyUI.CreateDisplay();
                GameManager.gm.dungeonPlayer.buttonSelectOnDecline = partyUI.sidebarPartyButton;

                GameManager.gm.dungeonPlayer.eventSystem.SetSelectedGameObject(partyUI.firstButton.gameObject);
            }
        }
    }



    public void UpdateGraphic()
    {
        nameText.text = partyMember.characterNameEn;
        levelText.text = "Lv." + partyMember.LV;

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

        if (partyMember.currentHP <= 0)
        {
            isDefeatedObject.SetActive(true);
        }
        else
        {
            isDefeatedObject.SetActive(false);
        }

    }

    public void UpdateDescription()
    {
        partyUI.nameText.text = partyMember.characterNameEn;

        string text1 =
        "Level: " + partyMember.LV +
        "\nEXP: " + partyMember.EXP + "/1000" +
        "\nHP: " + partyMember.currentHP + "/" + partyMember.maxHP +
        "\nMP: " + partyMember.currentMP + "/" + partyMember.maxMP +
        "\nATK: " + partyMember.ATK +
        "\nINT: " + partyMember.INT +
        "\nDEF: " + partyMember.DEF +
        "\nRES: " + partyMember.RES +
        "\nAGL: " + partyMember.AGL +
        "\nACR: " + partyMember.ACR +
        "\nSPD: " + partyMember.SPD +
        "\nEDR: " + partyMember.EDR +
        "\n\nBody Parts: \n"
        ;
        foreach(BodyPart bp in partyMember.bodyParts)
        {
            text1 = text1 + "[" + bp.bodyPartName + "] ";
        }
        partyUI.statsText1.text = text1;



        String text2 = "Weaknesses: \n";
        foreach (EquipmentType weakness in partyMember.weaknesses)
        {
            String w = "";
            if (weakness == EquipmentType.Sword) { text2 = text2 + "[Sword] "; }
            else if (weakness == EquipmentType.Lance) { text2 = text2 + "[Lance] "; }
            else if (weakness == EquipmentType.Bludgeon) { text2 = text2 + "[Bludgeon] "; }
            else if (weakness == EquipmentType.Fists) { text2 = text2 + "[Fists] "; }
            else if (weakness == EquipmentType.Bow) { text2 = text2 + "[Bow] "; }
            else if (weakness == EquipmentType.Fire) { text2 = text2 + "[Fire] "; }
            else if (weakness == EquipmentType.Water) { text2 = text2 + "[Water] "; }
            else if (weakness == EquipmentType.Ice) { text2 = text2 + "[Ice] "; }
            else if (weakness == EquipmentType.Bio) { text2 = text2 + "[Bio] "; }
            else if (weakness == EquipmentType.Light) { text2 = text2 + "[Light] "; }
            else if (weakness == EquipmentType.Dark) { text2 = text2 + "[Dark] "; }
            else if (weakness == EquipmentType.Gun) { text2 = text2 + "[Gun] "; }
        }

        text2 = text2 + "\n\nCan Equip: \n";
        foreach (EquipmentType weakness in partyMember.equippable)
        {
            String w = "";
            if (weakness == EquipmentType.Sword) { text2 = text2 + "[Sword] "; }
            else if (weakness == EquipmentType.Lance) { text2 = text2 + "[Lance] "; }
            else if (weakness == EquipmentType.Bludgeon) { text2 = text2 + "[Bludgeon] "; }
            else if (weakness == EquipmentType.Fists) { text2 = text2 + "[Fists] "; }
            else if (weakness == EquipmentType.Bow) { text2 = text2 + "[Bow] "; }
            else if (weakness == EquipmentType.Fire) { text2 = text2 + "[Fire] "; }
            else if (weakness == EquipmentType.Water) { text2 = text2 + "[Water] "; }
            else if (weakness == EquipmentType.Ice) { text2 = text2 + "[Ice] "; }
            else if (weakness == EquipmentType.Bio) { text2 = text2 + "[Bio] "; }
            else if (weakness == EquipmentType.Light) { text2 = text2 + "[Light] "; }
            else if (weakness == EquipmentType.Dark) { text2 = text2 + "[Dark] "; }
            else if (weakness == EquipmentType.Gun) { text2 = text2 + "[Gun] "; }
        }

        partyUI.statsText2.text = text2;

        partyUI.charSprite.sprite = partyMember.sprite;
    }

}
