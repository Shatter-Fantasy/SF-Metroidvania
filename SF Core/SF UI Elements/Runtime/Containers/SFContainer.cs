using UnityEngine.UIElements;

using SF.UIElements.Utilities;


namespace SF.UIElements
{
    [UxmlElement]
    public partial class SFContainer : VisualElement
    {
        public SFContainer() 
        {
            this.Rename("sf-container")
                .AddClass("sf-container");
        }
    }
}
