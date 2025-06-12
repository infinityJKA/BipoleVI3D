using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;

public class CombatUI : MonoBehaviour
{

    public DungeonUI ui;

    [Header("Combat Sprites")]
    public CombatEnemyDisplay[] combatSprites;

    [Header("Main Box")]
    public GameObject mainBox;
    public GameObject mainBox_FirstButton;
    public TMP_Text mainBox_Text;
    public Image mainBox_Portrait;

    [Header("Act Box")]
    public GameObject actBox;
    public GameObject actDescriptionBox;
    public TMP_Text actDescriptionText;
    public GameObject actGrid;
    public ActionSelectButton actionSelectButtonPrefab;
    public GameObject lastSelectedAction;

    [Header("Target Select Box")]
    public GameObject targetSelectBox;
    public GameObject targetSelectGrid;
    public TargetSelectButton targetSelectButtonPrefab;

    [Header("Attack Order")]
    public Image[] orderIcons;

    [Header("Calculation Stuff (don't edit its automatic)")]
    public List<PartyMember> battlers; // updated to be in correct SPD order
    public GameManager gm;
    public List<EquipmentAction> validActions;

    public void InitializeBattleOrder()
    {
        gm = GameManager.gm;

        // reset list of battlers
        battlers.Clear();

        battlers.Add(gm.partyMembers[0]);

        // add each party member
        for (int i = 1; i < 4; i++) // first 4 are used for combat
        {
            if (gm.partyMembers.Count > i) // make sure party member exists
            {
                PartyMember current = gm.partyMembers[i];
                bool placed = false;

                int currentSPD = current.CalculateStat("SPD");

                for (int x = 0; x < battlers.Count; x++)
                { // check if faster than other battlers and insert correctly
                    if (!placed)
                    {
                        if (currentSPD > battlers[x].CalculateStat("SPD"))
                        {
                            Debug.Log(current.characterNameEn + " is faster than " + battlers[x].characterNameEn);
                            battlers.Insert(x, current);
                            placed = true;
                        }
                        else Debug.Log(current.characterNameEn + " is slower than " + battlers[x].characterNameEn);
                    }
                }
                if (!placed)
                { // add at the end otherwise
                    battlers.Add(current);
                }
            }
        }

        // add each enemy
        for (int i = 0; i < gm.enemies.Count; i++) // first 4 are used for combat
        {
            PartyMember current = gm.enemies[i];
            bool placed = false;

            int currentSPD = current.CalculateStat("SPD");

            for (int x = 0; x < battlers.Count; x++)
            { // check if faster than other battlers and insert correctly
                if (!placed)
                {
                    if (currentSPD > battlers[x].CalculateStat("SPD"))
                    {
                        Debug.Log(current.characterNameEn + " is faster than " + battlers[x].characterNameEn);
                        battlers.Insert(x, current);
                        placed = true;
                    }
                    else Debug.Log(current.characterNameEn + " is slower than " + battlers[x].characterNameEn);
                }
            }
            if (!placed)
            { // add at the end otherwise
                battlers.Add(current);
            }

        }


        UpdateOrderGraphic();
    }

    public void PlayerCombatAction()
    {
        actBox.SetActive(false);
        actDescriptionBox.SetActive(false);

        EquipmentAction ea = gm.currentAction;
        if (ea.targetType == TargetType.Self || ea.targetType == TargetType.BothParties || ea.targetType == TargetType.EnemyParty || ea.targetType == TargetType.Party)
        {
            // start the action here
        }
        else // generate a list of selectable targets
        {
            Debug.Log("Need to manually select a target for this aciton");

            List<PartyMember> chars = gm.enemies; // default to current enemies

            if (ea.targetType == TargetType.OneAlly) // create a list of valid allies if targeting ally
            {
                chars.Clear();
                for (int i = 0; i < 4; i++)
                {
                    if (i < gm.partyMembers.Count)
                    {
                        chars.Add(gm.partyMembers[i]);
                    }
                }
            }

            GameObject firstSelected = null;

            foreach (Transform child in targetSelectGrid.transform) Destroy(child.gameObject); // destroy old buttons

            for (int i = 0; i < chars.Count; i++) // generate the buttons in the gird
            {
                TargetSelectButton tsb = Instantiate(targetSelectButtonPrefab, targetSelectBox.transform.position, targetSelectBox.transform.rotation, targetSelectGrid.transform);
                if (firstSelected == null) firstSelected = tsb.gameObject;
                tsb.battler = chars[i];
                tsb.combatUI = this;
                tsb.nameText.text = tsb.battler.characterNameEn;
            }

            targetSelectBox.SetActive(true);
            gm.dungeonPlayer.eventSystem.SetSelectedGameObject(firstSelected);

            gm.dungeonPlayer.combatReturnTo = CombatReturnTo.ActSelect;


        }
    }

    public void HideMenusForDialogue()
    {
        mainBox.SetActive(false);
        actBox.SetActive(false);
        actDescriptionBox.SetActive(false);
        targetSelectBox.SetActive(false);

    }

    public void UpdateOrderGraphic()
    {
        for (int i = 0; i < orderIcons.Length; i++)
        {
            if (battlers.Count > i) // if battler exists
            {
                orderIcons[i].gameObject.SetActive(true);
                orderIcons[i].sprite = battlers[i].head;
            }
            else  // if battler doesn't exist
            {
                orderIcons[i].gameObject.SetActive(false);
            }
        }
    }

    public void ActSelected()
    {
        mainBox.SetActive(false);
        validActions.Clear(); // clear old action list
        foreach (Transform child in actGrid.transform) Destroy(child.gameObject); // destroy old buttons

        ItemObject[] equipment = gm.currentBattler.currentlyEquipped;

        foreach (ItemObject act in equipment) // create a list of each equipment that has a usable action
        {
            if (act != null)
            {
                if (act.equipmentEffect == EquipmentEffect.Action)
                {
                    validActions.Add(act.equipmentAction);
                }
            }
        }

        if (validActions.Count == 0)
        {
            actDescriptionText.text = "No valid actions equipped.";
        }
        else // iterate through each valid action and create buttons for them
        {
            GameObject firstSelected = null;

            for (int i = 0; i < validActions.Count; i++)
            {
                ActionSelectButton asb = Instantiate(actionSelectButtonPrefab, actBox.transform.position, actBox.transform.rotation, actGrid.transform);
                if (firstSelected == null) firstSelected = asb.gameObject;
                asb.action = validActions[i];
                asb.combatUI = this;
                asb.buttonText.text = asb.action.actionName;
                string cost = "";
                asb.costText.text = "FREE";
                if (asb.action.setCost == false) cost = "%";
                if (asb.action.costHP != 0) asb.costText.text = asb.action.costHP + cost + " HP";
                if (asb.action.costMP != 0) asb.costText.text = asb.action.costHP + cost + " MP";
            }

            actBox.SetActive(true);
            gm.dungeonPlayer.eventSystem.SetSelectedGameObject(firstSelected);
        }

        actDescriptionBox.SetActive(true);
        gm.dungeonPlayer.combatReturnTo = CombatReturnTo.Main;
    }

}
