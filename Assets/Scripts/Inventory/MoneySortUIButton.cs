using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class MoneySortUIButton : MonoBehaviour, ISelectHandler
{
    public MicroeconomicsUI meUI;
    public string sortBy;
    public GameObject sideButton; // this is for when the player clicks the return button

    public void OnSelect(BaseEventData eventData)
    {
        meUI.CreateDisplay(sortBy);
        GameManager.gm.dungeonPlayer.buttonSelectOnDecline = sideButton;
        //meUI.ResetButtonSnap();
    }


}
