using SF.Inventory;


namespace SF.ItemModule
{
    [System.Serializable]
    public class ItemData
    {
        public int ID = 0;
        public string Name;

        public static implicit operator ItemData(ItemDTO itemAsset) 
        {
            ItemData itemData = new ItemData();
            itemData.ID = itemAsset.ID;
            itemData.Name = itemAsset.Name;
            return itemData;
        }
    }
}