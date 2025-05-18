using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemsUI : MonoBehaviour
{
    // public int xBetweenItems, yBetweenItems, columns;
    public GameObject parentSpawnUnder;
    public ItemUIButton prefab;


    Dictionary<InventorySlot, ItemUIButton> itemsDisplayed = new Dictionary<InventorySlot, ItemUIButton>();

    void Start()
    {

    }

    void Update()
    {

    }

    public void CreateDisplay()
    {
        ItemType itype = ItemType.Consumable;

        // destroy old children first
        foreach (Transform child in parentSpawnUnder.transform)
        {
            Destroy(child.gameObject);
        }

        itemsDisplayed.Clear();

        inventoryObject inv = GameManager.gm.inventory;

        // spawn new display
        for (int i = 0; i < inv.Container.Count; i++)
        {
            if (inv.Container[i].item.itemType == itype)
            {
                ItemUIButton iui = Instantiate(prefab, parentSpawnUnder.transform);
                iui.itemInfo = inv.Container[i];
                iui.UpdateGraphic();
                itemsDisplayed.Add(inv.Container[i], iui);
            }
        }
    }
}
