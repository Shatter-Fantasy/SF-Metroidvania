using SF.ItemModule;
using UnityEngine;

namespace SF.Inventory
{
    public enum EquipmentType
    {
        Weapon, Armor, Jewelry
    }

    [CreateAssetMenu(fileName = "New Equipment", menuName = "SF/Inventory/Equipment")]
    public class EquipmentDTO : ItemDTO
    {
        public Weapon WeaponData;
        public EquipmentType EquipmentType;
    }
}
