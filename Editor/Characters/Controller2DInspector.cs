using SF.PhysicsLowLevel;
using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine.LowLevelPhysics2D;
using UnityEngine.UIElements;


namespace SFEditor.Characters
{
    [CustomEditor(typeof(ControllerBody2D), true)]
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
            if (target is not ControllerBody2D controller2D)
                return;
            
            controller2D.ControllerBody.type = PhysicsBody.BodyType.Dynamic;
        }
    }
}
