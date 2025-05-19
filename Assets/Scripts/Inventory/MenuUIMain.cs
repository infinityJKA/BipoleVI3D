using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIMain : MonoBehaviour
{
    public GameObject[] menuScreens;


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
