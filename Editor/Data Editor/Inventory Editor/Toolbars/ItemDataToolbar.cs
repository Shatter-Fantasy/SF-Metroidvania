using SF.Inventory;

using UnityEditor.UIElements;

using UnityEngine.UIElements;

namespace SFEditor.Data
{
    [UxmlElement]
    public partial class ItemDataToolbar : Toolbar
    {
        private ItemDatabase _itemDatabase;

        private ItemSubType _itemSubTypeFilter;
        public ItemSubType ItemSubTypeFilter
        {
            get => _itemSubTypeFilter;
            set
            {
                var previous = _itemSubTypeFilter;

                // If the value was not changed don't invoke OnFilter actions or anythign else.
                if(previous == value)
                    return;

                _itemSubTypeFilter = value;

                if(_itemDatabase == null)
                    return;

                _itemDatabase.OnItemsFiltered?.Invoke();
            }
        }

        public ItemDataToolbar() { }

        public ItemDataToolbar(ItemDatabase itemDatabase)
        {
            EnumField _itemSubFilterField = new(ItemSubType.None) { label = "Item Sub Type Filter" };
            _itemSubFilterField.RegisterValueChangedCallback(evt =>
            {
                ItemSubTypeFilter = (ItemSubType)evt.newValue;
            });

            _itemDatabase = itemDatabase;

            Add(_itemSubFilterField);
        }
    }
}
