using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ShopSideButton : MonoBehaviour, ISelectHandler
{
    public ShopMenu shopMenu;
    public string toGenerate;

    public void OnSelect(BaseEventData eventData)
    {
        shopMenu.CreateDisplay(toGenerate);
    }

}
