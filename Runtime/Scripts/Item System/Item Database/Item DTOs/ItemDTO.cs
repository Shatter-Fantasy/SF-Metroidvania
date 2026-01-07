using UnityEngine;
using SF.DataModule;

namespace SF.ItemModule
{
    /// <summary>
    /// The scriptable object data asset that makes keeps track of an item inside the item databases.
    /// </summary>
    [CreateAssetMenu(fileName = "New Item", menuName = "SF/Inventory/ItemData")]
    public class ItemDTO : DTOAssetBase
    {
        public Sprite ItemIcon;
        public GameObject Prefab;
        public ItemSubType ItemSubType;
        public ItemPriceDTO PriceData;

        public ItemDTO(string name = "New Item", string description = "I am a new Item")
        {
            Name = name;
        }
    }
}