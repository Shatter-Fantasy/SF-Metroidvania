using SFEditor.UIElements.Utilities;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

using SF.Characters.Data;
using SFEditor.Data;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor.UIElements;

namespace SFEditor.Characters.Data
{
    [UxmlElement]
    public partial class CharacterEditorView : DataViewAsset<CharacterDTO>, INotifyValueChanged<CharacterDTO>
    {
        [SerializeField] private VisualTreeAsset _characterViewUXML;
        private string _characterViewUXMLPath = "Packages/shatter-fantasy.sf-metroidvania/Editor/Data Editor/Character Editor/Tab View/CharacterTab.uxml";

        private CharacterListView _characterListView;
        private CharacterDatabase _characterDatabase;

        #region UI Fields
        private IntegerField _idField;
        private IntegerField _guidField;
        private TextField _nameField;
        private TextField _descriptionField;
        private ObjectField _prefabField;
        #endregion

        public CharacterEditorView() 
        {
            InitView();         
        }

        public CharacterEditorView(CharacterDatabase database) : base()
        {
            if(database != null)
                _characterDatabase = database;

            SFUIElementsFactory.InitializeSFStyles(this);

            if(_characterDatabase != null)
            {
                if(_characterDatabase.DataEntries.Count > 0)
                    _value = _characterDatabase[0];

                InitView();
                InitListView();
                if(_value != null)
                {
                    RegisterUIFields();
                    SetValueWithoutNotify(_value);
                }
            }
        }

        private void RegisterUIFields()
        {
            SerializedObject characterSerailized = new SerializedObject(_value);
            this.Bind(characterSerailized);

            _idField = this.Q<IntegerField>("id-field");
            _idField.RegisterValueChangedCallback(evt =>
            {
                _value.ID = evt.newValue;
            });

            _guidField = this.Q<IntegerField>("guid-field");
            _guidField.RegisterValueChangedCallback(evt =>
            {
                _value.GUID = evt.newValue;
            });


            _nameField = this.Q<TextField>("name-field");
            _nameField.RegisterValueChangedCallback(evt =>
            {
                _value.Name = evt.newValue;
                string assetPath = AssetDatabase.GetAssetPath(value);
                AssetDatabase.RenameAsset(assetPath, _value.Name);
                AssetDatabase.SaveAssets();

                _characterListView.RefreshItems();
            });

            _descriptionField = this.Q<TextField>("description-field");
            _descriptionField.RegisterValueChangedCallback(evt =>
            {
                _value.Description = evt.newValue;
            });

            _prefabField = this.Q<ObjectField>();
            _prefabField.RegisterValueChangedCallback(evt =>
            {
                _value.Prefab = (GameObject)evt.newValue;
            });
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
            if(!selectedObjects.Any())
                return;

            var characterDTO = selectedObjects.First() as CharacterDTO;
            //_itemView = CreateItemView(characterDTO);
            //_itemView.style.flexGrow = 1;
            //_dataViewContainer.Add(_itemView);
            this.Bind(new SerializedObject(characterDTO));
            SetValueWithoutNotify(characterDTO);
            Selection.SetActiveObjectWithContext(characterDTO, null);
        }

        public override void SetValueWithoutNotify(CharacterDTO newValue)
        {
            if(newValue == null)
                return;

            _value = newValue;

            _idField.SetValueWithoutNotify(_value.ID);
            _guidField.SetValueWithoutNotify(_value.GUID);
            _nameField.SetValueWithoutNotify(_value.Name);
            _descriptionField.SetValueWithoutNotify(_value.Description);
           
            _prefabField.SetValueWithoutNotify(_value.Prefab);
        }
    }
}
