using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System;

public class MenuSideButton : MonoBehaviour, ISelectHandler
{
    public MenuUIMain menuUI;
    public GameObject screen;
    public String buttonType;
    public ItemsUI itemsUI;
    public EquipInMenuUI equipUI;
    public PartyInMenuUI partyUI;

    public void OnSelect(BaseEventData eventData)
    {
        menuUI.DisableAllButThis(screen);
        GameManager.gm.dungeonPlayer.buttonSelectOnDecline = menuUI.returnButton.gameObject;

        menuUI.equipUI.ResetButtonSnapParty();
        //menuUI.partyUI.ResetButtonSnap();

        if (buttonType == "Items")
        {
            itemsUI.CreateDisplay("Consumable");
        }
        else if (buttonType == "Equip")
        {
            equipUI.CreateDisplayParty();
        }
        else if (buttonType == "Party")
        {
            partyUI.CreateDisplay();
        }

    }


}
