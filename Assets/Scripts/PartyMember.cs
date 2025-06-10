using UnityEngine;
using System.Linq;
using System;
using Unity.Profiling;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PartyMember", menuName = "Bipole VI/PartyMember")]
public class PartyMember : ScriptableObject
{
    public string characterNameEn = "John Partymember", characterNameJp;
    public int
    ATK, // physical strength
    INT, // magical strength
    DEF, // physical defense
    RES, // magical defense
    AGL, // makes it easier to dodge
    ACR, // makes attacks more accurate
    SPD, // makes it so you act faster/more often
    LCK, // slight influences dodging/landing attacks AND affects critical hit chance
    EDR, // how many times you can get hit in the same body part without BREAKing, doesn't increase with level
    maxHP, currentHP, // health points
    maxMP, currentMP, // used for spells and abilities
    EXP, LV, // experience points, level goes up every 1000 EXP
    VIZ; // visibility, only used in combat and reset to 100 outside of battle

    public EquipmentType[] weaknesses, equippable;
    public Sprite sprite, portrait, damagedPortrait, veryDamagedPortrait, head;
    public BodyPart[] bodyParts;

    public ItemObject[] currentlyEquipped = new ItemObject[4];
    public List<StatusCondition> statusConditions;

    [Header("Only for enemies in combat")]
    public CombatEnemyDisplay display;
    public bool isEnemy = false;

    public bool Weakness(EquipmentType e)
    {
        return weaknesses.Contains(e);
    }

    public int CalculateStat(string stat)
    {
        float total = GetUnmodifiedStat(stat);
        int[] equipBonus = CalculateBonus(stat);
        float[] conditionsEffect = CalculateStatusConditions(stat);

        // calculate equip% first, the add equip# on top of that
        total = total * (1 + (equipBonus[0] / 100)) + equipBonus[1];

        // then calculate modifs from status conditions 
        total = total * (1 + (conditionsEffect[0] / 100)) + conditionsEffect[1];

        Debug.Log("SPD ("+GetUnmodifiedStat(stat)+") = "+total+" after modifiers");

        return Convert.ToInt32(total);
    }

    public float[] CalculateStatusConditions(string stat)
    {
        float pTotal = 0; //percent
        float nTotal = 0; //numeric

        if (statusConditions.Count > 0)
        {
            foreach (StatusCondition sc in statusConditions) // iterate through each status condition
            {
                if (sc.stat == stat)
                {
                    if (sc.isPercentage)
                    {
                        pTotal += sc.amount;
                    }
                    else
                    {
                        nTotal += sc.amount;
                    }
                }
            }
        }

        float[] r = new float[2];
        r[0] = pTotal;
        r[1] = nTotal;
        return r;

    }

    public int[] CalculateBonus(string stat)
    {
        int pTotal = 0;
        int nTotal = 0;
        foreach (ItemObject eqp in currentlyEquipped)
        {
            if (eqp != null)
            {
                if (stat == "ATK")
                {
                    pTotal += eqp.pATK;
                    nTotal += eqp.nATK;
                }
                else if (stat == "INT")
                {
                    pTotal += eqp.pINT;
                    nTotal += eqp.nINT;
                }
                else if (stat == "DEF")
                {
                    pTotal += eqp.pDEF;
                    nTotal += eqp.nDEF;
                }
                else if (stat == "RES")
                {
                    pTotal += eqp.pRES;
                    nTotal += eqp.nRES;
                }
                else if (stat == "AGL")
                {
                    pTotal += eqp.pAGL;
                    nTotal += eqp.nAGL;
                }
                else if (stat == "ACR")
                {
                    pTotal += eqp.pACR;
                    nTotal += eqp.nACR;
                }
                else if (stat == "LCK")
                {
                    pTotal += eqp.pLCK;
                    nTotal += eqp.nLCK;
                }
                else if (stat == "SPD")
                {
                    pTotal += eqp.pSPD;
                    nTotal += eqp.nSPD;
                }
                else if (stat == "EDR")
                {
                    pTotal += eqp.pEDR;
                    nTotal += eqp.nEDR;
                }
                else if (stat == "HP")
                {
                    pTotal += eqp.pHP;
                    nTotal += eqp.nHP;
                }
                else if (stat == "MP")
                {
                    pTotal += eqp.pMP;
                    nTotal += eqp.nMP;
                }
            }
        }

        int[] r = new int[2];
        r[0] = pTotal;
        r[1] = nTotal;

        return r;
    }

    public int GetUnmodifiedStat(string stat)
    {
        if (stat == "ATK") return ATK;
        else if (stat == "INT") return INT;
        else if (stat == "DEF") return DEF;
        else if (stat == "RES") return RES;
        else if (stat == "AGL") return AGL;
        else if (stat == "ACR") return ACR;
        else if (stat == "LCK") return LCK;
        else if (stat == "SPD") return SPD;
        else if (stat == "EDR") return EDR;
        else if (stat == "HP") return maxHP;
        else if (stat == "MP") return maxMP;

        Debug.Log("WRONG STAT ENTERED MF");
        return -999999999;

    }



}

[Serializable]
public class BodyPart
{
    public string bodyPartName;

    public StatusCondition[] conditionsOnBreak; 


}