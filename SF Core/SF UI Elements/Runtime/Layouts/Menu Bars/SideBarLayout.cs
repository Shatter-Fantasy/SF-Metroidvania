using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using static SF.UIElements.Utilities.VisualElementsExtensions;

namespace SF.UIElements
{
    [UxmlElement]
    public partial class SideBarLayout : VisualElement
    {
        public VisualElement SideBarMenu;
        public VisualElement ContentContainer;
        
        public const string USSClassName = "side-bar";
        public const string SideBarMenuUSSName = USSClassName + "__menu";
        public const string SideBarContainerUSSName = USSClassName + "__container";
        
        public SideBarLayout()
        {
            this.AddChild ( 
                // Start of SideBarMenu button group
                SideBarMenu = AddVisualElement(SideBarMenuUSSName)
                        .AddClass(SideBarMenuUSSName)
            ).AddChild ( 
                // Start of the main content container.
                ContentContainer = AddVisualElement(SideBarContainerUSSName)
                    .AddClass(SideBarContainerUSSName)
            )   // Start of root uss class names.
                .AddClass(USSClassName);
        }
        
        
        private void OnWelcomeLinkClicked(PointerDownLinkTagEvent evt)
        {
            Application.OpenURL(evt.linkText);
        }
    }
}
