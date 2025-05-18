using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemUIButton : MonoBehaviour
{
    public TMP_Text nameText, countText;
    public Image sprite;
    public InventorySlot itemInfo;
    public Button button;

    public void UpdateGraphic()
    {
        nameText.text = itemInfo.item.itemName;
        countText.text = "x" + itemInfo.amount;
    }

}
