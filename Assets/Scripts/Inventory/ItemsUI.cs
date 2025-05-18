using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEditor.Experimental.GraphView;

public class ItemsUI : MonoBehaviour
{
    public GameObject parentSpawnUnder;
    public ItemUIButton prefab;
    public Button itemButton; // this is used for setting the decline button after returning from the inventory to top item row
    private ItemUIButton firstButton, previousButton;
    public TMP_Text nameText, descriptionText;


    Dictionary<InventorySlot, ItemUIButton> itemsDisplayed = new Dictionary<InventorySlot, ItemUIButton>();

    void Start()
    {

    }

    void Update()
    {

    }

    public void CreateDisplay(string str)
    {
        ItemType itype = ItemType.Consumable;
        if (str == "Equipment") itype = ItemType.Equipment;
        else if (str == "Key") itype = ItemType.Key;
        else if (str == "Material") itype = ItemType.Material;
        else if (str == "Book") itype = ItemType.Book;

        // destroy old children first
        foreach (Transform child in parentSpawnUnder.transform)
        {
            Destroy(child.gameObject);
        }

        itemsDisplayed.Clear();

        inventoryObject inv = GameManager.gm.inventory;

        firstButton = null;
        bool isFirst = true;
        // spawn new display
        for (int i = 0; i < inv.Container.Count; i++)
        {
            if (inv.Container[i].item.itemType == itype)
            {
                // create the UI display for the item
                ItemUIButton iui = Instantiate(prefab, parentSpawnUnder.transform);
                iui.itemInfo = inv.Container[i];
                iui.UpdateGraphic();
                itemsDisplayed.Add(inv.Container[i], iui);
                iui.itemsUI = this;

                var nav = iui.button.navigation;

                // navigation stuff
                if (isFirst)
                {
                    firstButton = iui;
                    isFirst = false;
                }
                else
                {
                    nav.selectOnDown = firstButton.button; // this will be overriden unless this is the final button
                    nav.selectOnUp = previousButton.button;
                    iui.button.navigation = nav;

                    // override the previous selectOnDown to be this button
                    var pNav = previousButton.button.navigation;
                    pNav.selectOnDown = iui.button;
                    previousButton.button.navigation = pNav;

                }

                // set this button to the previous before moving forwards in the loop
                previousButton = iui;
            }
        }

        // set first button's navigation up to the last button
        if (firstButton != null)
        {
            Navigation nav = firstButton.button.navigation;
            nav.selectOnUp = parentSpawnUnder.transform.GetChild(parentSpawnUnder.transform.childCount - 1).GetComponent<ItemUIButton>().button;
            firstButton.button.navigation = nav;
        }


        SendToDisplayButtons(str);
    }

    public void SendToDisplayButtons(String str)
    {
        // make sure there is at least one item of the type
        if (firstButton != null)
        {
            // set decline button to the previous button
            GameManager.gm.dungeonPlayer.buttonSelectOnDecline = GameManager.gm.dungeonPlayer.eventSystem.currentSelectedGameObject;

            // move the selection to the first item
            GameManager.gm.dungeonPlayer.eventSystem.SetSelectedGameObject(firstButton.button.gameObject);
        }
        
    }

}
