using UnityEngine.UIElements;

using SF.Inventory;
using SFEditor.Inventory.Data;
using SF.UIElements.Utilities;
using static SF.UIElements.Utilities.SFCommonStyleClasses;

namespace SFEditor.Inventory
{
    [UxmlElement]
    public partial class EquipmentView : ItemView
    {
        
        private new const string GeneralDataSectionUXMLPath =
            "Packages/shatter-fantasy.sf-metroidvania/Editor/Data Editor/Common/Data Sections/GeneralDataEntrySection.uxml";
        
        protected EnumField _equipmentTypeField;

        public EquipmentView() { }

        public EquipmentView(EquipmentDTO equipmentDTO, SFItemListView itemListView = null) : base(equipmentDTO)
        {

        }
    }
}
