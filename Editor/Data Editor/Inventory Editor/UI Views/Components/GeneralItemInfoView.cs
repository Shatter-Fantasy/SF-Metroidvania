using SF.Inventory;

using UnityEngine.UIElements;

namespace SFEditor.Inventory
{
    [UxmlElement]
    public partial class GeneralItemInfoView : VisualElement
    {
        private ItemDTO _itemDTO;

        public void Init(ItemDTO itemDTO)
        {
            if(itemDTO != null)
                _itemDTO = itemDTO;
        }
    }
}
