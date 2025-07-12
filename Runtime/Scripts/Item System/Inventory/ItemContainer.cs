using System;
using System.Collections.Generic;
using SF.Inventory;
using SF.ItemModule;
using SF.Managers;
using UnityEngine;

namespace SF.InventoryModule
{
    public class ItemContainer : MonoBehaviour
    {
        public List<ItemData> Items = new List<ItemData>();

        public void AddItem(int itemID)
        {
            var item = GameLoader.Instance?.ItemDatabase[itemID];
            
            if (item != null)
            {
                Items.Add(item);
            }
        }
    }
}
