using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SFEditor.ItemModule
{
    using DataModule;
    using SF.DataModule;
    using SF.ItemModule;
    [CustomPropertyDrawer(typeof(ItemData))]
    public class ItemDataDrawer : PropertyDrawer
    {
        private ItemDatabase _itemDB;
        private ItemData _itemData;

        private SerializedProperty _property;
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _itemDB   = DatabaseRegistry.GetDatabase<ItemDatabase>();
            _property = property;
            _itemData = _property.boxedValue as ItemData;
            
            if (_itemDB == null)
                return new Label("There was no ItemDatabase set, so can not pull up the list of items in a dropdown menu.");
            
            // We need to copy the prop or the NextVisible will move us out of the current property.
            SerializedProperty copyProp = property.Copy();

            VisualElement root = new VisualElement()
            {
                name = $"{property.name}_root"
            };
            DataAssetDropdownMenu<ItemDTO> itemDropDown = new DataAssetDropdownMenu<ItemDTO>(_itemDB,OnListItem,OnSelectedValue,_itemData.ID);

            root.Add(new PropertyField(copyProp));
            root.Add(itemDropDown);

            return root;
        }
        

        private string OnSelectedValue(ItemDTO dataEntry)
        {
            var item          = dataEntry;
            Debug.Log($"OnSelected Type: {item.GetType()},  DataEntry Type: {dataEntry.GetType()}");
            if (dataEntry != null)
            {
                _property.managedReferenceValue =  item;
                _property.serializedObject.ApplyModifiedProperties();
                _property.serializedObject.Update();
            }
            
            return dataEntry != null ? dataEntry.Name : "None";
        }

        private string OnListItem(ItemDTO dataEntry)
        {
            return dataEntry.Name;
        }
    }
    
  
}
