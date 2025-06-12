using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class TargetSelectButton : MonoBehaviour, ISelectHandler
{
    public CombatUI combatUI;
    public PartyMember battler;
    public TMP_Text nameText;

    public void OnSelect(BaseEventData eventData)
    {
        if (battler.isEnemy) // show selection arrow for targeting an enemy
        {
            foreach (PartyMember enem in combatUI.gm.enemies)
            {
                if (enem != battler)
                {
                    enem.display.selectIcon.SetActive(false);
                }
                else
                {
                    enem.display.selectIcon.SetActive(true);
                }
            }
            
        }   
    }


    public void OnClick()
    {

    }


}
