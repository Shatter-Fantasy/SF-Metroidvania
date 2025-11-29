using SF.Inventory;

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
        public int ID = 0;
        public string Name;
        public string Description;

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
}