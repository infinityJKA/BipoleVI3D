using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ActionSelectButton : MonoBehaviour, ISelectHandler
{
    public CombatUI combatUI;
    public EquipmentAction action;
    public TMP_Text buttonText, costText;

    public void OnSelect(BaseEventData eventData)
    {
        combatUI.actDescriptionText.text = action.actionDescription;
        combatUI.lastSelectedAction = this.gameObject;
    }

    public void OnClick()
    {
        combatUI.gm.currentAction = action;
        combatUI.PlayerCombatAction();

    }


}
