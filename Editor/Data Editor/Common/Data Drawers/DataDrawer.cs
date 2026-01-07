using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SF.DataModule;
using SF.ItemModule;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SFEditor.DataModule
{
    [CustomPropertyDrawer(typeof(DataProperty))]
    public class DataDrawer : PropertyDrawer
    {
        private PopupField<ItemDTO> _typeDropdownMenu = new ("Items");
        private EnumField _itemTypeField = new("Item Type", ItemSubType.Equipment);

        private SerializedProperty _property;
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _property     = property;
            _itemTypeField.RegisterValueChangedCallback(OnItemTypeChanged);
            DropDownInit();
            
            _typeDropdownMenu.RegisterValueChangedCallback(OnItemSelected);
            
            VisualElement container = new();
            container.Add(_itemTypeField);
            container.Add(_typeDropdownMenu);
            container.Add(new PropertyField(property));
            return container;
        }

        private void OnItemTypeChanged(ChangeEvent<Enum> evt)
        {
            DropDownInit();
        }
        
        private void DropDownInit()
        {
            var database = DatabaseRegistry.GetDatabase<ItemDatabase>();
            if (database == null)
            {
                Debug.LogWarning("No Item Database was registered.");
                return;
            }

            switch ((ItemSubType)_itemTypeField.value)
            {
                case ItemSubType.Consumable:
                {
                    _typeDropdownMenu.choices = database.DataEntries;
                    break;
                }
                case ItemSubType.Key:
                    break;
                case ItemSubType.Equipment:
                {
                    _typeDropdownMenu.choices = database.Equipment.Cast<ItemDTO>().ToList();
                    break;
                }
                case ItemSubType.Material:
                    break;
                case ItemSubType.SideQuest:
                    break;
                case ItemSubType.MainQuest:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void OnItemSelected(ChangeEvent<ItemDTO> evt)
        {
            Debug.Log(((ItemData)evt.newValue).GetType());
            _property.boxedValue = (ItemData)evt.newValue;
        }
    }
}
