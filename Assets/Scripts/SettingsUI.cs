using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] Button returnButton;

    void OnEnable()
    {
        GameManager.gm.dungeonPlayer.buttonSelectOnDecline = returnButton.gameObject;
    }
    
}
