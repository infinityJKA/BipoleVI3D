using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using UnityEngine.UI;
using UnityEngine.EventSystems; 
using Unity.VisualScripting;

public class InventoryCoinButton : MonoBehaviour, ISelectHandler
{
    public TMP_Text nameText, countText;
    public Image sprite;
    public InventorySlot itemInfo;
    public Button button;
    public ItemsUI itemsUI;
    public RectTransform rectTransform;
    public int itemNumber;

    public void OnSelect(BaseEventData eventData)
    {
        UpdateDescription();

        Debug.Log("selected button worldPos: " + rectTransform.position.y);
        Debug.Log("selected button localPos: " + rectTransform.localPosition.y);

        itemsUI.NewerSnapTo(rectTransform, itemNumber);

    }

    public void UpdateGraphic()
    {
        nameText.text = itemInfo.item.itemName;
        countText.text = "x" + itemInfo.amount;
    }

    public void UpdateDescription()
    {
        itemsUI.nameText.text = itemInfo.item.itemName;
        itemsUI.descriptionText.text =
        "Base Value: " + itemInfo.item.value + "\n" +
        itemInfo.item.itemDescription + "\n\n" +
        itemInfo.item.equipmentAction.actionDescription;
    }

}
