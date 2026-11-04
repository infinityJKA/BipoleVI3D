using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class CoinInInventoryUI : MonoBehaviour, ISelectHandler
{
    public TMP_Text nameText, yearText;
    public GameObject favorited;
    public MoneyObject moneyObject;
    public MicroeconomicsUI microeconomicsUI;
    public Button button;
    public RectTransform rectTransform;

    public void OnSelect(BaseEventData eventData)
    {
        UpdateDescription();
        microeconomicsUI.scrollSnapTo.SnapTo(rectTransform);

    }

    public void UpdateGraphic()
    {
        nameText.text = moneyObject.itemName;// + " ("+moneyObject.mintYear+" AN)";
        //yearText.text = "("+moneyObject.mintYear+" AN)";
        favorited.SetActive(moneyObject.favorited);
    }

    public void UpdateDescription()
    {
        microeconomicsUI.nameText.text = moneyObject.itemName;
        microeconomicsUI.descriptionText.text =
        "Base Value: " + moneyObject.value + "\n" +
        "Mint Year: " + moneyObject.mintYear + "\n" +
        "Condition: " + moneyObject.condition + "\n" +
        moneyObject.itemDescription;
    }

    public void OnClick()
    {
        if(moneyObject.favorited)
        {
            moneyObject.favorited = false;
            favorited.SetActive(false);
        }
        else
        {
            moneyObject.favorited = true;
            favorited.SetActive(true);
        }

        UpdateGraphic();
    }


}
