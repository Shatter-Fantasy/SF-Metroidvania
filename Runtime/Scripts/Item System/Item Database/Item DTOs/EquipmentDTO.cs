using UnityEngine;

namespace SF.ItemModule
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
