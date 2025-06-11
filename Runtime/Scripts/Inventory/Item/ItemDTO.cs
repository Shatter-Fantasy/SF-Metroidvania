using UnityEngine;
using SF.DataModule;

namespace SF.Inventory
{
    public enum ItemSubType
    {
        Consumable,
        Key,
        Equipment,
        None // This is used for when filtering items in different places.
    }


    [CreateAssetMenu(fileName = "New Item", menuName = "SF/Inventory/ItemData")]
    public class ItemDTO : DTOAssetBase
    {
        public Sprite ItemIcon;
        public GameObject Prefab;
        public ItemGeneralDTO GeneralInformation = new();
        public ItemSubType ItemSubType;
        public ItemPriceDTO PriceData;
    }
}