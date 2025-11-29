using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SF.DataManagement;
using SF.Managers;

namespace SF.ItemModule
{
    [Serializable]
    public class PlayerInventory : ItemContainer
    {
        [NonSerialized] public List<ItemData> FilteredConsumable = new List<ItemData>();
        [NonSerialized] public List<Weapon> FilteredWeapons = new List<Weapon>();
        [NonSerialized] public List<Armor> FilteredArmor = new List<Armor>();

        /// <summary>
        /// Invoked event when an item has been picked up off the ground.
        /// Does not activate when gaining items from shops, quest rewards, or from NPC dialogue interactions.
        /// </summary>
        public static Action<int> ItemPickedUpHandler;
        
        private void Start()
        {
            MetroidvaniaSaveManager.PlayerInventory = this;
        }
        
        public override void AddItem(int itemID)
        {
            var item = GameLoader.Instance?.ItemDatabase[itemID];
            ItemData itemData = new ItemData();

            if (item is WeaponDTO equipmentDTO)
                itemData = (Weapon)equipmentDTO;
            else
                itemData = item;
            
            Items.Add(itemData);
        }

        public void PickUpItem(int itemID)
        {
            AddItem(itemID);
            ItemPickedUpHandler?.Invoke(itemID);
        }

        public void FilterInventory()
        {
            FilteredConsumable = Items
                .Where(data => data.ItemSubType == ItemSubType.Consumable)
                .ToList();
            
            FilteredWeapons = Items.OfType<Weapon>().ToList();
            FilteredArmor = Items.OfType<Armor>().ToList();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Items Count: {Items.Count} ");
            sb.Append($"Consumables Count: {FilteredConsumable.Count} ");
            sb.Append($"Weapons Count: {FilteredWeapons.Count} ");
            sb.Append($"Armor Count: {FilteredArmor.Count} ");
            return sb.ToString();
        }
    }
}
