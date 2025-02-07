using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;

#endif

using UnityEngine;
using UnityEngine.UIElements;

namespace SF
{

    [System.Serializable]
    public class ColorValuePair
    {
        public Color Color;
        public float Value;
    }

    [System.Serializable]
    public class FillColorBlock
    {
        /// <summary>
        /// A list of pairs that tell what a color should be at a certain value.
        /// </summary>
        public List<ColorValuePair> ColorSteps;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ColorValuePair))]
    public class ColorValuePairDrawerUIE : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {        
            var container = new VisualElement() { name = "Color Value Pair" };
            container.style.flexDirection = FlexDirection.Row;
            container.style.justifyContent = Justify.SpaceEvenly;

            var label = new Label("Color Step");
            var colorField = new PropertyField(property.FindPropertyRelative("Color")) { label = ""};
            var valueField = new PropertyField(property.FindPropertyRelative("Value")) { label = "" };

            colorField.style.width = new Length(50,LengthUnit.Percent);
            valueField.style.width = new Length(50,LengthUnit.Percent);

            container.Add(label);
            container.Add(colorField);
            container.Add(valueField);

            return container;
        }
    }
#endif

}
