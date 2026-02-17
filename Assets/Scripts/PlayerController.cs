using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;

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
    public List<DungeonDialogue> currentDialogue;
    public float textSpeed = 0.01f;
    private bool finishedDialogueEarly = false;
    [Header("UI Stuff")]
    public GameObject optionsUI, menuUI, optionsButtonSelected, menuButtonSelected;
    public EventSystem eventSystem;
    public GameObject buttonSelectOnDecline;
    private TMP_Text currentDialogueText;
    public CombatReturnTo combatReturnTo;

    private void Start()
    {
        currentDialogueText = ui.dialogueText;
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

    public void SetButtonSelectOnDecline(GameObject button)
    {
        buttonSelectOnDecline = button;
    }

    public void DeclineInMenu()
    {
        eventSystem.SetSelectedGameObject(buttonSelectOnDecline);
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

    public void StartDialogueCombat(List<DungeonDialogue> d)
    {
        inputState = DungeonInputControlState.Dialogue;
        dialogueIndex = -1; // -1 bc +1s at the start of dialogue

        List<DungeonDialogue> nd = new List<DungeonDialogue>();
        for (int i = 0; i < d.Count; i++) // create a new list that can be modified as combat goes on
        {
            nd.Add(d[i]);
        }

        currentDialogue = nd;
        ui.combat.HideMenusForDialogue();
        ui.dialogueBox.SetActive(true);
        finishedDialogueEarly = false;
        ui.dialogueTriangle.SetActive(false);
        ProgressDialogue();
    }

    public void StartDialogueCombatItem(ItemObject item)
    {
        inputState = DungeonInputControlState.Dialogue;
        dialogueIndex = -1; // -1 bc +1s at the start of dialogue

        List<DungeonDialogue> nd = new List<DungeonDialogue>();

        nd.Add(new DungeonDialogue(
            gm.currentBattler.characterNameEn + " used " + item.itemName + "!",
            "japanese translation here"
        ));

        if (item.equipmentAction.attackDialogue.Count > 0)
        {
            for (int i = 0; i < item.equipmentAction.attackDialogue.Count; i++) // create a new list that can be modified as combat goes on
            {
                nd.Add(item.equipmentAction.attackDialogue[i]);
            }
        }

        if (item.destoryOnUse)
        {
            Debug.Log("Destorying consumable " + item.itemName);
            gm.inventory.RemoveItem(item, 1);
        }

        currentDialogue = nd;
        ui.combat.HideMenusForDialogue();
        ui.dialogueBox.SetActive(true);
        finishedDialogueEarly = false;
        ui.dialogueTriangle.SetActive(false);

        if (item.restoreHP > 0 || item.restoreMP > 0)
        {
            if(item.targetType == TargetType.Party || item.targetType == TargetType.EnemyParty)
            {
                PerformHealItemAll(false, true);
            }
            else
            {
                PerformHealItem(gm.currentTarget, true);
            }
        }
        else
        {
            ProgressDialogue();
        }
    }

    public void ProgressCombatTurn()
    {
        Debug.Log("ProgressCombatTurn()");
        // this progresses the turn
        ui.dialogueBox.SetActive(false);
        ui.combat.mainBox.SetActive(true);
        inputState = DungeonInputControlState.Combat;

        int newIndex = (ui.combat.battlers.IndexOf(gm.currentBattler) + 1);
        if (newIndex >= ui.combat.battlers.Count)
        {
            newIndex = 0;
        }

        // sets the next character to take a turn
        gm.currentBattler = ui.combat.battlers[newIndex];

        StartCombatTurn();
    }

    public void ProgressDialogue()
    {
        Debug.Log("ProgressDialogue()");

        UpdatePartyUI();

        if (dialogueIndex == -1 || currentDialogue[dialogueIndex].textEn == currentDialogueText.text || finishedDialogueEarly) // makes sure dialogue is finished or skipped first
        {
            finishedDialogueEarly = false;
            ui.dialogueTriangle.SetActive(false);
            dialogueIndex++;

            if (dialogueIndex >= currentDialogue.Count)
            {
                if (gm.inCombat)
                {
                    Debug.Log("End of dialogue in combat, going to ProgressCombatTurn()");
                    ProgressCombatTurn();
                }
            }
            else
            {
                DungeonDialogue d = currentDialogue[dialogueIndex];
                if (d.command != "")
                {
                    Debug.Log("Going to do command \"" + d.command + "\"");
                    PerformDialogueCommand(d.command);
                }
                else
                {
                    currentDialogueText.text = "";
                    Debug.Log("Going to say line \"" + d.textEn + "\"");
                    if (d.portrait == null)
                    {
                        currentDialogueText = ui.dialogueText;
                        ui.dialogueText2.gameObject.SetActive(false);
                        ui.dialogueText.gameObject.SetActive(true);
                        ui.dialoguePortrait.gameObject.SetActive(false);
                    }
                    else
                    {
                        currentDialogueText = ui.dialogueText2;
                        ui.dialogueText.gameObject.SetActive(false);
                        ui.dialogueText2.gameObject.SetActive(true);
                        ui.dialoguePortrait.sprite = d.portrait;
                        ui.dialoguePortrait.gameObject.SetActive(true);
                    }
                    StartCoroutine(TypeLine(d.textEn));
                }
            }
        }
        else
        {
            Debug.Log("Finished dialogue early (" + currentDialogue[dialogueIndex].textEn +")");
            finishedDialogueEarly = true;
            currentDialogueText.text = currentDialogue[dialogueIndex].textEn;
            ui.dialogueTriangle.SetActive(true);

            if(currentDialogueText.text == "")
            {
                Debug.Log("Textbox is empty for some reason, progressing dialogue");
                ProgressDialogue();
            }
        }
    }

    private IEnumerator TypeLine(String l)
    {
        foreach (char c in l)
        {
            if (finishedDialogueEarly) { break; }
            currentDialogueText.text += c;
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
        else if (command == "ADDITEM")
        {
            GameManager.gm.inventory.AddItem(currentDialogue[dialogueIndex].item, 1);
            finishedDialogueEarly = true;
            ProgressDialogue();
        }
        else if (command == "START_TURN") // for starting a turn of combat
        {
            ui.dialogueBox.SetActive(false);
            ui.combat.mainBox.SetActive(true);
            inputState = DungeonInputControlState.Combat;
            StartCombatTurn();
        }
        else if (command == "SPAWN_EFFECT_ALL_ENEMIES")
        {
            foreach (PartyMember enem in gm.enemies)
            {
                Instantiate(currentDialogue[dialogueIndex].obj, enem.display.gameObject.transform, ui.combat.gameObject);
            }
            ProgressDialogue();
        }
        else if (command == "ATTACK_SINGLE_ENEMY")
        {
            PerformAttack(gm.currentTarget, true, false);
        }
        else if (command == "ATTACK_ALL_ENEMIES")
        {
            PerformAttackAll(true, false);
        }
        else if (command == "EFFECT_SINGLE_ENEMY")
        {
            PerformAttack(gm.currentTarget, false, false);
        }
        else if (command == "EFFECT_ALL_ENEMIES")
        {
            PerformAttackAll(false, false);
        }
        else if (command == "EFFECT_SINGLE_ALLY")
        {
            PerformAttack(gm.currentTarget, false, true);
        }
        else if (command == "EFFECT_ALL_ALLIES")
        {
            PerformAttackAll(false, true);
        }
        else if (command == "ENEM_DIED") // removes the enemy from the list of enemies (done here bc it messes up if done mid-loop of a multitarget attack)
        {
            gm.enemies.Remove(currentDialogue[dialogueIndex].battler);
            ui.combat.battlers.Remove(currentDialogue[dialogueIndex].battler);
            Destroy(currentDialogue[dialogueIndex].battler);
            ui.combat.UpdateOrderGraphic();

            finishedDialogueEarly = true; // <-- makes you click through a blank textbox if I don't do this, genuinely can't figure out why lmao

            ProgressDialogue();
        }
        else if (command == "END_COMBAT")
        {
            ui.combat.HideMenusForDialogue();
            ui.dialogueBox.SetActive(false);
            ui.popupTextParent.SetActive(true);
            gm.stepsSinceEyeChange = 0;
            gm.eyePhase = 0;
            inputState = DungeonInputControlState.FreeMove;
        }
        else if (command == "RESTART_SCENE")
        {
            Destroy(gm.gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }


    public void CombatEXP(int xp, int expld) {
        foreach (PartyMember pm in ui.combat.battlers)
        {
            if (pm.isEnemy == false)
            {
                int exp = xp;

                if (pm.LV > expld) // if over-leveled
                {
                    exp = xp / ((pm.LV - expld) * (pm.LV / expld));
                }

                currentDialogue.Add(new DungeonDialogue(
                    pm.characterNameEn + " gained " + exp + " EXP!",
                    "japanese translation here",
                    pm.portrait
                ));

                pm.EXP += exp;
                if (pm.EXP >= 1000)
                {
                    currentDialogue.Add(new DungeonDialogue(
                        pm.characterNameEn + " leveled up!",
                        "japanese translation here",
                        pm.portrait
                    ));

                    pm.EXP -= 1000;
                    pm.LV++;
                }
            }
        }
    }


    private void PerformAttack(PartyMember target, bool dealDamage, bool unmissable)
    {
        Debug.Log("PerformAttack() on " + target.characterNameEn);

        currentDialogue.Add(new DungeonDialogue(
            gm.currentBattler.characterNameEn + " used " + gm.currentAction.actionName + "!",
            "japanese translation here"
        ));

        // spawn animation
        if (target.isEnemy)
        {
            GameObject anim = Instantiate(currentDialogue[dialogueIndex].obj, gm.currentTarget.display.gameObject.transform.position, Quaternion.identity, ui.combat.transform);
        }

        PerformAttackOnTarget(target, dealDamage, unmissable);
        GiveSelfStatusesFromAttack();

        ProgressDialogue();

    }

    private void PerformHealItem(PartyMember target, bool unmissable)
    {

        // spawn animation
        if (target.isEnemy)
        {
            GameObject anim = Instantiate(currentDialogue[dialogueIndex].obj, gm.currentTarget.display.gameObject.transform.position, Quaternion.identity, ui.combat.transform);
        }

        PerformHealItemOnTarget(target, unmissable);

        ProgressDialogue();

    }

    private void PerformAttackAll(bool dealDamage, bool unmissable)
    {
        currentDialogue.Add(new DungeonDialogue(
            gm.currentBattler.characterNameEn + " used " + gm.currentAction.actionName + "!",
            "japanese translation here"
        ));

        if (gm.currentBattler.isEnemy == false) // spawn anim on each enemy if player is attacking
        {
            foreach (PartyMember enem in gm.enemies)
            {
                Instantiate(currentDialogue[dialogueIndex].obj, enem.display.gameObject.transform.position, Quaternion.identity, ui.combat.transform);
            }
        }

        // create list of targets depending on who is attacking
        List<PartyMember> targets;
        if (gm.currentBattler.isEnemy == false) // if player is attacking
        {
            targets = gm.enemies;
        }
        else // if enemy is attacking
        {
            targets = new List<PartyMember>();
            foreach (PartyMember battler in ui.combat.battlers)
            {
                if (battler.isEnemy == false)
                {
                    targets.Add(battler);
                    Debug.Log(battler.characterNameEn + " is being targeted in party attack");
                }
            }
        }

        // loop the attack for each target
        foreach (PartyMember target in targets)
        {
            gm.currentTarget = target;
            gm.currentHitrates = gm.CalculateHitRate();
            PerformAttackOnTarget(target,dealDamage, unmissable);
        }
        GiveSelfStatusesFromAttack();
        ProgressDialogue();

    }

    private void PerformHealItemAll(bool dealDamage, bool unmissable)
    {
        if (gm.currentBattler.isEnemy == false) // spawn anim on each enemy if player is attacking
        {
            foreach (PartyMember enem in gm.enemies)
            {
                Instantiate(currentDialogue[dialogueIndex].obj, enem.display.gameObject.transform.position, Quaternion.identity, ui.combat.transform);
            }
        }

        // create list of targets depending on who is using
        List<PartyMember> targets;
        if (gm.currentBattler.isEnemy == false) // if player is using
        {
            targets = gm.enemies;
        }
        else // if enemy is using
        {
            targets = new List<PartyMember>();
            foreach (PartyMember battler in ui.combat.battlers)
            {
                if (battler.isEnemy == false)
                {
                    targets.Add(battler);
                    Debug.Log(battler.characterNameEn + " is being targeted in party target");
                }
            }
        }

        // loop the item for each target
        foreach (PartyMember target in targets)
        {
            gm.currentTarget = target;
            PerformHealItemOnTarget(target, unmissable);
        }

        GiveSelfStatusesFromAttack();
        ProgressDialogue();

    }

    private void PerformAttackOnTarget(PartyMember target, bool dealDamage, bool unmissable)
    {
        Debug.Log(target.characterNameEn + " is being attacked");

        // create a new dialogue object to show the result
        DungeonDialogue d = new DungeonDialogue();

        // calculate the hitrate
        float hitrate = 0;
        if (!unmissable)
        {
            if (gm.currentBodyPartIndex == -1) hitrate = gm.currentHitrates[0] * 100;
            else hitrate = gm.currentHitrates[1] * 100;
        }

        if (unmissable) hitrate = 999f;

        Debug.Log("hitrate: " + hitrate);

        if (UnityEngine.Random.Range(0, 100) <= hitrate) // this is a successful hit
        {
            int damage = 0;
            int defense = 0;
            if (gm.currentAction.damageType == DamageType.Physical)
            {
                damage = gm.currentBattler.CalculateStat("ATK", gm.currentAction.PWR);
                defense = target.CalculateStat("DEF");
            }
            else
            {
                damage = gm.currentBattler.CalculateStat("INT", gm.currentAction.PWR);
                defense = target.CalculateStat("RES");
            }

            if (dealDamage) // do attack damage if this action can deal damage
            {

                // check for weakness
                float weakness = 1f;
                String weaknessStr = "";
                foreach (EquipmentType w in target.weaknesses)
                {
                    if (w == gm.currentAction.damageEquipmentType)
                    {
                        weakness = 1.5f;
                        weaknessStr = " (Weakness applied!)";
                    }
                }

                // deal damage differently if crit
                Debug.Log("crit rate: " + gm.currentHitrates[2] * 100);
                if (UnityEngine.Random.Range(0, 100) <= gm.currentHitrates[2] * 100)
                {
                    int dmg = Convert.ToInt32(damage * damage * (weakness + 1.5) / (damage + defense));
                    d.textEn = "CRITICAL HIT! " + target.characterNameEn + " took " + dmg + " damage!" + weaknessStr;
                    target.currentHP -= dmg;
                }
                else
                {
                    int dmg = Convert.ToInt32(damage * damage * weakness / (damage + defense));
                    d.textEn = target.characterNameEn + " took " + dmg + " damage!" + weaknessStr;
                    target.currentHP -= dmg;
                }

                // add the damage dialogue
                currentDialogue.Add(d);
            }

            // add status effects to target
                foreach (StatusCondition sc in gm.currentAction.statusConditions)
                {
                    currentDialogue.Add(new DungeonDialogue(
                        "Inflicted " + sc.amount + "x " + sc.stat + " on " + target.characterNameEn + " for " + sc.turns + " turns",
                        "japanese stuff here"
                    ));
                    var effectClone = sc;
                    target.statusConditions.Add(effectClone);
                }

            // check for BREAKs
            if (gm.currentBodyPartIndex != -1)
            {
                target.bodyParts[gm.currentBodyPartIndex].timesDamaged += 1;
                if (target.bodyParts[gm.currentBodyPartIndex].timesDamaged >= target.EDR) // if broken
                {
                    DungeonDialogue d2 = new DungeonDialogue();
                    d2.textEn = target.characterNameEn + "'s " + target.bodyParts[gm.currentBodyPartIndex].bodyPartName + " broke!";
                    currentDialogue.Add(d2);

                    StatusCondition[] scs = target.bodyParts[gm.currentBodyPartIndex].conditionsOnBreak;
                    foreach (StatusCondition sc in scs)
                    {
                        currentDialogue.Add(new DungeonDialogue(
                            "BREAK! " + target.characterNameEn + " " + sc.amount + "x " + sc.stat + " for " + sc.turns + " turns",
                            "japanese stuff here"
                        ));
                        var effectClone = sc;
                        target.statusConditions.Add(effectClone);
                    }
                }
            }

            // check if dead
            if (target.currentHP <= 0)
            {
                // create knockout dialogue
                DungeonDialogue d2 = new DungeonDialogue();
                d2.textEn = target.characterNameEn + " was defeated!";
                currentDialogue.Add(d2);

                // give exp and destroy character if they are an enemy
                if (target.isEnemy)
                {
                    CombatEXP(target.expDrop, target.LV);

                    target.display.gameObject.SetActive(false);

                    DungeonDialogue death = new DungeonDialogue();
                    death.command = "ENEM_DIED";
                    death.battler = target;
                    currentDialogue.Add(death);
                }
            }
        }
        // if the attack misses
        else
        {
            d.textEn = "Attack missed!";
            currentDialogue.Add(d);
        }
    }

    private void PerformHealItemOnTarget(PartyMember target, bool unmissable)
    {
        Debug.Log(target.characterNameEn + " is being attacked");

        // create a new dialogue object to show the result
        DungeonDialogue d = new DungeonDialogue();

        // calculate the hitrate
        float hitrate = 0;
        if (!unmissable)
        {
            if (gm.currentBodyPartIndex == -1) hitrate = gm.currentHitrates[0] * 100;
            else hitrate = gm.currentHitrates[1] * 100;
        }

        if (unmissable) hitrate = 999f;

        Debug.Log("hitrate: " + hitrate);

        if (UnityEngine.Random.Range(0, 100) <= hitrate) // this is a successful hit
        {
            int amountHP = 0;
            int amountMP = 0;

            if (gm.itemToUse.item.restoreHP > 0) // do attack damage if this action can deal damage
            {
                if (gm.itemToUse.item.restoreSetAmount)
                {
                    amountHP = gm.itemToUse.item.restoreHP;
                }
                else
                {
                    amountHP = Convert.ToInt32(target.maxHP * (gm.itemToUse.item.restoreHP / 100f));
                }

                d.textEn = "Restored " + amountHP + " HP from " + target.characterNameEn + "!";
                target.currentHP += amountHP;

                if (target.currentHP > target.maxHP) {
                    target.currentHP = target.maxHP;
                }

                // add the dialogue
                currentDialogue.Add(d);
            }

            if (gm.itemToUse.item.restoreMP > 0) // do attack damage if this action can deal damage
            {
                if (gm.itemToUse.item.restoreSetAmount)
                {
                    amountMP = gm.itemToUse.item.restoreMP;
                }
                else
                {
                    amountMP = Convert.ToInt32(target.maxMP * (gm.itemToUse.item.restoreMP / 100f));
                }

                if (amountHP > 0)
                {
                    currentDialogue.Add(new DungeonDialogue(
                            "Restored " + amountMP + " MP from " + target.characterNameEn + "!",
                            "insert japanese stuff here"
                        ));
                }
                else
                {
                    d.textEn = "Restored " + amountMP + " MP from " + target.characterNameEn + "!";
                    target.currentMP += amountMP;

                    currentDialogue.Add(d);
                }

                if (target.currentMP > target.maxMP)
                {
                    target.currentMP = target.maxMP;
                }
            }

            // add status effects to target
            if (gm.itemToUse.item.equipmentAction.statusConditions.Count() > 0) { 
                foreach (StatusCondition sc in gm.itemToUse.item.equipmentAction.statusConditions)
                {
                    currentDialogue.Add(new DungeonDialogue(
                        "Inflicted " + sc.amount + "x " + sc.stat + " on " + target.characterNameEn + " for " + sc.turns + " turns",
                        "japanese stuff here"
                    ));
                    var effectClone = sc;
                    target.statusConditions.Add(effectClone);
                }
            }

            // check if dead
            if (target.currentHP <= 0)
            {
                // create knockout dialogue
                DungeonDialogue d2 = new DungeonDialogue();
                d2.textEn = target.characterNameEn + " was defeated!";
                currentDialogue.Add(d2);

                // give exp and destroy character if they are an enemy
                if (target.isEnemy)
                {
                    CombatEXP(target.expDrop, target.LV);

                    target.display.gameObject.SetActive(false);

                    DungeonDialogue death = new DungeonDialogue();
                    death.command = "ENEM_DIED";
                    death.battler = target;
                    currentDialogue.Add(death);
                }
            }
        }
        // if the attack misses
        else
        {
            d.textEn = "Item missed!";
            currentDialogue.Add(d);
        }
    }

    private void GiveSelfStatusesFromAttack()
    {

        // add status effects to user
        foreach (StatusCondition sc in gm.currentAction.addtionalStatusOnUser)
        {
            currentDialogue.Add(new DungeonDialogue(
                gm.currentBattler.characterNameEn + " gained " + sc.amount + "x " + sc.stat + " for " + sc.turns + " turns",
                "japanese stuff here"
            ));
            var effectClone = sc;
            gm.currentBattler.statusConditions.Add(effectClone);
        }

        // modify VIZ
        if (gm.currentAction.setVIZ == true)
        {
            gm.currentBattler.VIZ += gm.currentAction.gainVIZ;
        }
        else
        {
            gm.currentBattler.VIZ = (int)(gm.currentBattler.VIZ* (1f+((float)gm.currentAction.gainVIZ/100)) );
        }

        // gain BP
        gm.BP += gm.currentAction.gainBP;
    }

    private void GiveSelfStatusesFromItem()
    {
        // modify VIZ
        if (gm.itemToUse.item.equipmentAction.setVIZ == true)
        {
            gm.currentBattler.VIZ += gm.itemToUse.item.equipmentAction.gainVIZ;
        }
        else
        {
            gm.currentBattler.VIZ = (int)(gm.currentBattler.VIZ * (1f + ((float)gm.itemToUse.item.equipmentAction.gainVIZ / 100)));
        }

        if (gm.itemToUse.item.equipmentAction.addtionalStatusOnUser.Count() <= 0)
        {
            return;
        }

        // add status effects to user
        foreach (StatusCondition sc in gm.itemToUse.item.equipmentAction.addtionalStatusOnUser)
        {
            currentDialogue.Add(new DungeonDialogue(
                gm.currentBattler.characterNameEn + " gained " + sc.amount + "x " + sc.stat + " for " + sc.turns + " turns",
                "japanese stuff here"
            ));
            var effectClone = sc;
            gm.currentBattler.statusConditions.Add(effectClone);
        }

        

        // gain BP
        gm.BP += gm.currentAction.gainBP;
    }

    private void UpdatePartyUI()
    {
        ui.bpText.text = ""+gm.BP;

        UpdatePartyUISingle(0);

        if (gm.partyMembers.Count >= 2)
        {
            UpdatePartyUISingle(1);
        }
        else
        {
            ui.partyMemberUIs[1].SetEmpty(true);
        }

        if (gm.partyMembers.Count >= 3)
        {
            UpdatePartyUISingle(2);
        }
        else
        {
            ui.partyMemberUIs[2].SetEmpty(true);
        }

        if (gm.partyMembers.Count >= 4)
        {
            UpdatePartyUISingle(3);
        }
        else
        {
            ui.partyMemberUIs[3].SetEmpty(true);
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

                    ProgressTimeOnce();

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

                    // update eye or perfrom encounter
                    if (t.eventOnWalk == false && t.noEncounter == false)
                    {
                        gm.stepsSinceEyeChange++;
                        if (gm.stepsSinceEyeChange > dm.minimumStepsUntilEyeChange)
                        {
                            if (UnityEngine.Random.Range(0, 4) == 3)
                            {
                                gm.eyePhase++;
                                ui.eyeSprite.sprite = ui.eyeSprites[gm.eyePhase - 1];
                                gm.stepsSinceEyeChange = 0;
                            }
                        }

                        if (gm.eyePhase >= 4)
                        {
                            gm.eyePhase++;
                            ui.eyeSprite.sprite = ui.eyeSprites[gm.eyePhase - 1];
                            // start encounter
                            StartEncounter();
                        }
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

    public void StartEncounter()
    {
        inputState = DungeonInputControlState.Combat;
        combatReturnTo = CombatReturnTo.None;

        gm.inCombat = true;

        gm.BP = 0; // reset BP

        //disable environemnt stuff here

        EncounterObject encounter = dm.encounters[UnityEngine.Random.Range(0, dm.encounters.Count)]; // choose random encounter
        gm.enemies.Clear(); // clear the previous encounter
        foreach (PartyMember e in encounter.enemies)
        {
            var clone = Instantiate(e); // create clone of the enemy so it doesn't override during battle
            clone.isEnemy = true; // mark the clone as an enemy for combat
            gm.enemies.Add(clone); // add the clone to the current encounter
        }

        for (int i = 0; i < ui.combat.combatSprites.Length; i++) // adds the enemy sprites to the ui
        {
            if (i < gm.enemies.Count)
            {
                ui.combat.combatSprites[i].gameObject.SetActive(true); // enable enemy display
                ui.combat.combatSprites[i].enemySprite.sprite = gm.enemies[i].sprite; // update the sprite
                gm.enemies[i].display = ui.combat.combatSprites[i]; // connect the display to the clone
                ui.combat.combatSprites[i].selectIcon.SetActive(false);
            }
            else
            {
                ui.combat.combatSprites[i].gameObject.SetActive(false); // hide display if not enough enemies
            }
        }

        ui.combat.InitializeBattleOrder();
        gm.currentBattler = ui.combat.battlers[0];

        ui.combat.gameObject.SetActive(true);

        // reset VIZ
        foreach (PartyMember b in ui.combat.battlers)
        {
            b.VIZ = 500;
        }

        StartDialogueCombat(gm.battleStartDialogue);
    }

    public void StartCombatTurn()
    {
        Debug.Log("StartCombatTurn()");

        UpdatePartyUI();

        if (gm.enemies.Count == 0) // player wins
        {
            StartDialogueCombat(gm.battleCompleteDialogue);
        }

        else if (gm.PartyAlive() == false) // player loses
        {
            StartDialogueCombat(gm.gameOverDialogue);
        }

        else // still in combat
        {
            // tick down all status conditions
            if(gm.currentBattler.statusConditions.Count > 0)
            {
                List<StatusCondition> toRemove = new List<StatusCondition>();
                bool willRemove = false;

                foreach(StatusCondition s in gm.currentBattler.statusConditions)
                {
                    if(s.turns <= 1)
                    {
                        toRemove.Add(s);
                        willRemove = true;
                    }
                    else
                    {
                        s.turns -= 1;
                    }
                }

                if (willRemove)
                {
                    foreach(StatusCondition s in toRemove)
                    {
                        gm.currentBattler.statusConditions.Remove(s);
                    }
                }

            }


            if (gm.currentBattler.isEnemy)
            {
                currentDialogue.Clear();
                EnemyTurn();
            }
            else
            {
                if(gm.currentBattler.currentHP <= 0)
                {
                    currentDialogue.Clear();
                    currentDialogue.Add(new DungeonDialogue(
                        gm.currentBattler.characterNameEn + " is knocked out and can't act!",
                        "japanese translation here"
                    ));
                    StartDialogueCombat(currentDialogue);
                }
                else{
                    ui.combat.mainBox_Text.text = gm.currentBattler.characterNameEn + "'s turn.";

                    // set portrait depending on damage
                    if (gm.currentBattler.currentHP > gm.currentBattler.maxHP * 0.66) ui.combat.mainBox_Portrait.sprite = gm.currentBattler.portrait;
                    else if (gm.currentBattler.currentHP > gm.currentBattler.maxHP * 0.33) ui.combat.mainBox_Portrait.sprite = gm.currentBattler.damagedPortrait;
                    else ui.combat.mainBox_Portrait.sprite = gm.currentBattler.veryDamagedPortrait;

                    combatReturnTo = CombatReturnTo.None;
                    eventSystem.SetSelectedGameObject(ui.combat.mainBox_FirstButton);
                }
            }
        }
    }

    public void EnemyTurn()
    {
        Debug.Log("EnemyTurn()"); // Skipping enemy turn, not implemented yet");

        List<EquipmentAction> possibleActions = new List<EquipmentAction>();
        foreach (EnemyAction e in gm.currentBattler.enemyActions)
        {
            float hpPercent = (float)gm.currentBattler.currentHP / gm.currentBattler.maxHP;
            if (e.HpMin <= hpPercent && e.HpMax >= hpPercent)
            {
                Debug.Log("HP requirments met");

                float mpPercent = (float)gm.currentBattler.currentMP / gm.currentBattler.maxMP;
                if (e.MpMin <= mpPercent && e.MpMax >= mpPercent)
                {
                    Debug.Log("MP requirements met");
                    for (int i = 0; i < e.priority; i++)
                    {
                        possibleActions.Add(e.action);
                    }
                }
            }
        }

        if(possibleActions.Count == 0)
        {
            currentDialogue.Clear();
            currentDialogue.Add(new DungeonDialogue(
                gm.currentBattler.characterNameEn + " did nothing.",
                "japanese translation here"
            ));
            StartDialogueCombat(currentDialogue);
        }
        else
        {
            EquipmentAction actionToUse = possibleActions[UnityEngine.Random.Range(0, possibleActions.Count)];
            gm.currentAction = actionToUse;

            if (actionToUse.targetType == TargetType.OneEnemy)
            {
                int index = 0;
                List<EnemyAttackTarget> targets = new List<EnemyAttackTarget>();

                foreach (PartyMember p in ui.combat.battlers)
                {
                    if (!p.isEnemy && p.currentHP > 0)
                    {
                        targets.Add(new EnemyAttackTarget(p, index, index + p.VIZ));
                        Debug.Log(p.characterNameEn + " is a potential target (" + index + " to " + (index + p.VIZ) + ")");

                        index += p.VIZ;   
                    }

                }

                int rand = UnityEngine.Random.Range(0, index);
                Debug.Log("rand = " + rand);
                foreach (EnemyAttackTarget t in targets)
                {
                    if (rand <= t.maxPriority && rand >= t.minPriority)
                    {
                        gm.currentTarget = t.target;
                        Debug.Log(t.target.characterNameEn + " was selected as the target");
                        break;
                    }
                }



                if (gm.currentAction.dontTargetBodyPart == false)
                {
                    gm.currentBodyPartIndex = UnityEngine.Random.Range(0, gm.currentTarget.bodyParts.Count());
                }
                else
                {
                    gm.currentBodyPartIndex = -1;
                }

                gm.currentHitrates = gm.CalculateHitRate();



            }

            StartDialogueCombat(gm.currentAction.attackDialogue);
       

        }

        //ProgressCombatTurn();
    }

    public void DeclineInCombat()
    {
        if (combatReturnTo == CombatReturnTo.Main)
        {
            ui.combat.HideMenusForDialogue();
            ui.combat.mainBox.SetActive(true);
            eventSystem.SetSelectedGameObject(ui.combat.mainBox_FirstButton);
            combatReturnTo = CombatReturnTo.None;
        }
        else if (combatReturnTo == CombatReturnTo.ActSelect)
        {
            ui.combat.HideMenusForDialogue();
            foreach (PartyMember enem in gm.enemies)
            {
                enem.display.selectIcon.SetActive(false);
            }
            ui.combat.actBox.SetActive(true);
            ui.combat.actDescriptionBox.SetActive(true);
            eventSystem.SetSelectedGameObject(ui.combat.lastSelectedAction);
            combatReturnTo = CombatReturnTo.Main;
        }
        else if (combatReturnTo == CombatReturnTo.TargetSelect)
        {
            ui.combat.HideMenusForDialogue();
            ui.combat.targetSelectBox.SetActive(true);
            eventSystem.SetSelectedGameObject(ui.combat.targetSelectedButton);
            combatReturnTo = CombatReturnTo.ActSelect;
        }
        else if (combatReturnTo == CombatReturnTo.ItemSelect)
        {
            ui.combat.HideMenusForDialogue();
            ui.combat.itemsBox.SetActive(true);
            eventSystem.SetSelectedGameObject(ui.combat.itemsGrid.transform.GetChild(0).gameObject);
            combatReturnTo = CombatReturnTo.Main;
        }
    }


    public void RotateLeft()
    {
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

    public void ProgressTimeOnce()
    {
        gm.stepsSinceDayChange++;
        if (gm.stepsSinceDayChange >= 24) // day changes every 24 steps
        {
            gm.day++;
            gm.daysSinceMoonChange++;
            gm.stepsSinceDayChange = 0;

            // CALCULATE MONTH

            if (gm.month == 2 && gm.year % 4 != 0 && gm.day >= 28) // if februrary and not a leap year
            {
                gm.month++;
                gm.day = 1;
            }
            else if (gm.month == 2 && gm.year % 4 == 0 && gm.day >= 29) // if februrary and is a leap year
            {
                gm.month++;
                gm.day = 1;
            }
            else if (gm.month == 4 || gm.month == 6 || gm.month == 9 || gm.month == 11) // april (4), june (6), september (9), november (11)
            {
                if (gm.day >= 30)
                {
                    gm.month++;
                    gm.day = 1;
                }
            }
            else if (gm.day >= 31) // all other months are 31 days long
            {
                if (gm.month == 12)
                {
                    gm.month = 1;
                    gm.year++;
                }
                else
                {
                    gm.month++;
                }
                gm.day = 1;
            }

            // CALCULATE MOON PHASE

            if (gm.daysSinceMoonChange >= 4)
            {
                ProgressMoonPhase();
                gm.daysSinceMoonChange = 0;
            }

            // CALCULATE DAY OF WEEK

            gm.dayofWeek++;
            if (gm.dayofWeek >= 8) gm.dayofWeek = 1;

        }

        UpdateTimeUI();

    }

    public void ProgressMoonPhase()
    {
        switch (gm.moonPhase){
            case MoonPhase.NewMoon:
                gm.moonPhase = MoonPhase.WaxingCrescent; break;
            case MoonPhase.WaxingCrescent:
                gm.moonPhase = MoonPhase.FirstQuarter; break;
            case MoonPhase.FirstQuarter:
                gm.moonPhase = MoonPhase.WaxingGibbous; break;
            case MoonPhase.WaxingGibbous:
                gm.moonPhase = MoonPhase.FullMoon; break;
            case MoonPhase.FullMoon:
                gm.moonPhase = MoonPhase.WaningGibbous; break;
            case MoonPhase.WaningGibbous:
                gm.moonPhase = MoonPhase.ThirdQuarter; break;
            case MoonPhase.ThirdQuarter:
                gm.moonPhase = MoonPhase.WaningCrescent; break;
            case MoonPhase.WaningCrescent:
                gm.moonPhase = MoonPhase.NewMoon; break;
        }
    }

    public void UpdateTimeUI()
    {
        if (gm.dayofWeek == 1) ui.dayText.text = "Sunday";
        else if (gm.dayofWeek == 2) ui.dayText.text = "Monday";
        else if (gm.dayofWeek == 3) ui.dayText.text = "Tuesday";
        else if (gm.dayofWeek == 4) ui.dayText.text = "Wednsday";
        else if (gm.dayofWeek == 5) ui.dayText.text = "Thursday";
        else if (gm.dayofWeek == 6) ui.dayText.text = "Friday";
        else if (gm.dayofWeek == 7) ui.dayText.text = "Saturday";

        ui.dateText.text = gm.month + "/" + gm.day;

        switch (gm.moonPhase){
            case MoonPhase.NewMoon:
                ui.moonSprite.sprite = ui.newMoon; break;
            case MoonPhase.WaxingCrescent:
                ui.moonSprite.sprite = ui.waxingCrescent; break;
            case MoonPhase.FirstQuarter:
                ui.moonSprite.sprite = ui.firstQuarter; break;
            case MoonPhase.WaxingGibbous:
                ui.moonSprite.sprite = ui.waxingGibbous; break;
            case MoonPhase.FullMoon:
                ui.moonSprite.sprite = ui.fullMoon; break;
            case MoonPhase.WaningGibbous:
                ui.moonSprite.sprite = ui.waningGibbous; break;
            case MoonPhase.ThirdQuarter:
                ui.moonSprite.sprite = ui.thirdQuarter; break;
            case MoonPhase.WaningCrescent:
                ui.moonSprite.sprite = ui.waningCrescent; break;
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

public enum CombatReturnTo
{
    None, Main, ActSelect, TargetSelect, ItemSelect
}

public enum PlayerFacing{
    North,South,East,West
}