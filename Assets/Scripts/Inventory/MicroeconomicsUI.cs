using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.Localization;

public class MicroeconomicsUI : MonoBehaviour
{
    public GameObject parentSpawnUnder;
    public CoinInInventoryUI prefab;
    private CoinInInventoryUI firstButton, previousButton;
    public TMP_Text nameText, descriptionText, totalText1, totalText2;
      
    public int itemCount;



    void OnEnable()
    {
        //originalPos = content.anchoredPosition;
    }

    public void ResetButtonSnap()
    {
        //content.anchoredPosition = originalPos;
        //oldRect = null;
    }

    

    public void CreateDisplay(string str)
    {
        MoneyType mtype = MoneyType.Coin;
        if (str == "Check") mtype = MoneyType.Check;
        else if (str == "Stock") mtype = MoneyType.Stock;

        // destroy old children first
        foreach (Transform child in parentSpawnUnder.transform)
        {
            Destroy(child.gameObject);
        }

        inventoryObject inv = GameManager.gm.inventory;

        itemCount = 0;

        int gol = 0;
        int ryy = 0;
        int dwey = 0;
        int skia = 0;


        firstButton = null;
        bool isFirst = true;
        // spawn new display
        for (int i = 0; i < inv.money.Count; i++)
        {
            if (inv.money[i].moneyType == mtype)
            {
                // create the UI display for the item
                CoinInInventoryUI iui = Instantiate(prefab, parentSpawnUnder.transform);
                iui.microeconomicsUI = this;
                iui.moneyObject = inv.money[i];
                iui.UpdateGraphic();
                //itemsDisplayed.Add(inv.Container[i], iui);
                //iui.itemsUI = this;

                //rectTransforms.Add(iui.rectTransform);
                //iui.itemNumber = itemCount;
                itemCount++;

                if(iui.moneyObject.currencyType == CurrencyType.Bieace)
                {
                    gol += iui.moneyObject.value;
                }
                else if (iui.moneyObject.currencyType == CurrencyType.Bievil)
                {
                    dwey += iui.moneyObject.value;
                }
                else if (iui.moneyObject.currencyType == CurrencyType.Farle)
                {
                    ryy += iui.moneyObject.value;
                }
                else if (iui.moneyObject.currencyType == CurrencyType.ShadowRealm)
                {
                    skia += iui.moneyObject.value;
                }


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

        // set total money text
        totalText1.text = gol + " Gole\n" + ryy + " Ryy";
        totalText2.text = dwey + " Dwey\n" + skia + " Skĭā";

        // set first button's navigation up to the last button
        if (firstButton != null)
        {
            Navigation nav = firstButton.button.navigation;
            nav.selectOnUp = parentSpawnUnder.transform.GetChild(parentSpawnUnder.transform.childCount - 1).GetComponent<CoinInInventoryUI>().button;
            firstButton.button.navigation = nav;
        }


        //SendToDisplayButtons();
    }

    public void SendToDisplayButtons()
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

