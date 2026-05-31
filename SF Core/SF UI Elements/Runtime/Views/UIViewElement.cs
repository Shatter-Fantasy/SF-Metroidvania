using UnityEngine;
using UnityEngine.UIElements;

namespace SF.UIModule
{
    using SF.UIElements;
    
    [UxmlElement]
    public partial class UIViewElement : SFVisualElementBase, IUIView
    {
        public VisualElement RootElement { get; protected set; }
        
        [SerializeField] protected bool _hideOnAwake = true;
        
        public void Initialize(VisualElement rootElement, UIController uiController = null)
        {
            RootElement = rootElement;

            if(_hideOnAwake)
            {
                Hide();
            }
		    
            SetVisualElements();
            RegisterEventCallbacks();
        }
        
        /// <summary>
        /// Sets up the VisualElements for the UI. Override to customize.
        /// </summary>
        protected virtual void  SetVisualElements()
        {
  
        }
	    
        /// <summary>
        /// Registers callbacks for buttons in the UI. Override to customize.
        /// </summary>
        protected virtual void RegisterEventCallbacks()
        {

        }

        public void Dispose()
        {
            
        }

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
    }
}
