using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

using SFEditor.Characters.Data;
using SFEditor.Data;
using SFEditor.Inventory;
using SFEditor.Inventory.Data;

using SF.Inventory;


using SF.UIElements.Utilities;

using SF.Characters.Data;

namespace SFEditor
{
    public class SFInventoryEditor : EditorWindow
    {

        [SerializeField] private ItemDatabase _itemDatabase;
        [SerializeField] private CharacterDatabase _characterDatabase;
        [SerializeField] private StyleSheet _dataEditorStyleSheet;

        private VisualElement _root;
        private SFItemListView _itemListView;

        private TwoPaneSplitView _itemPaneView;
        private VisualElement _dataViewContainer;

        private Tab _itemTab;
        private ItemView _itemView;

        private Tab _characterTab;

        public void CreateGUI()
        {
           
            _root = rootVisualElement;
            _dataViewContainer = new();
            DataEditorTabView rootTab = new DataEditorTabView("Data Editor");

            _itemTab = new Tab();
            _itemTab.label = "Item Tab";
            rootTab.Add(_itemTab);

            _characterTab = new Tab();
            _characterTab.label = "Character Tab";
            rootTab.Add(_characterTab);

            _itemTab.Add(_itemPaneView);
            _root.Add(rootTab);
            
            if(_dataEditorStyleSheet != null)
                _root.styleSheets.Add(_dataEditorStyleSheet);
        }

        private void OnSelectionChanged(IEnumerable<object> selectedObjects)
        {
            if(!selectedObjects.Any())
                return;

            var itemDTO = selectedObjects.First() as ItemDTO;
            _dataViewContainer.Remove(_itemPaneView.Q<ItemView>());
            _itemView = CreateItemView(itemDTO);
            _itemView.style.flexGrow = 1;
            _dataViewContainer.Add(_itemView);
            _itemView.Bind(new SerializedObject(itemDTO));
            Selection.SetActiveObjectWithContext(itemDTO, null);
        }

        private ItemView CreateItemView<TItemDTO>(TItemDTO itemDTO) 
            where TItemDTO : ItemDTO
        {
            if(itemDTO is EquipmentDTO equipmentDTO)
                return new EquipmentView(equipmentDTO, _itemListView);

            return new ItemView(itemDTO, _itemListView);
        }
    }
}