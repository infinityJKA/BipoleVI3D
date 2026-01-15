using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIMain : MonoBehaviour
{
    public GameObject[] menuScreens;
    public Button returnButton;
    public EquipInMenuUI equipUI;
    public PartyInMenuUI partyUI;

    void OnEnable()
    {
        GameManager.gm.dungeonPlayer.buttonSelectOnDecline = returnButton.gameObject;
    }


    public void DisableAllButThis(GameObject obj)
    {
        foreach (GameObject screen in menuScreens)
        {
            if (screen == obj)
            {
                obj.SetActive(true);
            }
            else
            {
                screen.SetActive(false);
            }
        }
    }

}
