using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ItemUIButton : MonoBehaviour, ISelectHandler
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


        // itemsUI.topPos = -itemsUI.rectTransforms[itemNumber].anchoredPosition.y + itemsUI.offSet;
        // itemsUI.bottomPos = itemsUI.viewport.rect.height - itemsUI.rectTransforms[itemNumber].anchoredPosition.y - itemsUI.offSet;

        // bool inView = RectTransformUtility.RectangleContainsScreenPoint(itemsUI.viewport, itemsUI.rectTransforms[itemNumber].position);
        // if (!inView)
        // {
        //     if (itemsUI.topPos > itemsUI.content.anchoredPosition.y)
        //     {
        //         itemsUI.content.anchoredPosition = new Vector2(0, itemsUI.topPos);
        //     }
        //     else
        //     {
        //         itemsUI.content.anchoredPosition = new Vector2(0, itemsUI.bottomPos);
        //     }
        // }

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
