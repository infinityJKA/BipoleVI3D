using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Linq;
using System.Data.SqlTypes;

public class EquipInMenuUI : MonoBehaviour
{
    public GameObject parentSpawnUnderParty, parentSpawnUnderEquipment, sideBarEquipButton;
    public EquipToPartyInMenuUIButton prefabParty;
    public EquipSelectMenuUIButton prefabEquipment;
    public EquipToPartyInMenuUIButton firstButtonParty, previousButtonParty;
    public EquipSelectMenuUIButton firstButtonEquip, previousButtonEquip;
    public TMP_Text equipStats, characterStats;

    public ScrollRect scrollRectParty, scrollRectEquip;
    public RectTransform contentParty, viewportParty, contentEquip, viewportEquip;
    public List<RectTransform> rectTransforms;
    public float topPosParty, bottomPosParty, offSetParty;
    public int itemCountParty, itemCountEquip;
    private RectTransform oldRectParty, oldRectEquip;
    private Vector2 originalPosParty, originalPosEquip;
    public int offsetGoingUp, offsetGoingDown;

    public PartyMember selectedCharacter;
    public Button selectedCharacterButton;
    public int selectedEquipmentIndex;
    public CurrentlyEquippedMenuUIButton[] equipmentButtons;

    Dictionary<InventorySlot, ItemUIButton> itemsDisplayedParty = new Dictionary<InventorySlot, ItemUIButton>();
    Dictionary<InventorySlot, ItemUIButton> itemsDisplayedEquip = new Dictionary<InventorySlot, ItemUIButton>();

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

