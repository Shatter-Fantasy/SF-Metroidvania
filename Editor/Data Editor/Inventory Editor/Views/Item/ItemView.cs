using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

using SFEditor.UIElements.Utilities;
using SF.Inventory;

namespace SFEditor.Inventory.Data
{
    [UxmlElement]
    public partial class ItemView : DataView<ItemDTO>
    {
        /// <summary>
        /// The ListView that is currently being used to show all the Items in the database.
        /// </summary>
        protected SFItemListView _itemListView;

        protected VisualElement _generalDataSection = new();

        protected const string GeneralDataSectionUXMLPath =
            "Packages/shatter-fantasy.sf-metroidvania/Editor/Data Editor/Common/Data Sections/GeneralDataEntrySection.uxml";

        // Blank constructor is for the UxmlElement attribute and is needed to appear inside of the UI Builders asset library.
        public ItemView()
        {
            var generalDataUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(GeneralDataSectionUXMLPath);
            if (generalDataUXML != null)
            {
                _generalDataSection = generalDataUXML.Instantiate();
            }
        }

        public ItemView(ItemDTO itemDTO, SFItemListView itemListView = null) : this()
        {
            if(itemListView != null)
                _itemListView = itemListView;
            
            // This is a factory helper class that will set any style sheets defined as core styles in the SF UI Elements package.
            SFUIElementsFactory.InitializeSFStyles(this);

            Add(_generalDataSection);
        }
    }
}
