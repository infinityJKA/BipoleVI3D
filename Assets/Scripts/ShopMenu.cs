using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
    [HideInInspector] public GameManager gm;

    public GameObject buySellButton, exitButton;
    public TMP_Text buySellButtonText;
    public bool isBuying = true;

    public GameObject parentSpawnUnder;
    public ScrollSnapTo scrollSnapTo;
    public ShopItemButton itemPrefab;
    private ShopItemButton firstButton, previousButton;
    public TMP_Text nameText, descriptionText, fundsText, vendorStatusText;

    public InventorySlot selectedItem;
    public GameObject buySellParent;
    public TMP_Text buySellText;
    public GameObject buySellFirstSelect;
    [HideInInspector] public int buySellCount;

    public Image vendorPortrait;

    [HideInInspector] public int itemCount;

    private void OnEnable()
    {
        gm = GameManager.gm;
    }

    public void UpdateBuySellPopup()
    {
        string s;
        if (isBuying) s = "BUYING";
        else s = "SELLING";

        s = s + " x" + itemCount + " for ";

        int totalPrice;
        if(isBuying) totalPrice = (int) ( (selectedItem.item.value * gm.currentShop.buyMultiplier) * buySellCount );
        else totalPrice = (int) ( (selectedItem.item.value * gm.currentShop.sellMultiplier) * buySellCount );

        string c;
        if(gm.currentShop.currencyType == CurrencyType.Bieace) c = "Gole";
        else if(gm.currentShop.currencyType == CurrencyType.Bievil) c = "Dwey";
        else if(gm.currentShop.currencyType == CurrencyType.Farle) c = "Ryy";
        else c = "Skĭā́";

        s = s + totalPrice + " " + c;

        buySellParent.SetActive(true);
    }

    public void CreateDisplay(string str)
    {
        ItemType itype = ItemType.NULL;
        if (str == "Equipment") itype = ItemType.Equipment;
        else if (str == "Consumable") itype = ItemType.Consumable;
        else if (str == "Material") itype = ItemType.Material;
        else if (str == "Book") itype = ItemType.Book;
        else if (str == "Food") itype = ItemType.Food;
        else if (str == "Drug") itype = ItemType.Drug;

        // destroy old children first
        foreach (Transform child in parentSpawnUnder.transform)
        {
            Destroy(child.gameObject);
        }

        if(str != "NONE"){
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

        if(str != "NONE"){
            // set first button's navigation up to the last button
            if (firstButton != null)
            {
                Navigation nav = firstButton.button.navigation;
                nav.selectOnUp = parentSpawnUnder.transform.GetChild(parentSpawnUnder.transform.childCount - 1).GetComponent<ShopItemButton>().button;
                firstButton.button.navigation = nav;
            }

        }
        //SendToDisplayButtons();
    }

    public void GenerateMoneyText()
    {
        string cur = "Gole";
        if(gm.currentShop.currencyType == CurrencyType.Bievil) cur = "Dwey";
        else if(gm.currentShop.currencyType == CurrencyType.Bievil) cur = "Ryy";
        else if(gm.currentShop.currencyType == CurrencyType.Bievil) cur = "Skĭā́";

        fundsText.text = gm.GetTotalMoney(gm.currentShop.currencyType, false) + " " + cur +" total\n"+
         gm.GetTotalMoney(gm.currentShop.currencyType, true) + " " +cur +" usable";
    }

    public void InitializeMenu()
    {
        isBuying = true;
        UpdateBuySellButton();
        GenerateMoneyText();

        vendorPortrait.sprite = gm.currentShop.vendorPortrait; 

        foreach (Transform child in parentSpawnUnder.transform)
        {
            Destroy(child.gameObject);
        }

    }

    
    public void UpdateBuySellButton()
    {
        isBuying = !isBuying;

        if (isBuying)
        {
            buySellButtonText.text = "BUYING";
            buySellButtonText.color = Color.yellow;
        }
        else
        {
            buySellButtonText.text = "SELLING";
            buySellButtonText.color = Color.green;
        }
    }

    public void ClickedBuySellButton()
    {
        if(isBuying) isBuying = false;
        else isBuying = true;
        UpdateBuySellButton();
    }

    public void ClickedExitButton()
    {
        gameObject.SetActive(false);
        gm.dungeonPlayer.ui.dialogueBox.SetActive(true);
        gm.dungeonPlayer.inputState = DungeonInputControlState.Dialogue;
        gm.dungeonPlayer.ProgressDialogue();
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
