using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ItemSortUIButton : MonoBehaviour, ISelectHandler
{
    public ItemsUI itemsUI;
    public string str, nameText;
    [TextArea(3, 10)]
    public string descriptionText;
    public GameObject itemButton; // this is for when the player clicks the return button

    public void OnSelect(BaseEventData eventData)
    {
        itemsUI.CreateDisplay(str);
        itemsUI.nameText.text = nameText;
        itemsUI.descriptionText.text = descriptionText;
        GameManager.gm.dungeonPlayer.buttonSelectOnDecline = itemButton;
        itemsUI.ResetButtonSnap();
    }


}
