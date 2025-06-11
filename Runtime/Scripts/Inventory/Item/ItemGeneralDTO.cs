using UnityEngine;
using SF.DataModule;

namespace SF.Inventory
{
    [System.Serializable]
    public class ItemGeneralDTO : DTOBase
    {
        public string ItemGUID;
        public string ItemName = "New Item";
        public string ItemDescription = "This is an item";
        public Sprite ItemIcon;

        public ItemGeneralDTO() { }

        public ItemGeneralDTO(int iD = 0, string itemGUID = "", string itemName = "New Equipment", string itemDescription = "", Sprite itemIcon = null)
        {
            ID = iD;
            ItemGUID = itemGUID;
            ItemName = itemName;
            ItemDescription = itemDescription;
            ItemIcon = itemIcon;
        }
    }
}
