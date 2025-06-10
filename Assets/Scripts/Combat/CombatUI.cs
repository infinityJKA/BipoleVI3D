using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class CombatUI : MonoBehaviour
{

    public DungeonUI ui;

    [Header("Combat Sprites")]
    public CombatEnemyDisplay[] combatSprites;

    [Header("Main Box")]
    public GameObject mainBox, mainBox_FirstButton;
    public TMP_Text mainBox_Text;
    public Image mainBox_Portrait;

    [Header("Attack Order")]
    public Image[] orderIcons;

    [Header("Calculation Stuff (don't edit its automatic)")]
    public List<PartyMember> battlers;
    private GameManager gm;

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

                for (int x = 0; x < battlers.Count; x++){ // check if faster than other battlers and insert correctly
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
                if (!placed) { // add at the end otherwise
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

            for (int x = 0; x < battlers.Count; x++){ // check if faster than other battlers and insert correctly
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
            if (!placed) { // add at the end otherwise
                battlers.Add(current);
            }
        
        }


        UpdateOrderGraphic();
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

}
