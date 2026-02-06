using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.Localization;

public class ItemsUI : MonoBehaviour
{
    public GameObject parentSpawnUnder;
    public ItemUIButton prefab;
    private ItemUIButton firstButton, previousButton;
    public TMP_Text nameText, descriptionText;


    public ScrollRect scrollRect;
    public RectTransform content, viewport;
    public List<RectTransform> rectTransforms;
    public float topPos, bottomPos, offSet;
    public int itemCount;
    private RectTransform oldRect;
    private Vector2 originalPos;
    public int offsetGoingUp,offsetGoingDown;

    Dictionary<InventorySlot, ItemUIButton> itemsDisplayed = new Dictionary<InventorySlot, ItemUIButton>();

    void OnEnable()
    {
        originalPos = content.anchoredPosition;
    }

    public void ResetButtonSnap()
    {
        content.anchoredPosition = originalPos;
        oldRect = null;
    }

    public void NewerSnapTo(RectTransform target, int index)
    {
        Vector2 offsetVector = new Vector2(0, 0);
        RectTransform rect = rectTransforms[index];
        Vector2 v = rect.position;

        bool inView = RectTransformUtility.RectangleContainsScreenPoint(viewport, v);

        if (!inView)
        {
            if (oldRect != null)
            {
                if (oldRect.localPosition.y < rect.localPosition.y) // if old position was lower than new pos
                {
                    Debug.Log("offsetGoingUp");
                    offsetVector = new Vector2(0, offsetGoingUp);
                }
                else if (oldRect.localPosition.y > rect.localPosition.y)
                {
                    Debug.Log("offsetGoingDown");
                    offsetVector = new Vector2(0, offsetGoingDown);
                }

                Canvas.ForceUpdateCanvases();
                Vector2 targetPosition = (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
                Vector2 contentPosition = (Vector2)scrollRect.transform.InverseTransformPoint(content.position);
                content.anchoredPosition = contentPosition - targetPosition + offsetVector;
            }
        }
        oldRect = rect;
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

        rectTransforms = new List<RectTransform>();
        itemCount = 0;

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

                rectTransforms.Add(iui.rectTransform);
                iui.itemNumber = itemCount;
                itemCount++;


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
