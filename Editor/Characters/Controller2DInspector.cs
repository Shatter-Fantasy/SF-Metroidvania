using SF.Characters.Controllers;
using SF.Platformer.Utilities;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;


namespace SFEditor.Characters
{
    [CustomEditor(typeof(Controller2D), true)]
    public class Controller2DInspector : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement newInspector = new();
            InspectorElement.FillDefaultInspector(newInspector, serializedObject, this);
            newInspector.Add(
                    new Button(SetupControllerComponents) 
                    { 
                        text = "Setup Character Controller",
                        tooltip = "Sets up the rigidbody2d and platform contact filters values. " +
                        "Note in the Platform and OneWayPlatform filter settings you still need " +
                        "to set the layers to filter with. "
                    }
                );
            return newInspector;
        }

        private void SetupControllerComponents()
        {
            if (target is not Controller2D controller2D)
                return;
            
            Rigidbody2D rgb = controller2D.GetComponent<Rigidbody2D>();
            if(rgb != null)
            {
                rgb.bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }
}
