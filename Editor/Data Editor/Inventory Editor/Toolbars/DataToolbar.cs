using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

using SF.UIElements.Utilities;

namespace SFEditor.Data
{
    [UxmlElement]
    public partial class DataToolbar : Toolbar
    {
        /// <summary>
        /// This reprsents the base element type this custom element is based on.
        /// </summary>
        public static readonly string TypeUSSClassName = "toolbar";
        public static readonly string USSClassName = "data-" + TypeUSSClassName;
        public static readonly string ContentContainerUSSClassName = TypeUSSClassName + "__content-container";

        public VisualElement ContentContainer { get; protected set; } = new VisualElement().AddClass(ContentContainerUSSClassName);

        public static readonly string ContainerUSSClassName = "";


        public DataToolbar() 
        {
            Add(ContentContainer);

            this.AddClass(USSClassName);
        }

        public DataToolbar(string elementaName) : this()
        {

        }
    }
}
