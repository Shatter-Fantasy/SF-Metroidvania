using UnityEngine;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.U2D.Physics.LowLevelExtras;

namespace SF.PhysicsLowLevel
{
    [ExecuteAlways]
    [DefaultExecutionOrder(PhysicsLowLevelExtrasExecutionOrder.SceneShape)]
    [AddComponentMenu("Physics 2D/LowLevel/SF Circle Shape", 23)]
    [Icon("Packages/com.unity.2d.physics.lowlevelextras/Editor/Icons/SceneShape.png")]
    public class SFCircleComponent : SFShapeComponent
    {
        /// <summary>
        /// The radius of the circle.
        /// </summary>
        public float Radius = .5f;

        public Vector2 CenterPoint;

        public static readonly float MinAllowedSize = 0.0000001f;
        
        protected override void CreateBodyShapeGeometry()
        {
            if (MinAllowedSize > Radius)
                Radius = MinAllowedSize;
            
            Shape = Body.CreateShape(CircleGeometry.Create(Radius,CenterPoint), ShapeDefinition);
        }

        public override void SetShape<TGeometryType>(TGeometryType geometryType)
        {  
            if (!_shape.isValid)
                return;

            if (geometryType is CircleGeometry circleGeometry)
                Radius = circleGeometry.radius;
        }
    }
}
