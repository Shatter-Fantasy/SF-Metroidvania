using SF.PhysicsLowLevel;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SFEditor.PhysicsLowLevel
{
    public class CircleShapeGeometryTool : ShapeComponentGeometryTool<SFCircleShape>
    {
        public CircleShapeGeometryTool(SFCircleShape shapeComponent) : base(shapeComponent)
        {
            
        }

        public override void OnToolGUI(EditorWindow window)
        {
            // world space geometry.
            var geometry = _shape.circleGeometry;
            // localGeometry = the geometry in local space
            var localGeometry = _shapeComponent.Shape.circleGeometry;
            
            // Set-up handles for allowing in scene editing of the shapes properties.
            var snap            = EditorSnapSettings.move;
            var handleDirection = PhysicsMath.GetTranslationIgnoredAxes(_transformPlane);
            var handleRight     = _body.rotation.GetMatrix(_transformPlane).MultiplyVector(PhysicsMath.Swizzle(Vector3.right, _transformPlane)).normalized;
            var handleUp     = _body.rotation.GetMatrix(_transformPlane).MultiplyVector(PhysicsMath.Swizzle(Vector3.up, _transformPlane)).normalized;
            
            // Set up any labels - use Handles.DrawingScope with the shape's transform to ToPosition3D for better label positioning.
            // Update the shape if handles change it
            var shapeOrigin = PhysicsMath.ToPosition3D
                (
                    _body.transform.TransformPoint(geometry.center),
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
                        Undo.RecordObject(_shapeComponent, "Change SF Circle Shape Radius.");
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
                
                
                // Center.
                {
                    EditorGUI.BeginChangeCheck();
                    var newCenterValue = Handles.Slider2D(Vector3.zero, handleDirection, handleRight, handleUp, handleSize, Handles.CubeHandleCap, snap);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(_shapeComponent, "Change SF Circle Shape Center.");

                        geometry.center += _shapeComponent.Body.rotation.InverseRotateVector(PhysicsMath.ToPosition2D(newCenterValue,_transformPlane));
                        localGeometry = geometry.InverseTransform(Matrix4x4.identity, _shapeComponent.ScaleSize);
                        _shapeComponent.CircleGeometry = localGeometry;
                        _targetShapeChanged = true;
                    }

                    // Draw center label.
                    Handles.color = GrabHandleColor;
                    var labelGeometry = localGeometry;
                    Handles.Label(handleUp * handleSize * 2f, $"Center = {labelGeometry.center.ToString(LabelFloatFormat)}");
                }
            }
            
            // Update the actual physic shape on the component.
            if(_targetShapeChanged)
                _shapeComponent.UpdateShape();
            
            _world.SetElementDepth3D(shapeOrigin);
            _world.DrawCircle(PhysicsMath.ToPosition2D(shapeOrigin,_transformPlane), geometry.radius, Color.green);
        }

        public override void OnDrawHandles()
        {
            
        }
    }
}
