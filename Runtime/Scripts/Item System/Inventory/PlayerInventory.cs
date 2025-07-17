using System;
using System.Collections.Generic;
using System.Linq;
using SF.DataManagement;
using SF.Inventory;
using SF.ItemModule;
using SF.Managers;
using UnityEngine;

namespace SF.InventoryModule
{
    [Serializable]
    public class PlayerInventory : ItemContainer
    {
        [NonSerialized] public List<Weapon> _filteredWeapons = new List<Weapon>();
        [NonSerialized] public List<Armor> _filteredArmor = new List<Armor>();
        
        private void Start()
        {
            MetroidvaniaSaveManager.PlayerInventory = this;
        }

        public override void AddItem(int itemID)
        {
            //var item = GameLoader.Instance?.ItemDatabase.GetEquipment(itemID,EquipmentType.Weapon);
            var item = GameLoader.Instance?.ItemDatabase[itemID];
            ItemData itemData = new ItemData();

            if (item is EquipmentDTO equipmentDTO)
                itemData = (Weapon)equipmentDTO;
            else
                itemData = item;
            
            Items.Add(itemData);
        }

        public void FilterInventory()
        {
            _filteredWeapons = Items
                .Where(item => (item is Weapon))
                .Cast<Weapon>()
                .ToList();
            
            _filteredArmor = Items
                .Where(item => (item is Armor))
                .Cast<Armor>()
                .ToList();
        }
    }
}
