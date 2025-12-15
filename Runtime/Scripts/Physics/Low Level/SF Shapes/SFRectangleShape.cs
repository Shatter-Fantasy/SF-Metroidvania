using UnityEngine;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.U2D.Physics.LowLevelExtras;

namespace SF.PhysicsLowLevel
{
    [ExecuteAlways]
    [DefaultExecutionOrder(PhysicsLowLevelExtrasExecutionOrder.SceneShape)]
    [AddComponentMenu("Physics 2D/LowLevel/SF Rectangle Shape", 22)]
    [Icon("Packages/com.unity.2d.physics.lowlevelextras/Editor/Icons/SceneShape.png")]
    public class SFRectangleShape : SFShapeComponent
    {
        [Header("Rectangle Properties")]
        public Vector2 Size = Vector2.one; 
        public float CornerRadius;

        public static readonly Vector2 MinAllowedSize = new Vector2(0.00000005f,0.00000005f);
        
        protected override void CreateShapeGeometry()
        {
            if (MinAllowedSize.x > Size.x)
            {
                Size.x = MinAllowedSize.x;
#if UNITY_EDITOR
                Debug.LogWarning($"In the {nameof(SFRectangleShape)} component on gameobject: {gameObject.name}, the value for the Size.x was below the allowed min value of: {MinAllowedSize.x}", this);
#endif
            }
            if (MinAllowedSize.y > Size.y)
            {
                Size.y = MinAllowedSize.y;
#if UNITY_EDITOR
                Debug.LogWarning($"In the {nameof(SFRectangleShape)} component on gameobject: {gameObject.name}, the value for the Size.y was below the allowed min value of: {MinAllowedSize.y}", this);
#endif
            }
            Shape = Body.CreateShape(PolygonGeometry.CreateBox(Size), ShapeDefinition);
        }
    }
}

