using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class MenuSideButton : MonoBehaviour, ISelectHandler
{
    public MenuUIMain menuUI;
    public GameObject screen;

    public void OnSelect(BaseEventData eventData)
    {
        menuUI.DisableAllButThis(screen);
        GameManager.gm.dungeonPlayer.buttonSelectOnDecline = menuUI.returnButton.gameObject;

        menuUI.equipUI.ResetButtonSnapParty();
        //menuUI.partyUI.ResetButtonSnap();
    }


}
