using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate2dSprite : MonoBehaviour
{
    private PlayerController dungeonPlayer;
    private DungeonManager dm;

    private void Start()
    {
        dungeonPlayer = GameManager.gm.dungeonPlayer;
        dm = FindObjectOfType<DungeonManager>();
    }

    void Update()
    {
        UpdateRotation();
    }

    public void UpdateRotation()
    {
        if (dm.sceneMode == SceneMode.Town)
        {
            Vector3 cameraRotation = dungeonPlayer.cameraObject.transform.rotation.eulerAngles;

            transform.rotation = Quaternion.Euler(0f, cameraRotation.y, 0f);

        }
        else{
            if (dungeonPlayer.playerFacing == PlayerFacing.North)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (dungeonPlayer.playerFacing == PlayerFacing.South)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else if (dungeonPlayer.playerFacing == PlayerFacing.East)
            {
                transform.eulerAngles = new Vector3(0, 90, 0);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 270, 0);
            }
        }
    }
}
