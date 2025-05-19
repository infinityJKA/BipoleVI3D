using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V20;

public class PartyInMenuUI : MonoBehaviour
{
    public GameObject parentSpawnUnder, characterSwitchPopup, sidebarPartyButton;
    public PartyInMenuUIButton prefab;
    public PartyInMenuUIButton firstButton, previousButton;
    public TMP_Text nameText, statsText1, statsText2, characterSwitchText;
    public Image charSprite;


    public ScrollRect scrollRect;
    public RectTransform content, viewport;
    public List<RectTransform> rectTransforms;
    public float topPos, bottomPos, offSet;
    public int itemCount;
    private RectTransform oldRect;
    private Vector2 originalPos;
    public int offsetGoingUp,offsetGoingDown;

    public int charToSwitch1;

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

    public void SnapTo(RectTransform target, int index)
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

    public void CreateDisplay()
    {
        characterSwitchPopup.SetActive(false);
        charToSwitch1 = -1;

        // destroy old children first
        foreach (Transform child in parentSpawnUnder.transform)
        {
            Destroy(child.gameObject);
        }

        itemsDisplayed.Clear();

        List<PartyMember> party = GameManager.gm.partyMembers;

        rectTransforms = new List<RectTransform>();
        itemCount = 0;

        oldRect = null;

        firstButton = null;
        bool isFirst = true;
        // spawn new display
        for (int i = 0; i < GameManager.gm.partyMembers.Count; i++)
        {
            // create the UI display for the item
            PartyInMenuUIButton pimui = Instantiate(prefab, parentSpawnUnder.transform);
            pimui.partyMember = party[i];
            
            pimui.partyUI = this;

            rectTransforms.Add(pimui.rectTransform);
            pimui.itemNumber = itemCount;
            pimui.UpdateGraphic();
            itemCount++;


            var nav = pimui.button.navigation;

            // navigation stuff
            if (isFirst)
            {
                firstButton = pimui;
                isFirst = false;
            }
            else
            {
                nav.selectOnDown = firstButton.button; // this will be overriden unless this is the final button
                nav.selectOnUp = previousButton.button;
                pimui.button.navigation = nav;

                // override the previous selectOnDown to be this button
                var pNav = previousButton.button.navigation;
                pNav.selectOnDown = pimui.button;
                previousButton.button.navigation = pNav;

            }

            // set this button to the previous before moving forwards in the loop
            previousButton = pimui;
        }
        

        // set first button's navigation up to the last button
        if (firstButton != null)
        {
            Navigation nav = firstButton.button.navigation;
            nav.selectOnUp = parentSpawnUnder.transform.GetChild(parentSpawnUnder.transform.childCount - 1).GetComponent<PartyInMenuUIButton>().button;
            firstButton.button.navigation = nav;
        }


        SendToDisplayButtons();
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
