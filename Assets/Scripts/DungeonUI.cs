using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUI : MonoBehaviour
{
    public TMP_Text popupText, dialogueText, dialogueText2, facingText;
    public Image dialoguePortrait;
    public GameObject popupTextParent, dialogueBox, dialogueTriangle;
    public DungeonPartyMemberUI[] partyMemberUIs;
    public TMP_Text dateText, dayText;
    public Image moonSprite, eyeSprite;
    public Sprite newMoon, waxingCrescent, firstQuarter, waxingGibbous, fullMoon, waningGibbous, thirdQuarter, waningCrescent;
    public Sprite[] eyeSprites;
    public void PopupText(String s)
    {
        popupTextParent.SetActive(true);
        popupText.text = s;
    }

}
