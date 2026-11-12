using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
    [HideInInspector] public GameManager gm;

    public GameObject parentSpawnUnder;
    public ScrollSnapTo scrollSnapTo;
    public ShopItemButton itemPrefab;
    private ShopItemButton firstButton, previousButton;
    public TMP_Text nameText, descriptionText, fundsText, vendorStatusText;

    public int itemCount;

    private void OnEnable()
    {
        gm = GameManager.gm;
    }

    public void CreateDisplay(string str, bool isBuying)
    {
        ItemType itype = ItemType.Consumable;
        if (str == "Equipment") itype = ItemType.Equipment;
        else if (str == "Material") itype = ItemType.Material;
        else if (str == "Book") itype = ItemType.Book;
        else if (str == "Food") itype = ItemType.Food;
        else if (str == "Drug") itype = ItemType.Drug;

        // destroy old children first
        foreach (Transform child in parentSpawnUnder.transform)
        {
            Destroy(child.gameObject);
        }

        List<InventorySlot> items;

        if (isBuying)
        {
            items = gm.currentShop.itemsForSale;
        }
        else
        {
            items = gm.inventory.Container;
        }

        itemCount = 0;

        int gol = 0;
        int ryy = 0;
        int dwey = 0;
        int skia = 0;


        firstButton = null;
        bool isFirst = true;
        
        // spawn new display
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item.itemType == itype)
            {
                // create the UI display for the item
                ShopItemButton sib = Instantiate(itemPrefab, parentSpawnUnder.transform);
                sib.shopMenu = this;
                sib.InventorySlot = items[i];
                sib.isBuying = isBuying;

                sib.UpdateGraphic();

                itemCount++;

                var nav = sib.button.navigation;

                // navigation stuff
                if (isFirst)
                {
                    firstButton = sib;
                    isFirst = false;
                }
                else
                {
                    nav.selectOnDown = firstButton.button; // this will be overriden unless this is the final button
                    nav.selectOnUp = previousButton.button;
                    sib.button.navigation = nav;

                    // override the previous selectOnDown to be this button
                    var pNav = previousButton.button.navigation;
                    pNav.selectOnDown = sib.button;
                    previousButton.button.navigation = pNav;

                }

                // set this button to the previous before moving forwards in the loop
                previousButton = sib;
            }
        }


        // set other info
        string vString = "Vendor is NOT BUYING";
        float vAmount = gm.currentShopMeta.vendorWalletCurrent / gm.currentShopMeta.vendorWallet;
        
        if(vAmount >= 1.5f)
        {
            vString = "Vendor funds are RECREMENTITIOUS";
        }
        else if (vAmount >= 0.90f)
        {
            vString = "Vendor funds are REPLETE";
        }
        else if (vAmount >= 0.75f)
        {
            vString = "Vendor funds are AMPLE";
        }
        else if (vAmount >= 0.5f)
        {
            vString = "Vendor funds are MODERATE";
        }
        else if (vAmount >= 0.25f)
        {
            vString = "Vendor funds are DESTITUTE";
        }

        vendorStatusText.text = vString;


        // set first button's navigation up to the last button
        if (firstButton != null)
        {
            Navigation nav = firstButton.button.navigation;
            nav.selectOnUp = parentSpawnUnder.transform.GetChild(parentSpawnUnder.transform.childCount - 1).GetComponent<ShopItemButton>().button;
            firstButton.button.navigation = nav;
        }


        //SendToDisplayButtons();
    }
}
