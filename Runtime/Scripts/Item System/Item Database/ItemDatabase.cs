using System;
using System.Collections.Generic;
using SF.Characters.Data;
using UnityEngine;

namespace SF.Inventory
{
    [CreateAssetMenu(fileName = "Item Database", menuName = "SF/Data/Item Database")]
    public class ItemDatabase : SFDatabase<ItemDTO>
    {
        [SerializeReference] public List<EquipmentDTO> Equipment = new();

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
        
        public new ItemDTO this[int index]
        {
            get { return DataEntries.Find(dto => dto.ID == index); }
        }
    }
}
