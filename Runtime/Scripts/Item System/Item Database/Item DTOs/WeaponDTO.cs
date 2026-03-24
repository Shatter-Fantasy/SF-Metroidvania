using UnityEngine;

namespace SF.ItemModule
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "SF/Inventory/Weapon")]
    public class WeaponDTO : EquipmentDTO
    {
        public Weapon WeaponData;
    } 
}
