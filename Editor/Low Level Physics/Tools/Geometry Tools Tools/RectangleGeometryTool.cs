using SF.PhysicsLowLevel;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SFEditor.PhysicsLowLevel
{
    public class RectangleGeometryTool : ShapeComponentGeometryTool<SFRectangleShape>, IDrawSelectedHandles
    {
        public RectangleGeometryTool(SFRectangleShape shapeComponent) : base(shapeComponent) { }
        
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
            var handleSize = HandleUtility.GetHandleSize(shapeOrigin) * .25f;
            
            using (new Handles.DrawingScope(Matrix4x4.TRS(shapeOrigin,Quaternion.identity, Vector3.one)))
            {
                Handles.color = GrabHandleColor;

                // Grab the Size
                {
                    EditorGUI.BeginChangeCheck();
                    var size = handleRight * _shapeComponent.Size;
                    var newSize = Handles.Slider2D
                        (
                           Vector3.zero, 
                           handleDirection,
                           handleRight,
                           handleUp,
                           handleSize ,
                           Handles.RectangleHandleCap,
                           snap
                        );
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(_shapeComponent, "Change SF Rectangle Shape Size.");
                        
                        /* We times 2 because the handles technically only get from center to edge so only the half size is gotten from the handles.
                         * This is because we set the handle position to Vector3.Zero.
                         * Than we add the handle size because the starting handle size area is not taken into account for the Handle.Slider value change.
                         */
                        _shapeComponent.Size = Vector3.Max(new Vector3(.1f,.1f,.1f),newSize) * 2 + new Vector3(handleSize,handleSize, 0);
                        
                        localGeometry        = geometry.InverseTransform(
                                _shapeComponent.Shape.transform.rotation.GetMatrix(_transformPlane),
                                _shapeComponent.ScaleSize);
                        
                        _shapeComponent.SetShape(localGeometry);
                        _targetShapeChanged = true;
                    }
                }
                
                /*
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
                    Handles.Label((geometry.radius + handleSize * 2f) * handleRight, $"Radius = {labelGeometry.radius.ToString()}");
                }*/
                
                // Center.
                {
                    EditorGUI.BeginChangeCheck();
                    var newCenterValue = Handles.Slider2D(Vector3.zero, handleDirection, handleRight, handleUp, handleSize, Handles.CubeHandleCap, snap);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(_shapeComponent, "Change SF Rectangle Shape Center.");

                        //geometry.centroid += _shapeComponent.Body.rotation.InverseRotateVector(PhysicsMath.ToPosition2D(newCenterValue,_transformPlane));
                        localGeometry = geometry.InverseTransform(Matrix4x4.identity, _shapeComponent.ScaleSize);
                        _shapeComponent.PolygonGeometry = localGeometry;
                        _shapeComponent.Offset += _shapeComponent.Body.rotation.InverseRotateVector(PhysicsMath.ToPosition2D(newCenterValue,_transformPlane));
                        _targetShapeChanged = true;
                    }

                    // Draw center label.
                    Handles.color = GrabHandleColor;
                    var labelGeometry = localGeometry;
                    Handles.Label(handleUp * handleSize * 2f, $"Center = {labelGeometry.centroid.ToString(LabelFloatFormat)}");
                }
            }
            
            // Update the actual physic shape on the component.
            if(_targetShapeChanged)
                _shapeComponent.UpdateShape();
            
            _world.SetElementDepth3D(shapeOrigin);
            
            _world.DrawBox(PhysicsMath.ToPosition2D(shapeOrigin,_transformPlane), _shapeComponent.Size, geometry.radius, Color.green);
        }

        private static void DrawRectangleHandle()
        {
            // TODO: make a simple way to draw Rectangle Handles   
        }

        public override void OnDrawHandles()
        {
            
        }
    }
}
