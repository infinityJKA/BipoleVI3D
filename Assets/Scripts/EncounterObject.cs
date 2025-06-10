using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Encounter", menuName = "Bipole VI/Encounter")]
public class EncounterObject : ScriptableObject
{
    public PartyMember[] enemies;
}