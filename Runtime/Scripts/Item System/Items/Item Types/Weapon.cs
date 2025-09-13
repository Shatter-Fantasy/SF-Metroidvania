using SF.Inventory;
using UnityEngine;

namespace SF.ItemModule
{
	[System.Serializable]
	public class Weapon : ItemData
	{
		public WeaponType WeaponType;
		public HandAmount HandAmount;

		public Weapon() : this("New Weapon") { }
		public Weapon(string name = "New Weapon", string description = "I am a new weapon",
			WeaponType weaponType = WeaponType.Sword, HandAmount handAmount = default, int id = 0) : base(name, description, id)
		{
			WeaponType = weaponType;
			HandAmount = handAmount;
		}

		public override void Use()
		{
			
		}
		        
		public static implicit operator Weapon(EquipmentDTO equipmentAsset)
		{
			// Deep copy the weapon to prevent any reference types from being edited in the future.
			Weapon weapon = (Weapon)equipmentAsset.WeaponData.MemberwiseClone();
			// Might have to deep copy ItemPriceDTO as well.
			return weapon;
		}
		
		public static implicit operator Weapon(WeaponDTO equipmentAsset)
		{
			// Deep copy the weapon to prevent any reference types from being edited in the future.
			Weapon weapon = (Weapon)equipmentAsset.WeaponData.MemberwiseClone();
			// Might have to deep copy ItemPriceDTO as well.
			return weapon;
		}
	}
}
