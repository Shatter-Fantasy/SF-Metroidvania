using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace SFEditor.DataModule
{
    using SF.DataModule;
    
    [UxmlElement]
    public partial class DataAssetDropdownMenu<TDataType> : VisualElement where TDataType : DTOAssetBase
    {
        [NonSerialized] private List<Type> _optionsTypes;
        [NonSerialized] private List<TDataType> _dataEntries = new List<TDataType>();
        [NonSerialized] private PopupField<TDataType> _dataDropdownMenu;

        public Func<TDataType, string> FormatListItemCallback;
        public Func<TDataType, string> FormatSelectedValueCallback;
            
        private SFAssetDatabase<TDataType> _database;
        private int _startingValueIndex;
        
        public DataAssetDropdownMenu(){ }
        public DataAssetDropdownMenu(SFAssetDatabase<TDataType> database, 
            Func<TDataType, string> onListItem, 
            Func<TDataType, string> onSelectedValue,
            int startingValueIndex = 0)
        {

            FormatListItemCallback      = onListItem;
            FormatSelectedValueCallback = onSelectedValue;
            
            if (database == null)
                return;
            
            _database = database;
            
            Initialize();
        }
        
        private void Initialize()
        {
            DropDownInit();
            Add(_dataDropdownMenu);
        }
        
        private void DropDownInit()
        {
            _database.GetDataByID(_startingValueIndex, out var startingData);

            if (startingData == null)
                startingData = _database[0];
            
            _dataDropdownMenu       = new PopupField<TDataType>(_database.DataEntries,startingData,FormatSelectedValueCallback,FormatListItemCallback);
            _dataDropdownMenu.index = 0;
        }
    }
}