    public void ResetButtonSnapEquip()
    {
        contentEquip.anchoredPosition = originalPosEquip;
        oldRectEquip = null;
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

    public void SnapToEquip(RectTransform target, int index)
    {
        Vector2 offsetVector = new Vector2(0, 0);
        RectTransform rect = rectTransforms[index];
        Vector2 v = rect.position;

        bool inView = RectTransformUtility.RectangleContainsScreenPoint(viewportEquip, v);

        if (!inView)
        {
            if (oldRectEquip != null)
            {
                if (oldRectEquip.localPosition.y < rect.localPosition.y) // if old position was lower than new pos
                {
                    Debug.Log("offsetGoingUp");
                    offsetVector = new Vector2(0, offsetGoingUp);
                }
                else if (oldRectEquip.localPosition.y > rect.localPosition.y)
                {
                    Debug.Log("offsetGoingDown");
                    offsetVector = new Vector2(0, offsetGoingDown);
                }

                Canvas.ForceUpdateCanvases();
                Vector2 targetPosition = (Vector2)scrollRectEquip.transform.InverseTransformPoint(target.position);
                Vector2 contentPosition = (Vector2)scrollRectEquip.transform.InverseTransformPoint(contentEquip.position);
                contentEquip.anchoredPosition = contentPosition - targetPosition + offsetVector;
            }
        }
        oldRectEquip = rect;
    }

    public void CreateDisplayParty()
    {
        // destroy old children first
        foreach (Transform child in parentSpawnUnderParty.transform)
        {
            Destroy(child.gameObject);
        }

        itemsDisplayedParty.Clear();

        List<PartyMember> party = GameManager.gm.partyMembers;

        rectTransforms = new List<RectTransform>();
        itemCountParty = 0;

        oldRectParty = null;

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

    public void CreateDisplayEquip(bool sendTo)
    {
        // destroy old children first
        foreach (Transform child in parentSpawnUnderEquipment.transform)
        {
            Destroy(child.gameObject);
        }

        itemsDisplayedEquip.Clear();

        rectTransforms = new List<RectTransform>();
        itemCountEquip = 0;

        oldRectEquip = null;

        firstButtonEquip = Instantiate(prefabEquipment, parentSpawnUnderEquipment.transform); // default is set to the equip button
        firstButtonEquip.equipUI = this;
        var nav1 = firstButtonEquip.button.navigation;
        nav1.selectOnDown = firstButtonEquip.button; // this will be overriden unless this is the final button
        firstButtonEquip.button.navigation = nav1;
        rectTransforms.Add(firstButtonEquip.rectTransform);
        firstButtonEquip.UpdateDescription();
        previousButtonEquip = firstButtonEquip;
        


        // spawn new display
        foreach (InventorySlot item in GameManager.gm.inventory.Container)
        {
            if (item.item != null)
            {

                if (item.item.itemType == ItemType.Equipment)
                {

                    if (item.item.equipmentType == EquipmentType.Universal || selectedCharacter.equippable.Contains(item.item.equipmentType))
                    {
                        Debug.Log("Item member found");

                        // create the UI display for the item
                        EquipSelectMenuUIButton eimui = Instantiate(prefabEquipment, parentSpawnUnderEquipment.transform);
                        eimui.equipment = item.item;

                        eimui.equipUI = this;

                        eimui.itemQuantity = item.amount;

                        rectTransforms.Add(eimui.rectTransform);
                        eimui.itemNumber = itemCountEquip;
                        eimui.UpdateDescription();
                        itemCountEquip++;


                        var nav = eimui.button.navigation;


                        nav.selectOnDown = firstButtonEquip.button; // this will be overriden unless this is the final button
                        nav.selectOnUp = previousButtonEquip.button;
                        eimui.button.navigation = nav;

                        // override the previous selectOnDown to be this button
                        var pNav = previousButtonEquip.button.navigation;
                        pNav.selectOnDown = eimui.button;
                        previousButtonEquip.button.navigation = pNav;



                        // set this button to the previous before moving forwards in the loop
                        previousButtonEquip = eimui;
                    }
                }
            }
        }


        // set first button's navigation up to the last button
        if (firstButtonEquip != null)
        {
            Navigation nav = firstButtonEquip.button.navigation;
            nav.selectOnUp = parentSpawnUnderEquipment.transform.GetChild(parentSpawnUnderEquipment.transform.childCount - 1).GetComponent<EquipSelectMenuUIButton>().button;
            firstButtonEquip.button.navigation = nav;
        }

        if (sendTo)
        {
            SendToDisplayButtonsEquip();
        }
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
    
    public void SendToDisplayButtonsEquip()
    {
        // set decline button to the previous button
        GameManager.gm.dungeonPlayer.buttonSelectOnDecline = GameManager.gm.dungeonPlayer.eventSystem.currentSelectedGameObject;

        // move the selection to the first item
        GameManager.gm.dungeonPlayer.eventSystem.SetSelectedGameObject(firstButtonEquip.button.gameObject);

    }

    public void DescriptionText()
    {
        string str = "HP " + selectedCharacter.maxHP + "(+" + selectedCharacter.CalculateBonus("HP")[0] + "% +" + selectedCharacter.CalculateBonus("HP")[1] +
        ") MP " + selectedCharacter.maxHP + "(+" + selectedCharacter.CalculateBonus("MP")[0] + "% +" + selectedCharacter.CalculateBonus("MP")[1] +
        ")\nATK " + selectedCharacter.ATK + "(+" + selectedCharacter.CalculateBonus("ATK")[0] + "% +" + selectedCharacter.CalculateBonus("ATK")[1] +
        ") INT " + selectedCharacter.INT + "(+" + selectedCharacter.CalculateBonus("INT")[0] + "% +" + selectedCharacter.CalculateBonus("INT")[1] +
        ")\nDEF " + selectedCharacter.DEF + "(+" + selectedCharacter.CalculateBonus("DEF")[0] + "% +" + selectedCharacter.CalculateBonus("DEF")[1] +
        ") RES " + selectedCharacter.RES + "(+" + selectedCharacter.CalculateBonus("RES")[0] + "% +" + selectedCharacter.CalculateBonus("RES")[1] +
        ")\nAGL " + selectedCharacter.AGL + "(+" + selectedCharacter.CalculateBonus("AGL")[0] + "% +" + selectedCharacter.CalculateBonus("AGL")[1] +
        ") ACR " + selectedCharacter.ACR + "(+" + selectedCharacter.CalculateBonus("ACR")[0] + "% +" + selectedCharacter.CalculateBonus("ACR")[1] +
        ")\nSPD " + selectedCharacter.SPD + "(+" + selectedCharacter.CalculateBonus("SPD")[0] + "% +" + selectedCharacter.CalculateBonus("SPD")[1] +
        ") LCK " + selectedCharacter.LCK + "(+" + selectedCharacter.CalculateBonus("LCK")[0] + "% +" + selectedCharacter.CalculateBonus("LCK")[1] +
        ")\nEDR " + selectedCharacter.EDR + "(+" + selectedCharacter.CalculateBonus("EDR")[0] + "% +" + selectedCharacter.CalculateBonus("EDR")[1] + ")";

        bool hasPassive = false;
        foreach (ItemObject eqp in selectedCharacter.currentlyEquipped)
        {
            if (eqp != null)
            {
                if (eqp.equipmentEffect != EquipmentEffect.None)
                {
                    if (!hasPassive)
                    {
                        str = str + "\n";
                        hasPassive = true;
                    }
                    str = str + "<" + eqp.equipmentAction.actionName + "> ";
                }
            }
        }

        characterStats.text = str;

    }


}
