using System;
using System.Collections.Generic;
using UnityEngine;

namespace SF.ItemModule
{
    using DataModule;
    
    [CreateAssetMenu(fileName = "Item Database", menuName = "SF/Data/Item Database")]
    public class ItemDatabase : SFDatabase<ItemDTO>
    {
        public readonly Dictionary<EquipmentType, List<EquipmentDTO>> EquipmentDictionary = new();
        
        /* Filtered Item Lists */
        
        [SerializeReference] public List<EquipmentDTO> Equipment = new();
        [SerializeReference] public List<EquipmentDTO> Weapons = new();
        [SerializeReference] public List<EquipmentDTO> Armor = new();

        public Action OnItemsFiltered;
        
        public void AddItem<TItemDTOType>(TItemDTOType itemDTO) where TItemDTOType : ItemDTO
        {
            if(itemDTO == null)
                return;

            base.AddData(itemDTO);

            switch(itemDTO)
            {
                case EquipmentDTO equipment:
                    Equipment.Add(equipment);
                    break;
            }
        }

        public void RemoveItem<TItemDTOType>(TItemDTOType itemDTO) where TItemDTOType : ItemDTO
        {
            if(itemDTO == null)
                return;

            base.RemoveData(itemDTO);

            switch(itemDTO)
            {
                case EquipmentDTO equipment:
                    Equipment.Remove(equipment);
                    break;
            }
        }

        public ItemDTO GetEquipment(int id, EquipmentType equipmentType)
        {
            switch (equipmentType)
            {
                case EquipmentType.Weapon:
                    return DataEntries[id];
                    //return EquipmentDictionary[equipmentType].First(equipmentDTO => equipmentDTO.ID == id);
                default: return null;
            }
        }
        
        public new ItemDTO this[int index]
        {
            get
            {
                var item =  DataEntries.Find(dto => dto.ID == index);
                if (item is EquipmentDTO equipment)
                    return equipment;

                return item;
            }
        }
    }
}
