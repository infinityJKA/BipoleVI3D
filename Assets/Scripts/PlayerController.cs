using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] DungeonManager dm;
    private GameManager gm;
    public DungeonUI ui;
    public bool animateMovement = false;
    public float moveSpeed = 10f;
    public float rotateSpeed = 500f;
    public int playerX,playerY = 0;
    public PlayerFacing playerFacing = PlayerFacing.North;

    [SerializeField] Tile currentTile;

    Vector3 targetGridPos;
    Vector3 prevTargetGridPos;
    Vector3 targetRotation;

    [SerializeField] Sprite up,down,left,right;
    public DungeonInputControlState inputState = DungeonInputControlState.FreeMove;
    public int dialogueIndex;
    public DungeonDialogue[] currentDialogue;
    public float textSpeed = 0.01f;
    private bool finishedDialogueEarly = false;
    public GameObject optionsUI, menuUI,optionsButtonSelected,menuButtonSelected;
    private EventSystem eventSystem;

    private void Start()
    {
        targetGridPos = Vector3Int.RoundToInt(transform.position);
        currentTile = dm.GetTile(playerX, playerY);
        // currentTile.playerHasDiscovered = true;
        currentTile.EnterTile(PlayerMapSprite());
        gm = GameManager.gm;
        eventSystem = GameObject.FindObjectOfType<EventSystem>();
    }

    private void FixedUpdate()
    {
        MovePlayerObject();
    }

    private Sprite PlayerMapSprite(){
        if(playerFacing == PlayerFacing.North){
            return up;
        }
        else if(playerFacing == PlayerFacing.South){
            return down;
        }
        else if(playerFacing == PlayerFacing.East){
            return right;
        }
        else{
            return left;
        }
    }

    public void OpenOptions()
    {
        inputState = DungeonInputControlState.Menu;
        optionsUI.SetActive(true);
        if (eventSystem.currentSelectedGameObject != optionsButtonSelected)
        {
            eventSystem.SetSelectedGameObject(optionsButtonSelected);
            Debug.Log("Set eventSystem selected button");
        }
        else
        {
            Debug.Log("eventSystem button already set");
        }
    }

    public void OpenMenu()
    {
        inputState = DungeonInputControlState.Menu;
        menuUI.SetActive(true);
        if (eventSystem.currentSelectedGameObject != menuButtonSelected)
        {
            eventSystem.SetSelectedGameObject(menuButtonSelected);
        }
    }

    public void SetInputStateFreeMove()
    {
        inputState = DungeonInputControlState.FreeMove;
    }

    public void Interact()
    {
        if (DoneMoving)
        {
            if (dm.GetTile(playerX, playerY).interactType == InteractType.Talk)
            {
                inputState = DungeonInputControlState.Dialogue;
                dialogueIndex = -1; // -1 bc +1s at the start of dialogue
                currentDialogue = dm.GetTile(playerX, playerY).dialogue;
                ui.popupTextParent.SetActive(false);
                ui.dialogueBox.SetActive(true);
                finishedDialogueEarly = false;
                ui.dialogueTriangle.SetActive(false);
                ProgressDialogue();
            }
        }
    }

    public void ProgressDialogue()
    {
        if (dialogueIndex == -1 || currentDialogue[dialogueIndex].textEn == ui.dialogueText.text || finishedDialogueEarly) // makes sure dialogue is finished or skipped first
        {
            finishedDialogueEarly = false;
            ui.dialogueTriangle.SetActive(false);
            dialogueIndex++;
            DungeonDialogue d = currentDialogue[dialogueIndex];
            if (d.command != "")
            {
                Debug.Log("Going to do command \"" + d.command + "\"");
                PerformDialogueCommand(d.command);
            }
            else
            {
                ui.dialogueText.text = "";
                Debug.Log("Going to say line \"" + d.textEn + "\"");
                StartCoroutine(TypeLine(d.textEn));
            }
        }
        else
        {
            finishedDialogueEarly = true;
            ui.dialogueText.text = currentDialogue[dialogueIndex].textEn;
            ui.dialogueTriangle.SetActive(true);
        }
    }

    private IEnumerator TypeLine(String l)
    {
        foreach (char c in l)
        {
            if (finishedDialogueEarly) { break; }
            ui.dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        ui.dialogueTriangle.SetActive(true);
    }

    private void PerformDialogueCommand(String command)
    {
        if (command == "END")
        {
            ui.dialogueBox.SetActive(false);
            ui.popupTextParent.SetActive(true);
            inputState = DungeonInputControlState.FreeMove;
        }
    }

    private void UpdatePartyUI()
    {
        UpdatePartyUISingle(0);

        if (gm.partyMembers.Count >= 2)
        {
            UpdatePartyUISingle(1);
        }
        else
        {
            ui.partyMemberUIs[1].SetEmpty(true);
        }
    }

    private void UpdatePartyUISingle(int i)
    {
        ui.partyMemberUIs[i].SetEmpty(false);
        ui.partyMemberUIs[i].UpdateValues(gm.partyMembers[i].characterNameEn, gm.partyMembers[i].currentHP, gm.partyMembers[i].maxHP, gm.partyMembers[i].currentMP, gm.partyMembers[i].maxMP, gm.partyMembers[i].VIZ);
    }

    void MovePlayerObject(){
        // if(true){ //if can move
        prevTargetGridPos = targetGridPos;
        Vector3 targetPosition = targetGridPos;

        if(targetRotation.y > 270f && targetRotation.y < 361){
            targetRotation.y = 0f;
        }
        if(targetRotation.y < 0f){
            targetRotation.y = 270f;
        }

        if(!animateMovement){
            transform.position = targetPosition;
            transform.rotation = Quaternion.Euler(targetRotation);
        }
        else{
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * rotateSpeed);
        }



        // }
        // else{
        //     targetGridPos = prevTargetGridPos;
        // }
    }


    public void Walk(int x, int y){    // x is to the side, y is upwards
        if(DoneMoving){
            int oldX = x;
            int oldY = y;

            // adjust for rotation
            if(playerFacing == PlayerFacing.South){
                x *= -1;
                y *= -1;
            }
            else if(playerFacing == PlayerFacing.East){
                oldY = y;
                y = -1*x;
                x = oldY;
            }
            else if(playerFacing == PlayerFacing.West){
                oldY = y;
                y = x;
                x = -1*oldY;
            }
            Debug.Log("Old: "+oldX+","+oldY+"  New: "+x+","+y);
            Tile t = dm.GetTile(playerX+x,playerY+y);
            if(t != null){
                if (t.walkable)              // STUFF HERE IS CALLED IF YOU ACTUALLY WALKED
                {
                    playerX += x;
                    playerY += y;
                    targetGridPos = t.transform.position;

                    currentTile.UpdateMiniMapSprite(); // reset the minimap sprite before leaving
                    currentTile = t; // set new current tile
                    t.EnterTile(PlayerMapSprite()); // update minimap sprites

                    UpdatePartyUI(); // updates the party ui

                    if (t.interactType != InteractType.None)  // sets popup text
                    {
                        if (t.interactType == InteractType.Talk)
                        {
                            ui.PopupText("TALK");
                        }
                        else if (t.interactType == InteractType.Shop)
                        {
                            ui.PopupText("SHOP");
                        }
                        else if (t.interactType == InteractType.Exit)
                        {
                            ui.PopupText("EXIT");
                        }
                    }
                    else
                    {
                        ui.popupTextParent.SetActive(false);
                    }

                }
                else
                {
                    Debug.Log("Trying to walk to a nonwalkable tile!");
                }
            }
            else{
                Debug.Log("Tile you are trying to walk to is NULL!");
            }
        }
    }


    public void RotateLeft(){
        if (DoneMoving)
        {
            if (playerFacing == PlayerFacing.North)
            {
                playerFacing = PlayerFacing.West;
                ui.facingText.text = "Facing\nWEST";
            }
            else if (playerFacing == PlayerFacing.West)
            {
                playerFacing = PlayerFacing.South;
                ui.facingText.text = "Facing\nSOUTH";
            }
            else if (playerFacing == PlayerFacing.South)
            {
                playerFacing = PlayerFacing.East;
                ui.facingText.text = "Facing\nEAST";
            }
            else
            {
                playerFacing = PlayerFacing.North;
                ui.facingText.text = "Facing\nNORTH";
            }
            targetRotation -= Vector3.up * 90f;

            dm.GetTile(playerX, playerY).EnterTile(PlayerMapSprite());
        }
    }

    public void RotateRight(){
        if(DoneMoving){
            if (playerFacing == PlayerFacing.North)
            {
                playerFacing = PlayerFacing.East;
                ui.facingText.text = "Facing\nEAST";
            }
            else if (playerFacing == PlayerFacing.West)
            {
                playerFacing = PlayerFacing.North;
                ui.facingText.text = "Facing\nNORTH";
            }
            else if (playerFacing == PlayerFacing.South)
            {
                playerFacing = PlayerFacing.West;
                ui.facingText.text = "Facing\nWEST";
            }
            else
            {
                playerFacing = PlayerFacing.South;
                ui.facingText.text = "Facing\nSOUTH";
            }
            targetRotation += Vector3.up*90f;

            dm.GetTile(playerX,playerY).EnterTile(PlayerMapSprite());
            // MinimapSprite(dm.GetTile(playerX,playerY));
        }
    }




    bool DoneMoving {
        get{
            if( (Vector3.Distance(transform.position, targetGridPos) < 0.00001f) && (Vector3.Distance(transform.eulerAngles, targetRotation) < 0.00001f) ){
                return true;
            }
            else{return false;}
        }
    }





}



public enum PlayerFacing{
    North,South,East,West
}