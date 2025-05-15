using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class Rotate2dSprite : MonoBehaviour
{
    void Update()
    {
        UpdateRotation();
    }

    public void UpdateRotation()
    {
        if (GameManager.gm.dungeonPlayer.playerFacing == PlayerFacing.North)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (GameManager.gm.dungeonPlayer.playerFacing == PlayerFacing.South)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (GameManager.gm.dungeonPlayer.playerFacing == PlayerFacing.East)
        {
            transform.eulerAngles = new Vector3(0, 90, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 270, 0);
        }
    }
}
