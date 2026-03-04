using Unity.Burst;
using Unity.Collections;
using UnityEngine.LowLevelPhysics2D;

namespace SF.PhysicsLowLevel
{
    [BurstCompile]
    public static class PhysicsQueriesExtensions
    {
        
#region World Queries
        public static NativeArray<PhysicsQuery.WorldOverlapResult> OverlapOnWorld(
            this PhysicsShape shape,
            PhysicsWorld world,
            PhysicsQuery.QueryFilter filter,
            Allocator allocator = Allocator.Temp)
        {
            PhysicsShape.ShapeType                       shapeType   = shape.shapeType;
            NativeArray<PhysicsQuery.WorldOverlapResult> nativeArray = new  NativeArray<PhysicsQuery.WorldOverlapResult>();
            switch (shapeType)
            {
                case PhysicsShape.ShapeType.Circle:
                    nativeArray = world.OverlapGeometry(shape.circleGeometry.Transform(shape.body.transform), filter, allocator);
                    break;
                case PhysicsShape.ShapeType.Capsule:
                    nativeArray = world.OverlapGeometry(shape.capsuleGeometry.Transform(shape.body.transform), filter, allocator);
                    break;
                case PhysicsShape.ShapeType.Segment:
                    nativeArray = world.OverlapGeometry(shape.segmentGeometry.Transform(shape.body.transform), filter, allocator);
                    break;
                case PhysicsShape.ShapeType.Polygon:
                    nativeArray = world.OverlapGeometry(shape.polygonGeometry.Transform(shape.body.transform), filter, allocator);
                    break;
                case PhysicsShape.ShapeType.ChainSegment:
                    nativeArray = world.OverlapGeometry(shape.chainSegmentGeometry.segment.Transform(shape.body.transform), filter, allocator);
                    break;
            }
            return nativeArray;
        }
#endregion

#region PhysicsQuery.QueryFilter
        
        public static PhysicsQuery.QueryFilter GetQueryFilter(this in PhysicsShape.ContactFilter contactFilter) 
            => new PhysicsQuery.QueryFilter(contactFilter.categories, contactFilter.contacts);
        
        public static PhysicsQuery.QueryFilter GetQueryFilter(this in PhysicsShape shape) 
            => new PhysicsQuery.QueryFilter(shape.contactFilter.categories, shape.contactFilter.contacts);
#endregion
    }
}
