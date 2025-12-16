using UnityEngine;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.U2D.Physics.LowLevelExtras;

namespace SF.PhysicsLowLevel
{
    [ExecuteAlways]
    [DefaultExecutionOrder(PhysicsLowLevelExtrasExecutionOrder.SceneBody)]
    [Icon("Packages/com.unity.2d.physics.lowlevelextras/Editor/Icons/SceneBody.png")]
    public class RectangleBody2D : SFShapeBody2D
    {
        [Header("ShapeProperties")] 
        public Vector2 Size;

        public PolygonGeometry ShapeGeometry;
        public bool ScaleRadius = true;
        
        protected override void CreateShape()
        {
            CreateBody();
            
            if (!Body.isValid)
                return;
            
            // Calculate the relative transform from the scene body to this scene shape.
            var relativeTransform = PhysicsMath.GetRelativeMatrix(transform, transform, Body.world.transformPlane);
            
            var geometry = ShapeGeometry.Transform(relativeTransform, ScaleRadius);
            if (!geometry.isValid)
                return;

            Shape = Body.CreateShape(geometry, ShapeDefinition);
            
            if (!Shape.isValid)
                return;
            
            // Set the owner.
            _ownerKey = Shape.SetOwner(this);
            _ownedShapes.Add(new OwnedShapes(ref Shape, _ownerKey));
        }

        protected override void DestroyShape()
        {
            if (Shape.isValid)
                    Shape.Destroy(updateBodyMass: false, ownerKey: _ownerKey);
            
            if (Body.isValid)
                Body.ApplyMassFromShapes();
            
            if (!_ownedShapes.IsCreated)
                return;
            

            foreach (var ownedShape in _ownedShapes)
            {
                if (ownedShape.Shape.isValid)
                    ownedShape.Shape.Destroy(updateBodyMass: false, ownerKey: ownedShape.OwnerKey);
            }
            
            _ownedShapes.Clear();
            
            
        }
    }
}
