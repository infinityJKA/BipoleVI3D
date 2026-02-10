using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonPartyMemberUI : MonoBehaviour
{
    public GameObject main, empty;
    public TMP_Text nameText, hpText, mpText, vizText;
    public Image bgSprite,hpBar,mpBar;
    [SerializeField] Sprite normalBg, emptyBg;

    public void UpdateValues(string n, int cHP, int mHP, int cMP, int mMP, int viz)
    {
        nameText.text = n;
        hpText.text = cHP + "/" + mHP;
        mpText.text = cMP + "/" + mMP;
        vizText.text = viz + " VIZ";

        hpBar.fillAmount = (float) cHP / mHP;
        mpBar.fillAmount = (float) cHP / mHP;
    
    }

    public void SetEmpty(bool b) {
        if (b)
        {
            main.SetActive(false);
            //empty.SetActive(true);
            bgSprite.sprite = emptyBg;
        }
        else
        {
            empty.SetActive(false);
            main.SetActive(true);
            bgSprite.sprite = normalBg;
        }
    }

}
