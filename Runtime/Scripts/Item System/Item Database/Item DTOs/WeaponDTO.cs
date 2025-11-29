using UnityEngine;

namespace SF.ItemModule
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "SF/Inventory/Weapon")]
    public class WeaponDTO : ItemDTO
    {
        public Weapon WeaponData;
    } 
}
