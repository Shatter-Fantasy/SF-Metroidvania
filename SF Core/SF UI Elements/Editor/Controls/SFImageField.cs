
using System;

using UnityEditor.UIElements;

using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UIElements;

using SF.UIElements.Utilities;

namespace SFEditor.UIElements
{
    [UxmlElement]
    public partial class SFImageField : VisualElement
    {
        public Label Label = new("Image:");
        public VisualElement PreviewField = new();
        public Sprite Icon;

        public const string USSClassName = "image-field";
        public const string LabelUSSClassName = "image-label";
        public const string PreviewUSSClassName = "image-preview";

        /// <summary>
        /// This action is invoked when the sprite is changed in the selector window for the image field.
        /// </summary>
        public Action<Object> OnSelectionChanged;
        public Action<Object> OnSelectorClosed;

        public SFImageField()
        {
            AddToClassList("row");
            AddToClassList(USSClassName);
            CreateUI();
        }

        public SFImageField(string label) : this()
        {
            Label.text = label;
        }

        public void CreateUI()
        {
            OnSelectionChanged += SelectionChanged;
            OnSelectorClosed += SelectorClosed;

            this.AddChild(Label,LabelUSSClassName);
            this.AddChild(PreviewField.AddChild(new Label("None:"))
                , PreviewUSSClassName);
                

            RegisterCallback<MouseDownEvent>(evt => ShowObjectSelector());
        }

        private void ShowObjectSelector()
        {
            ObjectPickerExtensions.ShowObjectPicker<Sprite>(OnSelectionChanged, OnSelectorClosed);
        }

        private void SelectionChanged(Object sprite)
        {
            Icon = sprite as Sprite;
            PreviewField.style.backgroundImage = new StyleBackground(Icon);

            PreviewField.Q<Label>().text = (Icon) 
                ? Icon.name
                : "None:";
        }

        private void SelectorClosed(Object sprite)
        {
            // TODO:
        }
    }
}
