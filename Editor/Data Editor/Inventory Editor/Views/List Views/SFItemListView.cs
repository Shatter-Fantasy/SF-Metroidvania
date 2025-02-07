using System.Collections.Generic;

using UnityEditor;
using UnityEngine.UIElements;

using SF.Inventory;
using System;
using UnityEngine;

namespace SFEditor.Inventory
{
    [UxmlElement]
    public partial class SFItemListView : ListView, INotifyValueChanged<ItemDatabase>
    {

        private ContextualMenuManipulator _dropDownMenu;

        private ItemDatabase _value;
        public ItemDatabase value
        {
            get => _value;
            set => _value = value;
        }

        private static string _rootDataAssetPath = "Assets/Data/Item Data";

        public SFItemListView() { }
        public SFItemListView(ItemDatabase itemDatabase) 
        {
            if(itemDatabase == null)
                return;
            
            itemsSource = itemDatabase.Items;
            showAddRemoveFooter = true;
            _value = itemDatabase;

            bindItem = (e,i) => BindItem(e as Label,i);
            makeItem += MakeItem;
            itemsAdded += OnItemsAdded;
            itemsRemoved += OnItemsRemoved;
            CreateAddItemDropDownMenu();
            overridingAddButtonBehavior += OnOverrideAddButton;

            this.style.height = new Length(100, LengthUnit.Percent);
        }

        /// <summary>
        /// Called when the ListView footer's add button is clicked. This is used to override the normal add logic.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="button"></param>
        private void OnOverrideAddButton(BaseListView view, Button button)
        {
            // When a ListViews overridingAddButtonBehavior has any actions in it Unity won't call onAdd and instead call
            // the overridingAddButtonBehavior methods.

            // We can leave this blank because the add button has a
            // ContextualMenuManipulator called _dropDownMenu registered to it. So when the button is clicked the 
            // _dropDownMenu handles all logic for the Add Button.
        }

        private void OnItemsRemoved(IEnumerable<int> enumerable)
        {
            string assetPath = "";
            foreach(int listIndex in enumerable)
            {
                // Don't need to remove from the default item list. The view controller does that for us.
                // We only need to worry about the non defualt item lists.

                if(value.Items[listIndex] is EquipmentDTO equipment)
                    value.Equipment.Remove(equipment);

                assetPath = AssetDatabase.GetAssetPath(value.Items[listIndex]);
                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Creates the DropDownMenu for the add new items button in the footer of the list view.
        /// </summary>
        private void CreateAddItemDropDownMenu()
        {
            // This is the button that Unity adds if the footer add button is enabled.
            Button addButton = this.Q<Button>("unity-list-view__add-button");


            _dropDownMenu = new ContextualMenuManipulator(e =>
            {
                e.menu.AppendAction("Add Item", 
                    action =>
                    {
                        var itemDTO = ItemDTO.CreateInstance("ItemDTO") as ItemDTO;
                        itemDTO.name = "New Item";
                        itemDTO.GeneralInformation.ItemName = "New Item";

                        if(GetOrCreateDataPath($"{_rootDataAssetPath}/Items", "Items"))
                        {
                            AssetDatabase.CreateAsset(itemDTO, $"Assets/Item Data/Items/{itemDTO.name}.asset");
                        }
                            
                        value.AddItem(itemDTO);
                        RefreshItems();
                    });

                e.menu.AppendAction("Add Equipment/Weapon",
                    action =>
                    {
                        var equipmentDTO = EquipmentDTO.CreateInstance("EquipmentDTO") as EquipmentDTO;
                        equipmentDTO.name = "New Weapon";
                        equipmentDTO.GeneralInformation.ItemName = "New Weapon";
                        equipmentDTO.EquipmentType = EquipmentType.Weapon;

                        if(GetOrCreateDataPath($"{_rootDataAssetPath}/Equipment", "Equipment"))
                        {
                            AssetDatabase.CreateAsset(equipmentDTO, 
                                $"Assets/Item Data/Equipment/{equipmentDTO.EquipmentType}/{equipmentDTO.name}.asset");
                        }
                        value.AddItem(equipmentDTO);
                        RefreshItems();
                    });
                e.menu.AppendAction("Add Equipment/Armor",
                    action =>
                    {
                        var equipmentDTO = EquipmentDTO.CreateInstance("EquipmentDTO") as EquipmentDTO;
                        equipmentDTO.name = "New Equipment";
                        equipmentDTO.GeneralInformation.ItemName = "New Equipment";
                        equipmentDTO.EquipmentType = EquipmentType.Armor;

                        if(GetOrCreateDataPath($"{_rootDataAssetPath}/Equipment", "Equipment"))
                        {
                            AssetDatabase.CreateAsset(equipmentDTO, 
                                $"Assets/Item Data/Equipment/{equipmentDTO.EquipmentType}/{equipmentDTO.name}.asset");
                        }
                        
                        value.AddItem(equipmentDTO);
                        RefreshItems();
                    });
            });
            _dropDownMenu.activators.Add(new ManipulatorActivationFilter 
            { 
                button = MouseButton.LeftMouse, 
                modifiers = EventModifiers.None 
            });

            addButton.AddManipulator(_dropDownMenu);
        }
        /// <summary>
        /// This is only called when items are directly added through the view controller. 
        /// This is called from other ListView methods like after the AddItems method is called.
        /// </summary>
        /// <param name="enumerable"></param>
        private void OnItemsAdded(IEnumerable<int> enumerable)
        {
            Rebuild();
            RefreshItems();
        }

        public void SetValueWithoutNotify(ItemDatabase newValue)
        {
            if(newValue == null)
                return;

            throw new NotImplementedException();
        }

        private VisualElement MakeItem()
        {
            Label itemLabel = new Label();

            return itemLabel;
        }

        private void BindItem(Label ve,int listIndex)
        {
            ve.text = value.Items[listIndex].GeneralInformation.ItemName;
        }

        private bool GetOrCreateDataPath(string parentDirectory, string newFolder)
        {
            // If the sub folder doesn't exist, make it.
            if(!AssetDatabase.AssetPathExists($"{parentDirectory}"))
                AssetDatabase.CreateFolder(parentDirectory, newFolder);

            // Return if the folder creation failed or succeeded. 
            return AssetDatabase.AssetPathExists($"{parentDirectory}/{newFolder}");
        }
    }
}
