using UnityEngine.UIElements;

using SF.Inventory;
using SF.UIElements.Utilities;
using static SF.UIElements.Utilities.SFCommonStyleClasses;

namespace SFEditor.Inventory
{
    [UxmlElement]
    public partial class EquipmentView : ItemView, INotifyValueChanged<EquipmentDTO>
    {

        protected EnumField _equipmentTypeField;

        protected new EquipmentDTO _value;
        public new EquipmentDTO value 
        { 
            get => _value;
            set
            {
                if(value == this.value)
                    return;

                // Make a copy of the current value and set it as a previous value for change evtn sending.
                var previous = this.value;
                SetValueWithoutNotify(value);

                using(var evt = ChangeEvent<EquipmentDTO>.GetPooled(previous, value))
                {
                    evt.target = this;
                    SendEvent(evt);
                }
            }
        }

        public EquipmentView() { }

        public EquipmentView(EquipmentDTO equipmentDTO, SFItemListView itemListView = null) : base(equipmentDTO,itemListView)
        {
            // This constructor calls the base ItemView constructor to set up the base fields for all item types.
            // The base ItemView constructor also sets up the fields value change callbacks and binding paths.

            _equipmentTypeField = new EnumField() { label = "Equipment Type", bindingPath = "EquipmentType" };
            _equipmentTypeField.Init(EquipmentType.Weapon);
            _equipmentTypeField.RegisterValueChangedCallback(evt =>
            {
                _value.EquipmentType = (EquipmentType)evt.newValue;
            });

            this.AddChild(_equipmentTypeField, classNames: TwoColumnRow);

            if(equipmentDTO != null)
            {
                _value = equipmentDTO;
                _equipmentTypeField.SetValueWithoutNotify(_value.EquipmentType);
            }
        }

        public void SetValueWithoutNotify(EquipmentDTO newValue)
        {
            if(newValue != null)
            {
                _value = newValue;
                // We don't want fields sending a change event so we use SetValueWithoutNotify
                // instead of just setting the fields value.
                _equipmentTypeField.SetValueWithoutNotify(_value.EquipmentType);
            }
        }
    }
}
