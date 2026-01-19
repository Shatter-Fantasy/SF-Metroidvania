using SF.PhysicsLowLevel;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SFEditor.PhysicsLowLevel
{
    
    public abstract class ShapeComponentGeometryTool : IGeometryToolSettings, IDrawSelectedHandles
    {
        public Color GrabHandleColor { get; set; }
        
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
        
        public abstract bool UpdateTool();

        public abstract bool IsValid { get; }
        public abstract void OnDrawHandles();
    }

    /// <summary>
    /// Inherit from <see cref="ShapeComponentGeometryTool"/> to create a new tool for editing
    /// a component that inherits from <see cref="SFShapeComponent"/>
    /// </summary>
    public abstract class ShapeComponentGeometryTool<T> : ShapeComponentGeometryTool where T : SFShapeComponent
    {
        /// <summary>
        /// The local cached <see cref="PhysicsShape"/> that is being drawn by <see cref="PhysicsWorld"/> draw methods.
        /// </summary>
        protected PhysicsShape _shape;
        protected PhysicsBody _body;
        protected readonly T _shapeComponent;
        protected PhysicsWorld _world;
        protected PhysicsWorld.TransformPlane _transformPlane;

        /// <summary>
        /// Has the target shaped been changed since the last time the tool was updated.
        /// </summary>
        protected bool _targetShapeChanged;
        
        protected ShapeComponentGeometryTool(T shapeComponent)
        {
            GrabHandleColor = Color.whiteSmoke;
            
            _shapeComponent = shapeComponent;

            if (_shapeComponent == null)
                return;
            
            UpdateTool();
        }
        
        public override bool IsValid => _shapeComponent != null && _shape.isValid && _shapeComponent.isActiveAndEnabled;

        public override bool UpdateTool()
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
        public Color LabelColor { get; set; } = Color.white;
        
        protected static string LabelFloatFormat => "F4";
#endregion

    }
    
    public class PolygonShapeGeometryTool : ShapeComponentGeometryTool<SFShapeComponent>
    {
        public PolygonShapeGeometryTool(SFShapeComponent shapeComponent) : base(shapeComponent)
        {
            
        }

        public override void OnToolGUI(EditorWindow window)
        {
           // world space geometry.
            var geometry = _shape.polygonGeometry;
            // localGeometry = the geometry in local space
            var localGeometry = _shapeComponent.Shape.polygonGeometry;
            
            // Set-up handles for allowing in scene editing of the shapes properties.
            var snap            = EditorSnapSettings.move;
            var handleDirection = PhysicsMath.GetTranslationIgnoredAxes(_transformPlane);
            var handleRight     = _body.rotation.GetMatrix(_transformPlane).MultiplyVector(PhysicsMath.Swizzle(Vector3.right, _transformPlane)).normalized;
            var handleUp     = _body.rotation.GetMatrix(_transformPlane).MultiplyVector(PhysicsMath.Swizzle(Vector3.up, _transformPlane)).normalized;
            
            // Set up any labels - use Handles.DrawingScope with the shape's transform to ToPosition3D for better label positioning.
            // Update the shape if handles change it
            var shapeOrigin = PhysicsMath.ToPosition3D
                (
                    _body.transform.TransformPoint(geometry.centroid),
                    _shapeComponent.transform.position, 
                    _transformPlane
                );
            var handleSize = HandleUtility.GetHandleSize(shapeOrigin) * 0.1f;;
            
            using (new Handles.DrawingScope(Matrix4x4.TRS(shapeOrigin,Quaternion.identity, Vector3.one)))
            {
                Handles.color = GrabHandleColor;

                // Grab the radius
                {
                    EditorGUI.BeginChangeCheck();
                    var radius    = handleRight * geometry.radius;
                    var newRadius = Handles.Slider2D
                        (
                            radius,
                            handleDirection, 
                            handleRight, 
                            handleUp, 
                            handleSize, 
                            Handles.SphereHandleCap,
                            snap
                        );
                    
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(_shapeComponent, "Change SF Rectangle Shape Radius.");
                        geometry.radius = newRadius.magnitude;
                        localGeometry = geometry.InverseTransform(Matrix4x4.identity, _shapeComponent.ScaleSize);
                        _shapeComponent.SetShape(localGeometry);
                        _targetShapeChanged = true;
                    }
                    
                    // Draw the radius label.
                    Handles.color = LabelColor;
                    // Make it where the user can switch between local and world space for tool labels positioning.
                    // var labelGeometry = are we in local space ? localGeometry : geometry
                    var labelGeometry = localGeometry;
                    Handles.Label((geometry.radius + handleSize) * handleRight, $"Radius = {labelGeometry.radius.ToString()}");
                }
            }
            
            // Update the actual physic shape on the component.
            if(_targetShapeChanged)
                _shapeComponent.UpdateShape();
            
            _world.SetElementDepth3D(shapeOrigin);
            
            _world.DrawBox(PhysicsMath.ToPosition2D(shapeOrigin,_transformPlane),Vector2.one, geometry.radius, Color.green);
        }

        public override void OnDrawHandles()
        {
            throw new System.NotImplementedException();
        }
    }

    public class CapsuleShapeGeometryTool : ShapeComponentGeometryTool<SFCapsuleShape>
    {
        public CapsuleShapeGeometryTool(SFCapsuleShape shapeComponent) : base(shapeComponent)
        {
            
        }

        public override void OnToolGUI(EditorWindow window)
        {
            throw new System.NotImplementedException();
        }

        public override void OnDrawHandles()
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class SFCapsuleShape : SFShapeComponent {}
}
