using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.PhysicsLowLevel
{
    [ExecuteAlways]
    [AddComponentMenu("Physics 2D/LowLevel/SF Capsule Shape", 23)]
    [Icon("Packages/com.unity.2d.physics.lowlevelextras/Editor/Icons/SceneShape.png")]
    public class SFCapsuleShape : SFShapeComponent
    {
        /// <summary>
        /// The radius of the circle.
        /// </summary>
        public float Radius = .5f;

        /// <summary>
        /// The starting point of the <see cref="CapsuleGeometry"/> which represents
        /// the <see cref="CapsuleGeometry.center1"/>
        /// </summary>
        public Vector2 StartingPoint = new Vector2(0,.5f);
        
        /// <summary>
        /// The ending point of the <see cref="CapsuleGeometry"/> which represents
        /// the <see cref="CapsuleGeometry.center2"/>
        /// </summary>
        public Vector2 EndingPoint= new Vector2(0,-.5f);

        public static readonly float MinAllowedSize = 0.0000001f;
        
        protected override void CreateBodyShapeGeometry()
        {
            if (MinAllowedSize > Radius)
                Radius = MinAllowedSize;
            
            _shape = Body.CreateShape(CapsuleGeometry.Create(StartingPoint,EndingPoint,Radius), ShapeDefinition);
        }
    }
}
