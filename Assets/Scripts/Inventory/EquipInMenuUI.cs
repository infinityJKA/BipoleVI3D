using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Linq;

public class EquipInMenuUI : MonoBehaviour
{
    public GameObject parentSpawnUnderParty, parentSpawnUnderEquipment, sideBarEquipButton;
    public EquipToPartyInMenuUIButton prefabParty;
    public EquipToPartyInMenuUIButton firstButtonParty, firstButtonEquip, previousButtonParty,previousButtonEquip;
    public TMP_Text equipStats, characterStats;

    public ScrollRect scrollRectParty;
    public RectTransform contentParty, viewportParty, contentEquip, viewportEquip;
    public List<RectTransform> rectTransforms;
    public float topPosParty, bottomPosParty, offSetParty;
    public int itemCountParty, itemCountEquip;
    private RectTransform oldRectParty, oldRectEquip;
    private Vector2 originalPosParty, originalPosEquip;
    public int offsetGoingUp,offsetGoingDown;

    public PartyMember selectedCharacter;
    public CurrentlyEquippedMenuUIButton[] equipmentButtons;

    Dictionary<InventorySlot, ItemUIButton> itemsDisplayed = new Dictionary<InventorySlot, ItemUIButton>();

    void OnEnable()
    {
        originalPosParty = contentParty.anchoredPosition;
        //originalPosEquip = contentEquip.anchoredPosition;
    }

    public void ResetButtonSnapParty()
    {
        contentParty.anchoredPosition = originalPosParty;
        oldRectParty = null;
    }

    public void SnapToParty(RectTransform target, int index)
    {
        Vector2 offsetVector = new Vector2(0, 0);
        RectTransform rect = rectTransforms[index];
        Vector2 v = rect.position;

        bool inView = RectTransformUtility.RectangleContainsScreenPoint(viewportParty, v);

        if (!inView)
        {
            if (oldRectParty != null)
            {
                if (oldRectParty.localPosition.y < rect.localPosition.y) // if old position was lower than new pos
                {
                    Debug.Log("offsetGoingUp");
                    offsetVector = new Vector2(0, offsetGoingUp);
                }
                else if (oldRectParty.localPosition.y > rect.localPosition.y)
                {
                    Debug.Log("offsetGoingDown");
                    offsetVector = new Vector2(0, offsetGoingDown);
                }

                Canvas.ForceUpdateCanvases();
                Vector2 targetPosition = (Vector2)scrollRectParty.transform.InverseTransformPoint(target.position);
                Vector2 contentPosition = (Vector2)scrollRectParty.transform.InverseTransformPoint(contentParty.position);
                contentParty.anchoredPosition = contentPosition - targetPosition + offsetVector;
            }
        }
        oldRectParty = rect;
    }

    public void CreateDisplayParty()
    {
        // destroy old children first
        foreach (Transform child in parentSpawnUnderParty.transform)
        {
            Destroy(child.gameObject);
        }

        itemsDisplayed.Clear();

        List<PartyMember> party = GameManager.gm.partyMembers;

        rectTransforms = new List<RectTransform>();
        itemCountParty = 0;

        oldRectParty= null;

        firstButtonParty = null;
        bool isFirst = true;
        // spawn new display
        for (int i = 0; i < GameManager.gm.partyMembers.Count; i++)
        {
            Debug.Log("Party member found");

            // create the UI display for the item
            EquipToPartyInMenuUIButton eimui = Instantiate(prefabParty, parentSpawnUnderParty.transform);
            eimui.partyMember = party[i];
            
            eimui.equipUI = this;

            rectTransforms.Add(eimui.rectTransform);
            eimui.itemNumber = itemCountParty;
            eimui.UpdateGraphic();
            itemCountParty++;


            var nav = eimui.button.navigation;

            // navigation stuff
            if (isFirst)
            {
                firstButtonParty = eimui;
                isFirst = false;
            }
            else
            {
                nav.selectOnDown = firstButtonParty.button; // this will be overriden unless this is the final button
                nav.selectOnUp = previousButtonParty.button;
                eimui.button.navigation = nav;

                // override the previous selectOnDown to be this button
                var pNav = previousButtonParty.button.navigation;
                pNav.selectOnDown = eimui.button;
                previousButtonParty.button.navigation = pNav;

            }

            // set this button to the previous before moving forwards in the loop
            previousButtonParty = eimui;
        }
        

        // set first button's navigation up to the last button
        if (firstButtonParty != null)
        {
            Navigation nav = firstButtonParty.button.navigation;
            nav.selectOnUp = parentSpawnUnderParty.transform.GetChild(parentSpawnUnderParty.transform.childCount - 1).GetComponent<EquipToPartyInMenuUIButton>().button;
            firstButtonParty.button.navigation = nav;
        }


        SendToDisplayButtonsParty();
    }

    public void CreateCurrentlyEquippedDisplay()
    {
        for (int i = 0; i < 4; i++)
        {
            if (selectedCharacter.currentlyEquipped[i] == null)
            {
                equipmentButtons[i].nameText.text = i + ". [empty]";
            }
            else
            {
                equipmentButtons[i].nameText.text = i + ". " + selectedCharacter.currentlyEquipped[i].itemName;
            }
            equipmentButtons[i].itemNumber = i;
        }
    }

    public void SendToDisplayButtonsParty()
    {
        // make sure there is at least one item of the type
        if (firstButtonParty != null)
        {
            // set decline button to the previous button
            GameManager.gm.dungeonPlayer.buttonSelectOnDecline = GameManager.gm.dungeonPlayer.eventSystem.currentSelectedGameObject;

            // move the selection to the first item
            GameManager.gm.dungeonPlayer.eventSystem.SetSelectedGameObject(firstButtonParty.button.gameObject);
        }

    }

}
