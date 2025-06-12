using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Item", menuName = "Bipole VI/Item")]
public class ItemObject : ScriptableObject
{
    [Header("Universal")]
    public string itemName;
    public string itemName_jp;
    [TextArea(5, 10)]
    public string itemDescription, itemDescription_jp;
    public ItemType itemType;
    public int value; // for coins, you can't sell them normally so this is only used for when you BUY the coin from a special vendor
    public Sprite sprite; // not used for all item types atm

    [Header("Consumable")]
    public int restoreHP;
    public int restoreMP;
    public bool restoreSetAmount; // restore # if true, restore % if false
    public bool targetType;
    public bool canUseInBattle; // if this can be used in battle
    public bool canUseInInventory; // if this can be used in inventory
    public bool isAttack; // treats this as an action when used in battle

    [Header("Equipment")]
    // pXXX = stat% boost, nXXX = +stat boost
    public EquipmentType equipmentType;
    public int pDEF, nDEF, pRES, nRES, pATK, nATK, pINT, nINT, pAGL, nAGL, pACR, nACR, pSPD, nSPD, pLCK, nLCK, pEDR, nEDR, pHP, nHP, pMP, nMP;
    public EquipmentEffect equipmentEffect; // action or passive
    public EquipmentAction equipmentAction; // the action to be used if the equipment effect isn't passvie

    [Header("Book")]
    public DungeonDialogue[] pages;

    [Header("Coin")]
    public int coinValue; // used to determine your wealth, etc 1G coin would have 1 coinValue
    public CurrencyType currencyType;
    public float condition; // 100 max, 0.01 minimum
    public int mintYear;
    public String design; // the same year can have different designs



}

[Serializable]
public class EquipmentAction
{
    [TextArea(5, 10)]
    public string actionDescription, actionDescription_jp;
    public bool isUlt; // false if a normal action, true if can only be used as an ULT action
    public bool isHealing; // true if healing action, false in all other cases
    public String actionName, actionName_jp;
    public DamageType damageType; // physical or magical
    public EquipmentAction damageEquipmentType; // for hitting weaknesses (sword, fire, etc.)
    public int PWR, HIT;
    public bool setCost; // costs # if true, % if false
    public int costMP, costHP, costBP; // costBP is only used if the action is an ULT action
    public int gainBP,gainVIZ;
    public bool setVIZ; // increase VIZ by # if true, increase by % if false
    public TargetType targetType; // who the action can target
    public StatusCondition[] statusConditions; // status condition(s) afficted onto the target
    public StatusCondition[] addtionalStatusOnUser; // additional condition(s) the user gets affected by, no matter the target
}

[Serializable]
public class StatusCondition
{
    public String stat;
    public float amount;
    public bool isPercentage; // uses amount% percent if true, otherwise affects stat by amount#
    public int turns; // counts down every turn
}

public enum TargetType
{
    Self, OneEnemy, OneAlly, Party, EnemyParty, BothParties
}

public enum ItemType
{
    Consumable, Equipment, Material, Book, Key
}

public enum CurrencyType {
    Bieace, Bievil, Farle, ShadowRealm, Lunarian
}

public enum EquipmentEffect
{
    Action, UltAction, Passive, None
}

public enum DamageType
{
    Physical, Magical
}

public enum EquipmentType
{
    // primarily physical
    Sword, // cut
    Lance, // pierce
    Bludgeon, // staffs, clubs, etc
    Fists,
    Bow,
    Gun,
    // primarily magical
    Fire,
    Water,
    Ice,
    Bio,
    Light,
    Dark,
    // primarily armor
    Universal,
    // character specific (only used for equip type and not damage type)
    Stellein,
    Eriuqs

}

[Serializable]
public class BookDialogue //: MonoBehaviour
{
    [TextArea(5, 10)]
    public String textEn, textJp;
    public Sprite sprite;

}
