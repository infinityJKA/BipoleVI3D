using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DungeonDialogue //: MonoBehaviour
{
    [TextArea(2, 10)]
    public String command = "",
    textEn, textJp;
    public Sprite portrait;
    public ItemObject item;
    public GameObject obj;

    public DungeonDialogue()
    {
        
    }
    
    public DungeonDialogue(String en, String jp)
    {
        textEn = en;
        textJp = jp;
    }

}

