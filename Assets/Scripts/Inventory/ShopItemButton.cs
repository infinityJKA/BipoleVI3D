using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemButton : MonoBehaviour, ISelectHandler
{

    public Button button;

    public ShopMenu shopMenu;
    public string initializeType;
    public bool isBuying;
    public TMP_Text itemNameText, itemCountText, itemValueText;
    public InventorySlot InventorySlot;

    public void UpdateGraphic()
    {
        if (isBuying) itemCountText.text = "";
        else itemCountText.text = InventorySlot.amount + "x";

        itemNameText.text = InventorySlot.item.itemName;

        int num = InventorySlot.item.value;

        if (isBuying == false)
        {
            num = Mathf.RoundToInt(num * shopMenu.gm.currentShop.sellMultiplier);
        }
        else
        {
            num = Mathf.RoundToInt(num * shopMenu.gm.currentShop.buyMultiplier);
        }
    
    }

    public void UpdateDescription()
    {
        shopMenu.nameText.text = InventorySlot.item.itemName;
        shopMenu.descriptionText.text =
        "Base value: "+ InventorySlot.item.value+"\n"
        + InventorySlot.item.itemDescription + "\n\n";

        if (InventorySlot.item.itemType == ItemType.Equipment) { 
            shopMenu.descriptionText.text += InventorySlot.item.equipmentAction.actionDescription;
        }

    }

    public void OnSelect(BaseEventData eventData)
    {
        //shopMenu.CreateDisplay(initializeType);

    }





}
