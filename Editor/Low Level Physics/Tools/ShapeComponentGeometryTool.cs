using SF.PhysicsLowLevel;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SFEditor.PhysicsLowLevel
{
    
    /// <summary>
    /// Inherit from <see cref="ShapeComponentGeometryTool"/> to create a new tool for editing
    /// a component that inherits from <see cref="SFShapeComponent"/>
    /// </summary>
    public abstract class ShapeComponentGeometryTool : IGeometryToolSettings
    {
        /// <summary>
        /// The local cached <see cref="PhysicsShape"/> that is being drawn by <see cref="PhysicsWorld"/> draw methods.
        /// </summary>
        protected PhysicsShape _shape;
        protected PhysicsBody _body;
        protected readonly SFShapeComponent _shapeComponent;
        protected PhysicsWorld _world;
        protected PhysicsWorld.TransformPlane _transformPlane;

        /// <summary>
        /// Has the target shaped been changed since the last time the tool was updated.
        /// </summary>
        protected bool _targetShapeChanged;
        
        protected ShapeComponentGeometryTool(SFShapeComponent shapeComponent)
        {
            
            _shapeComponent = shapeComponent;

            if (_shapeComponent == null)
                return;
            
            UpdateTool();
        }

        /// <summary>
        /// <example>
        /// <para>
        /// How to use <see cref="OnToolGUI"/>
        /// Step One: Get the shapes and geometry. 
        /// Step Two:Get the relative Transform
        /// Step Three:Set-up handles.
        /// Step Four:Set up any labels - use Handles.DrawingScope with the shape's transform to ToPosition3D for better label positioning.
        /// Step Five:Update the shape if handles change it
        /// Step Six:Draw a representation of the geometry and inform the physic world of of what it needs to draw.
        /// </para>
        /// </example>
        /// </summary>
        /// <param name="window"></param>
        public abstract void OnToolGUI(EditorWindow window);

        public virtual bool IsValid => _shapeComponent != null && _shape.isValid && _shapeComponent.isActiveAndEnabled;

        public bool UpdateTool()
        {
            _targetShapeChanged = false;

            if (_shape.isValid)
                return true;

            _shape = _shapeComponent.Shape;
            
            if (!_shape.isValid)
                return true;

            _body           = _shape.body;
            _world          = _shape.world;
            _transformPlane = _world.transformPlane;
            
            return true;
        }

#region Scene Handle Properties.
        public Color GrabHandleColor { get; set; } = Color.whiteSmoke;
        public Color LabelColor { get; set; } = Color.white;
        
        protected static string LabelFloatFormat => "F4";
#endregion

    }
    
    
    public class PolygonShapeGeometryTool : ShapeComponentGeometryTool
    {
        public PolygonShapeGeometryTool(SFShapeComponent shapeComponent) : base(shapeComponent)
        {
            
        }

        public override void OnToolGUI(EditorWindow window)
        {
            throw new System.NotImplementedException();
        }
        
    }

    public class CapsuleShapeGeometryTool : ShapeComponentGeometryTool
    {
        public CapsuleShapeGeometryTool(SFShapeComponent shapeComponent) : base(shapeComponent)
        {
            
        }

        public override void OnToolGUI(EditorWindow window)
        {
            throw new System.NotImplementedException();
        }
    }
}
