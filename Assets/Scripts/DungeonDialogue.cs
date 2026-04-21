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
    public Sprite portrait, commandSprite;
    public String voice = "";
    public DialogueChoice[] choices;
    public int x, y;

    [Header("Only used in save data, automatic")]
    [HideInInspector] public string spriteName;
    [HideInInspector] public String commandSpriteName;

    [Header("Only used in combat")]
    public PartyMember battler; // used for combat targeting
    public GameObject obj; // used for attack effects


    public DungeonDialogue()
    {

    }

    public DungeonDialogue(String en, String jp)
    {
        textEn = en;
        textJp = jp;
    }

    public DungeonDialogue(String en, String jp, Sprite s)
    {
        textEn = en;
        textJp = jp;
        portrait = s;
    }

}

[Serializable]
public class DialogueChoice
{
    public String choiceText, choiceFlag;
}

