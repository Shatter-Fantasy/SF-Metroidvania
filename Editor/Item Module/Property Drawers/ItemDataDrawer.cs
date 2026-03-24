using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SFEditor.ItemModule
{
    using DataModule;
    using SF.DataModule;
    using SF.ItemModule;
    [CustomPropertyDrawer(typeof(ItemDTO))]
    public class ItemDataDrawer : PropertyDrawer
    {
        private ItemDatabase _itemDB;
        private ItemData _itemData;
        private ItemDTO _itemDTO;
        private SerializedProperty _property;
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _itemDB   = DatabaseRegistry.GetDatabase<ItemDatabase>();
            _property = property;
            _itemDTO  = _property.boxedValue as ItemDTO;
            
            if (_itemDB == null)
                return new Label("There was no ItemDatabase set, so can not pull up the list of items in a dropdown menu.");
            
            // We need to copy the prop or the NextVisible will move us out of the current property.
            SerializedProperty copyProp = property.Copy();

            VisualElement root = new VisualElement()
            {
                name = $"{property.name}_root"
            };
            DataAssetDropdownMenu<ItemDTO> itemDropDown = new DataAssetDropdownMenu<ItemDTO>(_itemDB,OnListItem,OnSelectedValue,
                _itemDTO != null ? _itemDTO.ID : 0);

            root.Add(new PropertyField(copyProp));
            root.Add(itemDropDown);

            return root;
        }
        

        private string OnSelectedValue(ItemDTO dataEntry)
        {
            if (dataEntry != null)
            {
                _property.objectReferenceValue =  dataEntry;
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
