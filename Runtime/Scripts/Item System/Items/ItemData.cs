using SF.Inventory;

namespace SF.ItemModule
{
    /// <summary>
    /// IMPORTANT: At the moment the <see cref="ItemGeneralDTO"/> does something similar to this.
    /// I need to make them into a single class for the future, instead of needing two classes.
    /// </summary>
    [System.Serializable]
    public class ItemData
    {
        // TODO: IMPORTANT: At the moment the <see cref="ItemGeneralDTO"/> does something similar to this.
        //  I need to make them into a single class for the future, instead of needing two classes.
        
        public int ID = 0;
        public string Name = "New Item";
        public string Description = "New Description";

        public static implicit operator ItemData(ItemDTO itemAsset) 
        {
            ItemData itemData = new ItemData();
            itemData.ID = itemAsset.ID;
            itemData.Name = itemAsset.Name;
            itemData.Description = itemAsset.Description;
            return itemData;
        }
    }
}