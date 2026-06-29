using SF.UIElements.Utilities;

using UnityEngine.UIElements;

namespace SF.UIElements
{
    [UxmlElement]
    public partial class SFCard : VisualElement
    {
        // This is the overall container element class name
        public const string USSClassName = "card";
        public const string USSImageClassName = "card-image";
        public const string USSHeaderClassName = "card-header";
        public const string USSHeaderTitleClassName = "card-header__title";
        public const string USSBodyClassName = "card-body";

        public Image CardImage = new();
        public VisualElement HeaderContainer = new();
        public Label HeaderTitle = new();
        public VisualElement BodyContainer = new();
        
        public SFCard()
        {
            this.AddClass(USSClassName);

            CreateUI();
        }

        public SFCard(VisualElement bodyContent, string cardTitle = "") : this()
        {
            BodyContainer.AddChild(bodyContent);
            if(!string.IsNullOrEmpty(cardTitle))
                HeaderTitle.text = cardTitle;
        }

        private void CreateUI()
        {
            this.AddChild(CardImage,USSImageClassName);

            this.AddChild(HeaderContainer, USSHeaderClassName)
                .AddChild(HeaderTitle, USSHeaderTitleClassName);

            this.AddChild(BodyContainer, USSBodyClassName);
        }
    }
}
