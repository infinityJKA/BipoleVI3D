using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Inventory", menuName = "Inventory System/Inventory")]
public class inventoryObject : ScriptableObject
{
    public List<InventorySlot> Container = new List<InventorySlot>();
    public void AddItem(ItemObject _item, int _amount)
    {
        bool hasItem = false;
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].item == _item)
            {
                Container[i].AddAmount(_amount);
                hasItem = true;
                break;
            }
        }

        if (!hasItem)
        {
            Container.Add(new InventorySlot(_item, _amount));
        }
    }

    public void RemoveItem(ItemObject _item, int _amount)
    {
        int index = -1;
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].item == _item)
            {
                index = i;
            }
        }

        Container[index].amount -= _amount;
        if (Container[index].amount <= 0)
        {
            Container.RemoveAt(index);
        }
    }


    public int GetItemCount(ItemObject _item)
    {
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].item == _item)
            {
                return Container[i].amount;
            }
        }

        Debug.Log("Item not found while trying to get count");
        return 0;
    }

}

[System.Serializable]
public class InventorySlot
{
    public ItemObject item;
    public int amount;
    public InventorySlot(ItemObject _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }

    public void AddAmount(int v)
    {
        amount += v;
    }

}