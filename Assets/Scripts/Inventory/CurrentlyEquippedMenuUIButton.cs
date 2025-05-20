using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System;

// this is the button used to select a party member to equip to
public class CurrentlyEquippedMenuUIButton : MonoBehaviour, ISelectHandler
{
    public TMP_Text nameText;
    public Button button;
    public EquipInMenuUI equipUI;
    public int itemNumber; // index of this button

    public void OnSelect(BaseEventData eventData)
    {
        UpdateDescription();
    }

    public void UpdateGraphic()
    {
        
    }

    public void UpdateDescription()
    {

    }

}
