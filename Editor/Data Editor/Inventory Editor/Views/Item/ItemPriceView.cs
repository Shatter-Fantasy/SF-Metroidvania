using UnityEngine;
using UnityEngine.UIElements;

using SF.Inventory;

using SF.UIElements;
using SF.UIElements.Utilities;
using SFEditor.UIElements.Utilities;


namespace SFEditor.Inventory
{
    [UxmlElement]
    public partial class ItemPriceView : DataView, INotifyValueChanged<ItemPriceDTO>
    {
        private IntegerField _itemBuyPriceField; 
        private IntegerField _itemSellPriceField;
        private Toggle _itemCanBuyField;
        private Toggle _itemCanSellField;

        private ItemPriceDTO _value;
        public ItemPriceDTO value 
        { 
            get => _value;
            set
            {
                if(value == this.value)
                    return;

                var previous = this.value;
                SetValueWithoutNotify(value);

                using(var evt = ChangeEvent<ItemPriceDTO>.GetPooled(previous, value))
                {
                    evt.target = this;
                    SendEvent(evt);
                }
            }
        }

        public ItemPriceView() { }
        public ItemPriceView(ItemPriceDTO itemPriceDTO)
        {
            _dataSectionHeader.text = "Price Settings";         
            _itemCanBuyField = new Toggle() { label = "Can Buy"};
            _itemCanBuyField.RegisterValueChangedCallback(evt =>
            {
                _value.CanBuy = evt.newValue;
            });

            _itemCanSellField = new Toggle() { label = "Can Sell" };
            _itemCanSellField.RegisterValueChangedCallback(evt =>
            {
                _value.CanSell = evt.newValue;
            });

            _itemBuyPriceField = new IntegerField() { label = "Buy Price", bindingPath = "PriceData.BuyPrice" };
            _itemBuyPriceField.RegisterValueChangedCallback(evt => 
            {
                Debug.Log("");
                _value.BuyPrice = evt.newValue;
            });

            _itemSellPriceField = new IntegerField() { label = "Sell Price", bindingPath = "PriceData.SellPrice" };
            _itemSellPriceField.RegisterValueChangedCallback(evt =>
            {
                _value.SellPrice = evt.newValue;
            });

            this.AddChild(_dataSectionHeader
                )
               .AddChild(
                    new SFRow("two-column")
                        .AddChild(_itemCanBuyField)
                        .AddChild(_itemCanSellField)
                        .SetChildrenAsRowItems()
              ).AddChild(
                  new SFRow("two-column")
                        .AddChild(_itemBuyPriceField)
                        .AddChild(_itemSellPriceField)
                        .SetChildrenAsRowItems()
              );

            AddToClassList("data-section-view");
            // This is a factory helper class that will set any style sheets defined as core styles in the SF UI Elements package.
            SFUIElementsFactory.InitializeSFStyles(this);

            if(itemPriceDTO == null)
                return;

            SetValueWithoutNotify(itemPriceDTO);
        }

        public void SetValueWithoutNotify(ItemPriceDTO newValue)
        {
            if(newValue == null)
                return;

            _value = newValue;
            _itemCanBuyField.SetValueWithoutNotify(_value.CanBuy);
            _itemCanSellField.SetValueWithoutNotify(_value.CanSell);
            _itemBuyPriceField.SetValueWithoutNotify(_value.BuyPrice);
            _itemSellPriceField.SetValueWithoutNotify(_value.SellPrice);
        }
    }
}
