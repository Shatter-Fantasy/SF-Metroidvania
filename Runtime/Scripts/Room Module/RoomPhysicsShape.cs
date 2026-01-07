using SF.PhysicsLowLevel;
using Unity.Burst;
using Unity.Cinemachine;
using Unity.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.RoomModule
{
    [AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Physic2D Shape")]
    [SaveDuringPlay]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [BurstCompile]
    public class RoomPhysicsShape : CinemachineExtension
    {
        public SFShapeComponent ConfinerShape;

        public PhysicsShape Shape;

        [Header("Shape Properties")]
        public Vector3 PreviousCameraPosition;
        
        /// <summary>Damping applied automatically around corners to avoid jumps.</summary>
        [Tooltip("Damping applied around corners to avoid jumps.  Higher numbers are more gradual.")]
        [Range(0, 5)]
        [SerializeField] private float _damping;
        
        ///// <summary>Size of the slow-down zone at the edge of the bounding shape.</summary>
        //[Tooltip("Size of the slow-down zone at the edge of the bounding shape.")]
        //[SerializeField] private float _slowingDistance = 0.5f;
        
        // Shortest way is to use clipper just like the original CinemachineConfiner2D,
        // but honestly think it would be better to use the low level physics math.
        // See the Unity class called ConfinerOven. This is what bakes the Confiner for CinemachineConfiner2D.
        
        /// <summary>
        /// Updates the position of the virtual <see cref="CameraState"/> and confines the <see cref="CinemachineCamera"/>
        /// into the defined <see cref="UnityEngine.LowLevelPhysics2D.PhysicsShape"/>
        /// </summary>
        /// <param name="vcam"></param>
        /// <param name="stage"></param>
        /// <param name="state"></param>
        /// <param name="deltaTime"></param>
        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            // We only update during the body (stage where position is initially checked before rotation) stage calculation stage.
            if (stage != CinemachineCore.Stage.Body)
                return;

            // ConfinerShape is of type SFShapeComponent, my custom base class for PhysicShape/PhysicBody that act like Components for GameObjects.
            // Low Level Physics2D equivalent to Collider2D.
            if(ConfinerShape == null)
               return;
            
            
            Shape = ConfinerShape.Shape;
            
            if (!Shape.isValid)
                return;

            var camPosition = state.GetCorrectedPosition();
            
            PhysicsTransform frustumTransform = new PhysicsTransform(camPosition);
            
            if (!Shape.isValid)
                return;

            /* Camera.orthographicSize => Camera's half-size when in orthographic mode.
             * The orthographicSize property defines the viewing volume of an orthographic Camera.
             * To edit orthographicSize, you must set the Camera projection to orthographic.
             * The height of the viewing volume is (orthographicSize * 2).
             * Unity calculates the width of the viewing volume using orthographicSize and the camera's aspect.
             */
            var settings = state.Lens;
            float frustumHalfHeight = CalculateFrustumHalfHeight(settings.OrthographicSize,camPosition.z,settings.FieldOfView);
            float frustumHalfWidth = frustumHalfHeight * settings.Aspect;
            
            Vector2 frustum = new Vector2(frustumHalfWidth * 2, frustumHalfHeight * 2);
            // Create a shape using the camera frustum for the geometry outline and physical transform.
            PolygonGeometry  frustumGeometry  = PolygonGeometry.CreateBox(frustum);
            
            // If the shape is contained we don't need to do any bounding checks or calculations.
            if(Shape.aabb.Contains(frustumGeometry.CalculateAABB(frustumTransform)))
                return;

            // We only confine for the x and y axis for 2D games.
            Vector2 cameraPoint2D  = new(camPosition.x, camPosition.y);

            cameraPoint2D = frustumGeometry.ClosestPoint(cameraPoint2D);
            
            // TODO: Check if the point is on the left or right side of the camera.
            
            var frustumSize = new Vector2(frustumHalfWidth, frustumHalfHeight);
            Shape.world.DrawPoint(cameraPoint2D,7.5f,Color.green);
            Shape.world.DrawPoint(cameraPoint2D - frustumSize,7.5f,Color.brown);
           
            
            // Grab the original fixed Z value
            var targetPosition = new Vector3(cameraPoint2D.x, cameraPoint2D.y, camPosition.z);
            
            var deltaPosition  = camPosition - targetPosition;
            
            // TODO: Damping via exponential smoothing 
            // We need to do damping to prevent the camera from ever even going past the boundary.
            // When pass the boundary that is when math breaks down.
            
            Debug.Log($"Camera Raw Position:{camPosition}, Target Position: {targetPosition}, deltaPosition: {camPosition - targetPosition}");
            if (Mathf.Abs(deltaPosition.y) < 1f)
                frustumSize.y = 0;
            if (Mathf.Abs(deltaPosition.x) < 1f)
                frustumSize.x = 0;
            state.PositionCorrection += (deltaPosition + ((Vector3)frustumSize * 2));
        }
        
        private static void ConfineFrustum(PhysicsShape shape, PhysicsTransform frustumTransform, in LensSettings settings, in Vector3 cameraOriginPos, out Vector3 newCameraPosition)
        {
            newCameraPosition = cameraOriginPos;
            
            if (!shape.isValid)
                return;

            /* Camera.orthographicSize => Camera's half-size when in orthographic mode.
             * The orthographicSize property defines the viewing volume of an orthographic Camera.
             * To edit orthographicSize, you must set the Camera projection to orthographic.
             * The height of the viewing volume is (orthographicSize * 2).
             * Unity calculates the width of the viewing volume using orthographicSize and the camera's aspect.
             */
            
            float frustumHalfHeight = CalculateFrustumHalfHeight(settings.OrthographicSize,cameraOriginPos.z,settings.FieldOfView);
            float frustumHalfWidth = frustumHalfHeight * settings.Aspect;
            
            Vector2 frustum = new Vector2(frustumHalfWidth * 2, frustumHalfHeight * 2);
            // Create a shape using the camera frustum for the geometry outline and physical transform.
            PolygonGeometry  frustumGeometry  = PolygonGeometry.CreateBox(frustum);
            
            // If the shape is contained we don't need to do any bounding checks or calculations.
            if(shape.aabb.Contains(frustumGeometry.CalculateAABB(frustumTransform)))
               return; 
            
            var                               vertices          = shape.polygonGeometry.AsReadOnlySpan();
            using NativeList<SegmentGeometry> segmentGeometries = new (Allocator.Temp);

            for (int i = 0; i < 4; i++)
            {
                if(i + 1 < 4)
                    segmentGeometries.Add(SegmentGeometry.Create(vertices[i], vertices[i + 1]));
                else
                    segmentGeometries.Add(SegmentGeometry.Create(vertices[i], vertices[0]));

                segmentGeometries[i].Transform(frustumTransform);
            }
            
            NativeList<PhysicsShape.ContactManifold> manifolds = new (Allocator.Temp);
            for (int i = 0; i < segmentGeometries.Length; i++)
            {
                manifolds.Add(frustumGeometry.Intersect(frustumTransform, segmentGeometries[i], shape.transform));
            }
            Color manifoldColor = Color.mediumPurple;
            DrawManifold(ref shape, ref manifolds, ref manifoldColor);
            
            // For each manifold with a hit check the position we need to restrain the camera frustum on.
            for (int i = 0; i < manifolds.Length; i++)
            {
                if(manifolds[i].pointCount < 1)
                    continue;
                
                var manifold = manifolds[i];
                var midPoint = GetCenterManifoldPoint(ref manifold);
                // left Side
                if (midPoint.x > cameraOriginPos.x - frustumHalfWidth)
                {
                    newCameraPosition.x = midPoint.x;
                    // Closest 
                    shape.world.DrawPoint(midPoint, 5.5f, Color.blue);
                }
            }

            manifolds.Dispose();
        }
        
        private static Vector2 GetCenterManifoldPoint(ref PhysicsShape.ContactManifold manifold)
        {
            return new Vector2
                ((manifold.points.contactInfo1.point.x + manifold.points.contactInfo0.point.x) / 2,
                    (manifold.points.contactInfo1.point.y + manifold.points.contactInfo0.point.y) / 2);
        }

        [BurstCompile]
        private static void DrawManifold(ref PhysicsShape shape, ref NativeList<PhysicsShape.ContactManifold> manifolds, ref Color color, float radius = 15f)
        {
            for (int i = 0; i < manifolds.Length; i++)
            {
                if(manifolds[i].pointCount < 1)
                    continue;
                
                var manifold = manifolds[i];
                shape.world.DrawPoint(manifolds[i].points.contactInfo0.point,radius, Color.mediumPurple);
                shape.world.DrawPoint(manifolds[i].points.contactInfo1.point,radius, Color.mediumPurple);
                shape.world.DrawPoint(GetCenterManifoldPoint( ref manifold), radius, Color.red);
            }
        }
        
        /// <summary>
        /// Calculates half frustum height for orthographic or perspective camera.
        /// </summary>
        /// <param name="orthographicSize">Camera Lens Orthographic Size</param>
        /// <param name="cameraPosLocalZ"> Camera's z pos in local space</param>
        /// <param name="fieldOfView">Camera lens field of view</param>
        /// <param name="isOrthographic">Is camera lens orthographic</param>
        /// <returns>Frustum height of the camera</returns>
        /// <remarks>
        /// This method assumes the passed in Z has already been transformed from the 
        /// </remarks>
        [BurstCompile]
        public static float CalculateFrustumHalfHeight(
            in float orthographicSize, 
            in float cameraPosLocalZ,
            in float fieldOfView,
            bool isOrthographic = true)
        {
            float frustumHeight;
            
            // in Orthographic mode the half height of the frustum is literally just the lens.OrthographicSize
            if (isOrthographic) 
                frustumHeight = orthographicSize;
            else
            {
                // distance between the collider's plane and the camera
                float distance = cameraPosLocalZ;
                frustumHeight = distance * Mathf.Tan(fieldOfView * 0.5f * Mathf.Deg2Rad);
            }

            return Mathf.Abs(frustumHeight);
        }
    }
}
