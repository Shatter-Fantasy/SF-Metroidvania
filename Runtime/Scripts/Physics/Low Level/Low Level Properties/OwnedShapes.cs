using UnityEngine.LowLevelPhysics2D;

namespace SF.PhysicsLowLevel
{
    /// <summary>
    /// Used to keep track of physic shapes and their owner ids used in a single <see cref="SFShapeComponent"/>
    /// or other SF based low level physics shape classes.
    /// </summary>
    /// <remarks>
    /// This is recommended to be used with <see cref="Unity.Collections.NativeList[OwnedShapes]"/>
    /// instead of <see cref="System.Collections.Generic.List[OwnedShapes]"/>.
    /// </remarks>
    [System.Serializable]
    public readonly struct OwnedShapes
    {
        public readonly PhysicsShape Shape;
        public readonly int OwnerKey;

        public OwnedShapes(PhysicsShape shape, UnityEngine.Object owner)
        {
            if (shape.isValid && owner != null)
            {
                Shape    = shape;
                OwnerKey = Shape.SetOwner(owner);
            }
            else
            {
                Shape    = new PhysicsShape();
                OwnerKey = -1;
            }
        }
        
        public OwnedShapes(PhysicsShape shape, int ownerKey)
        {
            if (shape.isValid)
            {
                Shape    = shape;
                OwnerKey = ownerKey;
            }
            else
            {
                Shape    = new PhysicsShape();
                OwnerKey = -1;
            }
        }
        
        /// <summary>
        /// Checks if the <see cref="Shape"/> is valid and if the <see cref="OwnerKey"/>
        /// doesn't equal -1 which indicates no owner was set on the shape. 
        /// </summary>
        /// <param name="ownedShapes"></param>
        /// <returns></returns>
        public static bool IsValid(OwnedShapes ownedShapes)
        {
            return ownedShapes.Shape.isValid
                   && ownedShapes.OwnerKey != -1;
        }
    }
}
