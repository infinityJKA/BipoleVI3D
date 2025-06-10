using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatUI : MonoBehaviour
{

    public DungeonUI ui;

    [Header("Combat Sprites")]
    public CombatEnemyDisplay[] combatSprites;

    [Header("Main Box")]
    public GameObject mainBox, mainBox_FirstButton;
    public TMP_Text mainBox_Text;
    public Image mainBox_Portrait;



}
