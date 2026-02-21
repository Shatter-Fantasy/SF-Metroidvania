using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace SFEditor.Data
{
    
    using SF.DataModule;
    /// <summary>
    /// The generic class for all DataList views to show data of type DTObase classes and allow them to be selected and edited in editor windows.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public abstract class DataListView<T,U> : ListView, 
        INotifyValueChanged<T> 
        where T : SFDatabase<U> 
        where U : DTOAssetBase
    {
        private T _value;
        /// <summary>
        /// This is the ScriptableObject database being used for this listview.
        /// </summary>
        public T value
        {
            get => _value;
            set => _value = value;
        }

        /// <summary>
        /// The folder path for data files.
        /// </summary>
        protected abstract string _dataFilePath { get; }

        protected ContextualMenuManipulator _dropDownMenu;

        public DataListView() { }

        public DataListView(T databaseValue) 
        {
            InitDataListView(databaseValue);
        }

        /// <summary>
        /// Used to manually Initialize the DataListView from external classes.
        /// Can be useful if you have a DataListView in a UXML file and want to to use and init data on it.
        /// </summary>
        public void InitDataListView(T databaseValue)
        {
            if(databaseValue == null)
                return;

            // Set the data source of the list to be the data entries in the loaded database.
            itemsSource = databaseValue.DataEntries;
            _value = databaseValue;
            showAddRemoveFooter = true;

            bindItem = (e, i) => BindItem(e as Label, i);
            makeItem += MakeItem;
            itemsAdded += OnItemsAdded;
            itemsRemoved += OnItemsRemoved;
            CreateAddItemDropDownMenu();
            overridingAddButtonBehavior += OnOverrideAddButton;

            this.style.height = new Length(100, LengthUnit.Percent);
        }

        protected virtual void BindItem(Label ve, int listIndex)
        {
            ve.text = value.DataEntries[listIndex].Name;
        }

        protected virtual VisualElement MakeItem()
        {
            Label dataLabel = new Label();

            return dataLabel;
        }

        /// <summary>
        /// This is only called when items are directly added through the view controller. 
        /// This is called from other ListView methods like after the AddItems method is called.
        /// </summary>
        /// <param name="enumerable"></param>
        protected virtual void OnItemsAdded(IEnumerable<int> enumerable)
        {
            Rebuild();
            RefreshItems();
        }

        /// <summary>
        /// When a ListView has items removed this function is called.
        /// </summary>
        /// <param name="enumerable"></param>
        protected virtual void OnItemsRemoved(IEnumerable<int> enumerable)
        {        
            string assetPath = "";
            
            foreach(int listIndex in enumerable)
            {
                // Don't need to remove from the default item list. The view controller does that for us.
                // We only need to worry about the non defualt item lists.

                assetPath = AssetDatabase.GetAssetPath(value.DataEntries[listIndex]);
                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Creates the DropDownMenu for the add new items button in the footer of the list view.
        /// </summary>
        protected virtual void CreateAddItemDropDownMenu()
        {
            // This is the button that Unity adds if the footer add button is enabled.
            Button addButton = this.Q<Button>("unity-list-view__add-button");

            _dropDownMenu = new ContextualMenuManipulator(e =>
            {
                e.menu.AppendAction($"Add {typeof(U).Name}",
                    action =>
                    {
                        var assetDTO = DTOAssetBase.CreateInstance(typeof(U).FullName) as U;
                        assetDTO.Name = "New Data Entry" + assetDTO.GUID;
                        assetDTO.name = assetDTO.Name;
                        // We don't do Count - 1 because we haven't added this to the list datbase yet.
                        assetDTO.ID = value.DataEntries.Count; 
                        AssetDatabase.CreateAsset(assetDTO, $"{_dataFilePath}{assetDTO.name}.asset");
                        value.AddData(assetDTO);
                        EditorUtility.SetDirty(value);
                        RefreshItems();
                        AssetDatabase.SaveAssets();
                    });
            });
            _dropDownMenu.activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.LeftMouse,
                modifiers = UnityEngine.EventModifiers.None,
            });

            addButton.AddManipulator(_dropDownMenu);


        }

        /// <summary>
        /// Called when the ListView footer's add button is clicked. This is used to override the normal add logic.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="button"></param>
        private void OnOverrideAddButton(BaseListView view, Button button)
        {
            // When a ListView's overridingAddButtonBehavior has any actions in it Unity won't call onAdd and instead call
            // the overridingAddButtonBehavior methods.

            // We can leave this blank because the add button has a
            // ContextualMenuManipulator called _dropDownMenu registered to it. So when the button is clicked the 
            // _dropDownMenu handles all logic for the Add Button.
        }

        public void SetValueWithoutNotify(T newValue)
        {
            // TODO: Not Implemented

            if(newValue == null)
                return;

            throw new NotImplementedException();
        }
    }
}
