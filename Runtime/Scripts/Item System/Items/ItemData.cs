using System.Collections.Generic;
using SF.Inventory;
using UnityEngine;

namespace SF.ItemModule
{
    /// <summary>
    /// The representation of an item and its data sent between objects. 
    /// </summary>
    /// <remarks>
    /// The <see cref="ItemDTO"/> is the actual data asset in the project, while <see cref="ItemData"/> is used for
    /// passing the data that represents an item between objects.
    /// </remarks>
    [System.Serializable]
    public class ItemData
    {
        // TODO: Replace the ID, Name, and description with DescriptionDataDefinition.
        public int ID = 0;
        public string Name;
        public string Description;

        /*  WP Data Definitions - Note we got to find a reason able way to figure out what we are attempting to data bind to.
            Currently I am thinking of in the data editors have a method that gets all the data definitions and type checks them.
            Based on the type than we show the proper UI Elements in the editors.
            This will also allow adding a button in the data editor classes to add on custom data types on a per data object basis.
          
        /// <summary>
        /// The list of data that defines the current item.
        /// This is used to allow modular data to be set on a per item basis.
        /// Used for enchanting, upgrading, saving/loading inventory, and game data editors.
        /// </summary>
        [SerializeReference]
        public List<DataDefinition> ItemDefinitions = new List<DataDefinition>();
        */
        
        public ItemSubType ItemSubType;
        
        public ItemData() : this("New Item") { }
        public ItemData(string name = "New Item", string description = "New Item", int id = -1)
        {
            Name = name;
            Description = description;
            ID = id;
        }

        public virtual void Use() { }
        
        /* TODO: Implement override for GetHashCode and Equal(Object)
        public static bool operator ==(ItemData item1, ItemData item2)
        {
            if (item1 is null || item2 is null)
                return false;
            
            return item1.ID == item2.ID;
        }
        public static bool operator !=(ItemData item1, ItemData item2)
        {
            return !(item1 == item2);
        }
        */
        
        public static implicit operator ItemData(ItemDTO itemAsset) 
        {
            ItemData itemData = new ItemData();
            itemData.ID = itemAsset.ID;
            itemData.Name = itemAsset.Name;
            itemData.Description = itemAsset.Description;
            itemData.ItemSubType = itemAsset.ItemSubType;
            return itemData;
        }
    }

    /// <summary>
    /// The base class for any data definition. This is used for making modular data containers.
    /// <example>
    /// A group of DataDefinitions can be used to describe items, enemies, and so forth in a modular way to make
    /// creating editors, save/loading, and customization a lot easier.
    /// This also includes making runtime bindings for UIToolkit a lot easier. 
    /// </example>
    /// </summary>
    [System.Serializable]
    public abstract class DataDefinition
    {
    }
    
    /// <summary>
    /// Description data for anything in the game.
    /// </summary>
    [System.Serializable]
    public class DescriptionDataDefinition : DataDefinition
    {
        public string Name;
        public string ID;
        public string Description;
    }
}