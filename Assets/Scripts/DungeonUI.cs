using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DungeonUI : MonoBehaviour
{
    public TMP_Text popupText;
    public GameObject popupTextParent;

    public void PopupText(String s)
    {
        popupTextParent.SetActive(true);
        popupText.text = s;
    }

}
