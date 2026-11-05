using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopData", menuName = "Bipole VI/ShopData")]
public class ShopData : ScriptableObject
{
    public CurrencyType currencyType;

    public float sellMultiplier = 1f;
    public float buyMultiplier = 0.5f;

    public List<ItemObject> itemsForSale;

}


[Serializable]
public class ShopDatabaseEntry
{
    public string shopID;
    public ShopData shopData;
    public int vendorWallet, vendorWalletCurrent;
}
