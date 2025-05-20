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

    public ItemObject[] currentlyEquipped;

    public bool Weakness(EquipmentType e)
    {
        return weaknesses.Contains(e);
    }


}

[Serializable]
public class BodyPart
{
    public string bodyPartName;

    public StatusCondition[] conditionsOnBreak; 


}