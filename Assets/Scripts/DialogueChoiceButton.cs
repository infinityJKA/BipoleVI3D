using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueChoiceButton : MonoBehaviour
{
    public PlayerController playerController;
    public string flag; // this is what this button will send you too
    public TMP_Text buttonText;

    public void OnClick()
    {
        Debug.Log("Button clicked!");
        playerController.GoToFlag(flag);
    }
}
