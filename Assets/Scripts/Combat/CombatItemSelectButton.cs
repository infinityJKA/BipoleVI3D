using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Localization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombatItemSelectButton : MonoBehaviour, ISelectHandler
{
    public CombatUI combatUI;
    public InventorySlot inventorySlot;
    public TMP_Text nameText;
    public RectTransform rectTransform;
    public int itemNumber;

    public void OnClick()
    {
        combatUI.gm.itemToUse = inventorySlot;
        combatUI.usingConsumable = true;
        combatUI.itemsBox.SetActive(false);
        combatUI.PlayerCombatAction();

    }

    public void OnSelect(BaseEventData eventData)
    {
        combatUI.itemDescription.text = inventorySlot.item.itemName + "\n" + inventorySlot.item.itemDescription;

        Debug.Log("selected button worldPos: " + rectTransform.position.y);
        Debug.Log("selected button localPos: " + rectTransform.localPosition.y);

        combatUI.itemsSnap.SnapTo(rectTransform);

    }


}
