using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class CoinInInventoryUI : MonoBehaviour
{
    public TMP_Text nameText, yearText;
    public GameObject favorited;
    public MoneyObject moneyObject;
    public MicroeconomicsUI microeconomicsUI;
    public Button button;

    public void UpdateGraphic()
    {
        nameText.text = moneyObject.itemName;// + " ("+moneyObject.mintYear+" AN)";
        //yearText.text = "("+moneyObject.mintYear+" AN)";
        favorited.SetActive(moneyObject.favorited);
    }


}
