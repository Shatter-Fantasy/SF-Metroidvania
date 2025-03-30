using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine.UIElements;

using SF.UIElements;
using SF.UIElements.Utilities;
using SFEditor.Inventory;
using SF.Inventory;
using SFEditor.UIElements.Utilities;
using UnityEngine;


namespace SFEditor
{
    [UxmlElement]
    public partial class ItemView : DataView<ItemGeneralDTO>, INotifyValueChanged<ItemGeneralDTO>
    {
        /// <summary>
        /// The ListView that is currently being used to show all the Items in the database.
        /// </summary>
        protected SFItemListView _itemListView;

        private VisualElement _generalDataSection;

        protected TextField _itemGUIDField;
        protected TextField _itemNameField;
        protected TextField _itemDescriptionField;
        protected ObjectField _itemIconField;

        protected ItemPriceView _priceView;
        protected ItemDTO _itemDTO;

        // Blank constructor is for the UxmlElement attribute and is needed to appear inside of the UI Builders asset library.
        public ItemView() {}

        public ItemView(ItemDTO itemDTO, SFItemListView itemListView = null) : base()
        {
            _itemDTO = itemDTO;

            if(itemListView != null)
                _itemListView = itemListView;

            CreateGeneralSection();
            
            // This is a factory helper class that will set any style sheets defined as core styles in the SF UI Elements package.
            SFUIElementsFactory.InitializeSFStyles(this);

            Add(_generalDataSection);

            if(_itemDTO != null)
            {
                _priceView = new ItemPriceView(_itemDTO.PriceData);
                Add(_priceView);
                SerializedObject priceObj = new SerializedObject(_itemDTO);
                _priceView.Bind(priceObj);

                SetValueWithoutNotify(_itemDTO.GeneralInformation);
            }
        }

        public void CreateGeneralSection()
        {

            _generalDataSection = new();
            _dataSectionHeader.text = "General";

            _itemGUIDField = new TextField() { label = "GUID", bindingPath = "GeneralInformation.ItemGUID" };
            _itemGUIDField.RegisterValueChangedCallback(evt =>
            {
                _value.ItemGUID = evt.newValue;
            });

            _itemNameField = new TextField() { label = "Name", bindingPath = "GeneralInformation.ItemName" };
            _itemNameField.RegisterValueChangedCallback(evt =>
            {

                // TODO: Don't let a empty string be set as the name.
                // If it is empty the previous value can be set back as the
                // field's value using SetWithoutNotify at the end.

                _value.ItemName = evt.newValue;
                string assetPath = AssetDatabase.GetAssetPath(_itemDTO);
                AssetDatabase.RenameAsset(assetPath, _value.ItemName);
                AssetDatabase.SaveAssets();

                // Update the list view labels if there was a name changed.
                _itemListView.RefreshItems();
            });

            _itemDescriptionField = new TextField()
            {
                label = "Description",
                multiline = true,
                maxLength = 140,
                bindingPath = "GeneralInformation.ItemDescription"
            };
            _itemDescriptionField.RegisterValueChangedCallback(evt =>
            {
                _value.ItemDescription = evt.newValue;
            });

            _itemIconField = new ObjectField()
            {
                label = "Icon",
                objectType = typeof(Sprite),
            };
            _itemIconField.RegisterValueChangedCallback(evt =>
            {
                _value.ItemIcon = evt.newValue as Sprite;
            });

            _generalDataSection
                .AddChild(_dataSectionHeader)
                .AddChild(_itemGUIDField)
                .AddChild(
                new SFRow("two-column")
                    .AddChild(_itemIconField)
                    .AddChild(_itemNameField)
                    .SetChildrenAsRowItems()
                )
                .AddChild(_itemDescriptionField
                        .SetWidth(100, LengthUnit.Percent)
                );

            _generalDataSection.AddToClassList("data-section-view");
        }

        public override void SetValueWithoutNotify(ItemGeneralDTO newValue)
        {
            if(newValue == null)
                return;

            _value = newValue;

            // We don't want fields sending a change event so we use SetValueWithoutNotify
            // instead of just setting the fields value.
            _itemGUIDField.SetValueWithoutNotify(_value.ItemGUID);
            _itemNameField.SetValueWithoutNotify(_value.ItemName);
            _itemDescriptionField.SetValueWithoutNotify(_value.ItemDescription);
            _itemIconField.SetValueWithoutNotify(_value.ItemIcon);
        }
    }
}
