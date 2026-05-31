using UnityEngine.UIElements;

namespace SF.UIElements
{
    [UxmlElement]
    public partial class SFButtonGroup: VisualElement
    {
        // This one is on hold do to Unity not supporting the :first-child and :last-child
        // This will be put into on GitHub for UI Elements needing Unity to add new features to UI Toolkit.

        public const string USSClassName = "button-group";

        public SFButtonGroup() 
        {
            AddToClassList(USSClassName);
        }

        public SFButtonGroup(Button[] buttons) : this()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                Add(buttons[i]);
            }
        }

        /// <summary>
        /// Adds a new button to the Button Group.
        /// </summary>
        /// <param name="button"></param>
        public void AddButton(Button button)
        {
            Add(button);
        }

        /// <summary>
        /// Adds a new button to the Button Group and also returns it to the calling method to allow for editing the newly created button properties.
        /// </summary>
        /// <returns></returns>
        public Button AddButton()
        {
            Button button = new Button();

            return button;
        }
    }
}
