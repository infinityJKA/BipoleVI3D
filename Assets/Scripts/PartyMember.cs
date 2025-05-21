using UnityEngine;
using System.Linq;
using System;

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
    public Sprite sprite;
    public BodyPart[] bodyParts;

    public ItemObject[] currentlyEquipped = new ItemObject[4];

    public bool Weakness(EquipmentType e)
    {
        return weaknesses.Contains(e);
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



}

[Serializable]
public class BodyPart
{
    public string bodyPartName;

    public StatusCondition[] conditionsOnBreak; 


}