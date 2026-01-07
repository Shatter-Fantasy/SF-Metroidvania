using System;
using System.Collections.Generic;
using SF.PhysicsLowLevel;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.Overlays;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.UIElements;

namespace SFEditor.PhysicsLowLevel
{
    [EditorTool("Edit SF Shape Component", typeof(SFShapeComponent))]
    public sealed class SFShapeSceneEditorTool : EditorTool, IDrawSelectedHandles
    {
        private readonly List<SFShapeComponent> _sceneShapeComponents = new ();
        private List<ShapeComponentGeometryTool> _shapeGeometryTools = new ();
        
        private SFShapeGeometryOverlay _overlay;
        
        // Use SceneShapeEditorTool as a reference for figuring this out.
        public override void OnActivated()
        {
           // TODO: Create the Geometry Tool Overlay that inherits from Overlay class.
           /*
           _overlay = new SFShapeGeometryOverlay();
           SceneView.AddOverlayToActiveView(_overlay);
           */
           
           
           // Create a new list of selected SFShapeComponent targets to be edited.
           _shapeGeometryTools = new List<ShapeComponentGeometryTool>(capacity: 1);
           
           foreach (var toolTarget in  targets)
           {
               if (toolTarget is SFShapeComponent shapeComponent)
               {
                   /* TODO: Create a foreach statement to go through the types of SFShapeComponents
                        and check for the correct SFSceneShapeGeometryTool to use for it.
                        Note we might want to use some type utility methods to get a list of all subclass types that inherit from
                        SFShapeComponent to make this easier.                 */

                   if (shapeComponent.isActiveAndEnabled)
                   {
                       // TODO: Replace the shapeComponent instead with the declared type that inherits from SFShapeComponent
                       _sceneShapeComponents.Add(shapeComponent);
                       // TODO: Make the tool for the found SFShapeComponent type.
                       _shapeGeometryTools.Add(CreateTool(shapeComponent));
                   }
               }
           }
        }

        /// <summary>
        /// Draws the user interface and handles used for editing tools in scene view.
        /// </summary>
        /// <param name="window"></param>
        public override void OnToolGUI(EditorWindow window)
        {
            if (_shapeGeometryTools == null)
                return;

            for (int i = 0; i < _shapeGeometryTools.Count; i++)
            {
                if (!_shapeGeometryTools[i].UpdateTool())
                {
                    _shapeGeometryTools[i] = CreateTool(_sceneShapeComponents[i]);
                    _shapeGeometryTools[i].UpdateTool();
                }
                
                // Let the tool handle the callback if it's valid.
                if (_shapeGeometryTools[i].IsValid)
                    _shapeGeometryTools[i].OnToolGUI(window);
            }
            
            // If we're not playing then we should queue a player update.
            // NOTE: We do this, so we'll render the gizmos.
            if (!EditorApplication.isPlaying)
                EditorApplication.QueuePlayerLoopUpdate();
        }

        private ShapeComponentGeometryTool CreateTool(SFShapeComponent shapeComponent)
        {
            
            switch (shapeComponent)
            {
                case SFCircleShape circleShape:
                    return new CircleShapeGeometryTool(circleShape);
                case SFRectangleShape rectangleShape:
                    return new RectangleGeometryTool(rectangleShape);
                case SFCapsuleShape capsuleShape:
                    return new CapsuleShapeGeometryTool(capsuleShape);
            }

            return new PolygonShapeGeometryTool(shapeComponent);
        }

        public void OnDrawHandles()
        {
            for (int i = 0; i < _shapeGeometryTools.Count; i++)
            {
                _shapeGeometryTools[i].OnDrawHandles();
            }
        }
    }


    /// <summary>
    /// The overlay for shape geometry editing tools.
    /// </summary>
    public class SFShapeGeometryOverlay : Overlay
    {
        // Use SceneGeometryToolOverlay as a reference for creating the overlay class.
        public override VisualElement CreatePanelContent()
        {
            return new Label("Shape Tool Overlay");
        }
    }
}
