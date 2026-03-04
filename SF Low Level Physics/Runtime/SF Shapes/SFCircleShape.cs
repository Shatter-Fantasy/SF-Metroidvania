using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.PhysicsLowLevel
{
    [ExecuteAlways]
    [AddComponentMenu("Physics 2D/LowLevel/SF Circle Shape", 23)]
    [Icon("Packages/shatterfantasy.sf-metroidvania/Editor/Icons/SceneBody.png")]
    public class SFCircleShape : SFShapeComponent
    {
        /// <summary>
        /// The geometry properties that are used to create the <see cref="SFShapeComponent.Shape"/> for the SFCircleComponent.
        /// </summary>
        public CircleGeometry CircleGeometry = new CircleGeometry()
        {
            radius = 0.5f,
        };

        public static readonly float MinAllowedSize = 0.00001f;
        
        protected override void CreateBodyShapeGeometry()
        {
            if (MinAllowedSize > CircleGeometry.radius)
                CircleGeometry.radius = MinAllowedSize;
            
            _shape = Body.CreateShape(CircleGeometry, ShapeDefinition);
        }

        public override void SetShape<TGeometryType>(TGeometryType geometryType)
        {  
            if (!_shape.isValid)
                return;

            if (geometryType is CircleGeometry circleGeometry)
                CircleGeometry = circleGeometry;
        }
    }
}
