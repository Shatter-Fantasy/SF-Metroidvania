using UnityEngine.UIElements;
using SF;

namespace SFEditor
{
    public abstract class DataView : BindableElement
    {
        protected Label _dataSectionHeader;
        protected const string DataSectionHeaderUSSClass = "data-section__header-label";
        protected DataView()
        {
            _dataSectionHeader = new Label();
            _dataSectionHeader.AddToClassList(DataSectionHeaderUSSClass);
        }
    }

    /// <summary>
    /// Data view for normal DTOBase classes
    /// </summary>
    /// <typeparam name="TDataType"></typeparam>
    public abstract class DataView<TDataType> : BindableElement where TDataType : DTOBase
    {

        protected TDataType _value;
        public TDataType value
        {
            get => _value;
            set
            {
                if(value == this.value)
                    return;

                var previous = this.value;
                SetValueWithoutNotify(value);

                using(var evt = ChangeEvent<TDataType>.GetPooled(previous, value))
                {
                    evt.target = this;
                    SendEvent(evt);
                }
            }
        }

        protected Label _dataSectionHeader;
        protected const string DataSectionHeaderUSSClass = "data-section__header-label";
        protected DataView()
        {
            _dataSectionHeader = new Label();
            _dataSectionHeader.AddToClassList(DataSectionHeaderUSSClass);
        }

        public abstract void SetValueWithoutNotify(TDataType newValue);
    }

    /// <summary>
    /// Data view for DTOAssetBase classes.
    /// </summary>
    /// <typeparam name="TDataType"></typeparam>
    public abstract class DataViewAsset<TDataType> : BindableElement where TDataType : DTOAssetBase
    {

        protected TDataType _value;
        public TDataType value
        {
            get => _value;
            set
            {
                if(value == this.value)
                    return;

                var previous = this.value;
                SetValueWithoutNotify(value);

                using(var evt = ChangeEvent<TDataType>.GetPooled(previous, value))
                {
                    evt.target = this;
                    SendEvent(evt);
                }
            }
        }

        protected Label _dataSectionHeader;
        protected const string DataSectionHeaderUSSClass = "data-section__header-label";
        protected DataViewAsset()
        {
            _dataSectionHeader = new Label();
            _dataSectionHeader.AddToClassList(DataSectionHeaderUSSClass);
        }

        public abstract void SetValueWithoutNotify(TDataType newValue);
    }
}
