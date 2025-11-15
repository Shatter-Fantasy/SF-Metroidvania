using SFEditor.UIElements.Utilities;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

using SF.Characters.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;

namespace SFEditor.Characters.Data
{
    [UxmlElement]
    public partial class CharacterTab : DataView<CharacterDTO>
    {
        private VisualElement _rootDataElement;
        [SerializeField] private VisualTreeAsset _characterViewUXML;
        private string _characterViewUXMLPath = "Packages/shatter-fantasy.sf-metroidvania/Editor/Data Editor/Character Editor/Tab View/CharacterTab.uxml";

        private CharacterListView _characterListView;
        private CharacterDatabase _characterDatabase;

        public CharacterTab() 
        {
            InitView();         
        }

        public CharacterTab(CharacterDatabase database, VisualElement rootDataElement)
        {
            if (rootDataElement != null)
                _rootDataElement = rootDataElement;
            
            if(database != null)
                _characterDatabase = database;

            SFUIElementsFactory.InitializeSFStyles(this);

            if(_characterDatabase != null)
            {
                if(_characterDatabase.DataEntries.Count > 0)
                    _dataSource = _characterDatabase[0];

                InitView();
                InitListView();
            }
        }

        private void InitView()
        {
            // We can still init the TabView to prevent some logical errors.
            _characterViewUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_characterViewUXMLPath);

            if(_characterViewUXML == null)
                return;

            _characterViewUXML.CloneTree(this);
        }

        private void InitListView()
        {
            _characterListView = this.Q<CharacterListView>();
            _characterListView.InitDataListView(_characterDatabase);
            _characterListView.selectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(IEnumerable<object> selectedObjects)
        {
            var enumerable = selectedObjects as object[] ?? selectedObjects.ToArray();
            
            if(enumerable.Length  < 1)
                return;

            if (enumerable.First() is CharacterDTO dtoAsset)
            {
                _rootDataElement.Bind(new SerializedObject(dtoAsset));
                Selection.SetActiveObjectWithContext(dtoAsset, null);
            }
        }
    }
}
