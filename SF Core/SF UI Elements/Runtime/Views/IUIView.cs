using UnityEngine.UIElements;

namespace SF.UIModule
{
    public interface IUIView
    {
        // Properties
        public VisualElement RootElement { get; }
        public bool IsHidden => RootElement?.style.display == DisplayStyle.None;

        public void Initialize(VisualElement rootElement, UIController uiController = null);

        /// <summary>
        /// Shows the defined root element of the <see cref="UIView"/>.
        /// This sets the root element's style display to flex.
        /// </summary>
        public void Show()
        {
            if(RootElement != null)
                RootElement.style.display = DisplayStyle.Flex;
        }

        /// <summary>
        /// Hides the defined root element of the <see cref="UIView"/>.
        /// /// This sets the root element's style display to none.
        /// </summary>
        public void Hide()
        {
            if(RootElement != null)
                RootElement.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// Unregisters any callbacks or event handlers.
        /// </summary>
        public void Dispose();
    }
}
