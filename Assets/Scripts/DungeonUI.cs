using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DungeonUI : MonoBehaviour
{
    public TMP_Text popupText, dialogueText, facingText;
    public GameObject popupTextParent, dialogueBox, dialogueTriangle;
    public DungeonPartyMemberUI[] partyMemberUIs;

    public void PopupText(String s)
    {
        popupTextParent.SetActive(true);
        popupText.text = s;
    }

}
