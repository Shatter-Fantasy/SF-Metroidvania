using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace SFEditor.DataModule
{
    using SF.DataModule;
    
    [UxmlElement]
    public partial class DataAssetDropdownMenu<TDataType> : VisualElement where TDataType : DTOAssetBase
    {
        [NonSerialized] private List<Type> _optionsTypes;
        [NonSerialized] private PopupField<TDataType> _dataDropdownMenu;

        public Func<TDataType, string> FormatListItemCallback;
        public Func<TDataType, string> FormatSelectedValueCallback;
            
        private SFAssetDatabase<TDataType> _database;
        private int _startingValueDataID;
        
        public DataAssetDropdownMenu(){ }
        public DataAssetDropdownMenu(SFAssetDatabase<TDataType> database, 
            Func<TDataType, string> onListItem, 
            Func<TDataType, string> onSelectedValue,
            int startingValueDataID = 0)
        {

            FormatListItemCallback      = onListItem;
            FormatSelectedValueCallback = onSelectedValue;
            _startingValueDataID         = startingValueDataID;
            
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
            _database.GetDataByID(_startingValueDataID, out var startingData);
                
            
            if (startingData == null)
                startingData = _database.DataEntries.ElementAt(0);

            int startingIndex = _database.GetDataIndexInDB(startingData);
            _dataDropdownMenu       = new PopupField<TDataType>(_database.DataEntries,startingData,FormatSelectedValueCallback,FormatListItemCallback);
            _dataDropdownMenu.index = startingIndex;
        }
    }
}
