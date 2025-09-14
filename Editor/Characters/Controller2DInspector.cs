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

            newInspector.Add(
                    new Button(LowerControllerToGround)
                    {
                        text = "Lower Controller To Ground",
                        tooltip = "Uses a raycast to see how far the character needs to be lowered for it to be grounded. "
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
                rgb.bodyType = RigidbodyType2D.Kinematic;
                rgb.useFullKinematicContacts = true;
            }
        }

        /// <summary>
        /// Casts a ray to using the platform mask layers as possible checks.
        /// Then lowers the controller object using the distance the ray travelled to reach the ground.
        /// </summary>
        private void LowerControllerToGround()
        {
            if (target is not Controller2D controller2D)
                return;
            
            RaycastHit2D hit2D = Physics2D.Raycast(
                controller2D.GetColliderBounds().BottomCenter(),
                Vector2.down,
                20, 
                controller2D.PlatformFilter.layerMask);

            controller2D.transform.position = hit2D.point + new Vector2(0,controller2D.Bounds.size.y / 2f);
        }
    }
}
