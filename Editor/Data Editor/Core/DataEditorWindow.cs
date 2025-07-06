using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

using SF.Characters.Data;
using SF.DataModule;
using SF.Inventory;
using SFEditor.Characters.Data;
using SFEditor.Inventory.Data;
using SFEditor.UIElements.Utilities;


namespace SFEditor.Data
{
    public class DataEditorWindow : EditorWindow
    {
        [Header("Databases")]
        [SerializeField] private ItemDatabase _itemDatabase;
        [SerializeField] private CharacterDatabase _characterDatabase;
        
        [Header("UI Assets")]
        [SerializeField] private StyleSheet _dataEditorStyleSheet;
        [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;
        
        private VisualElement _rootDataElement;
        
        private CharacterListView _characterListView;
        private SFItemListView _itemListView;
        
        
        private CharacterTab _characterEditorView;

        private DataView _selectedView;
        [MenuItem("SF/Data Editor")]
        public static void OpenDataEditor()
        {
            DataEditorWindow wnd = GetWindow<DataEditorWindow>();
            wnd.titleContent = new GUIContent("Data Editor");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            VisualElement veUXML = m_VisualTreeAsset.Instantiate();
            
            root.Add(veUXML);
            SFUIElementsFactory.InitializeSFStyles(root);
            
            
            _rootDataElement = root.Q<VisualElement>("root-datasource");
            
            InitListView();
        }
        
        private void InitListView()
        {                // When first opening the editor guarantee at least a data entry is binded.
            _rootDataElement.Bind(new SerializedObject(_characterDatabase.DataEntries[0]));
            
            if(_characterDatabase != null)
            {
                _characterListView = _rootDataElement.Q<CharacterListView>();
                _characterListView.InitDataListView(_characterDatabase);
                _characterListView.selectionChanged += OnSelectionChanged;

                _characterEditorView = _rootDataElement.Q<CharacterTab>();
                _characterEditorView = new CharacterTab(_characterDatabase, _rootDataElement);
            }

            if (_itemDatabase != null)
            {
                _itemListView = _rootDataElement.Q<SFItemListView>();
                _itemListView.InitDataListView(_itemDatabase);
                _itemListView.selectionChanged += OnSelectionChanged;
            }
        }
        
        private void OnSelectionChanged()
        {
            if(Selection.activeObject is not DTOAssetBase || _rootDataElement == null)
                return;
            
            if(Selection.activeObject is CharacterDTO)
                _rootDataElement.Bind(new SerializedObject( Selection.activeObject as CharacterDTO));
            else  if(Selection.activeObject is ItemDTO)
                _rootDataElement.Bind(new SerializedObject( Selection.activeObject as ItemDTO));
        }
        
        private void OnSelectionChanged(IEnumerable<object> selectedObjects)
        {
            var enumerable = selectedObjects as object[] ?? selectedObjects.ToArray();
            if(enumerable.Length  < 1)
                return;

            var dtoAsset = enumerable.First() as DTOAssetBase;

            if (dtoAsset is CharacterDTO characterDTO)
            {
                _rootDataElement.Bind(new SerializedObject(characterDTO));
                Selection.SetActiveObjectWithContext(characterDTO, null);
            }
            else if (dtoAsset is ItemDTO itemDTO)
            {
                _rootDataElement.Bind(new SerializedObject(itemDTO));
                Selection.SetActiveObjectWithContext(itemDTO, null);
            }
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }
    }
}