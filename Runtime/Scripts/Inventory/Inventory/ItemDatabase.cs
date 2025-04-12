using System;
using System.Collections.Generic;

using UnityEngine;

namespace SF.Inventory
{
    [CreateAssetMenu(fileName = "Item Database", menuName = "SF/Inventory/Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeReference] public List<ItemDTO> Items = new();

        [SerializeReference] public List<EquipmentDTO> Equipment = new();

        public Action OnItemsFiltered;
        public Action OnEquipmentFiltered;


        public void AddItem<TItemDTOType>(TItemDTOType itemDTO) where TItemDTOType : ItemDTO
        {
            if(itemDTO == null)
                return;

            Items.Add(itemDTO);

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

            Items.Remove(itemDTO);

            switch(itemDTO)
            {
                case EquipmentDTO equipment:
                    Equipment.Remove(equipment);
                    break;
            }
        }
        
        public ItemDTO this[int index]
        {
            get { return Items.Find(dto => dto.ID == index); }
        }
    }
}
