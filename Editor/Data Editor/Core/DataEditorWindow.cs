using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

using SF.Characters.Data;
using SF.Inventory;
using SFEditor.Characters.Data;
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
            
            if(_characterDatabase != null)
            {
                InitListView();
            }
        }
        
        private void InitListView()
        {                // When first opening the editor guarantee at least a data entry is binded.
            _rootDataElement.Bind(new SerializedObject(_characterDatabase.DataEntries[0]));
            _characterListView = _rootDataElement.Q<CharacterListView>();
            _characterListView.InitDataListView(_characterDatabase);
            _characterListView.selectionChanged += OnSelectionChanged;
        }
        
        private void OnSelectionChanged()
        {
            if(Selection.activeObject is not CharacterDTO || _rootDataElement == null)
                return;

            _rootDataElement.Bind(new SerializedObject( Selection.activeObject as CharacterDTO));
        }
        
        private void OnSelectionChanged(IEnumerable<object> selectedObjects)
        {
            if(!selectedObjects.Any())
                return;

            var characterDTO = selectedObjects.First() as CharacterDTO;
            _rootDataElement.Bind(new SerializedObject(characterDTO));
            Selection.SetActiveObjectWithContext(characterDTO, null);
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