using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "Bipole VI/Equipment")]
public class Equipment : ScriptableObject
{
    public string equipmentName = "New Equipment";



}

public enum EquipmentType
{
    // primarily physical
    Sword, // cut
    Lance, // pierce
    Bludgeon, // staffs, clubs, etc
    Fists,
    Bow,
    Gun,
    // primarily magical
    Fire,
    Water,
    Ice,
    Bio,
    Light,
    Dark


}