using SF.PhysicsLowLevel;
using Unity.Burst;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.RoomModule
{
    [AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Physic2D Shape")]
    [SaveDuringPlay]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class RoomPhysicsShape : CinemachineExtension
    {
        public SFShapeComponent ConfinerShape;

        public PhysicsShape Shape;

        [Header("Shape Properties")]
        public Vector3 PreviousCameraPosition;
        
        [SerializeField] private bool _isConfined;
        
        /// <summary>Damping applied automatically around corners to avoid jumps.</summary>
        [Tooltip("Damping applied around corners to avoid jumps.  Higher numbers are more gradual.")]
        [Range(0, 5)]
        [SerializeField] private float _damping;
        
        /// <summary>Size of the slow-down zone at the edge of the bounding shape.</summary>
        [Tooltip("Size of the slow-down zone at the edge of the bounding shape.")]
        [SerializeField] private float _slowingDistance = 0.5f;
        
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

            if(ConfinerShape == null)
               return;
            
            Shape = ConfinerShape.Shape;
            
            if (!Shape.isValid)
                return;
            
            _isConfined = IsConfined(Shape,state.Lens,vcam.transform.position);

            var camPos  = state.GetCorrectedPosition();
            var prevPos = PreviousCameraPosition;
            
            GetDistanceToEdge(Shape,state.Lens,vcam.transform.position);
            
            if (_slowingDistance > Epsilon && deltaTime >= 0 && vcam.PreviousStateIsValid)
            {
                // Only reduce the speed when moving toward the edge and close to it.
                
            }
            /* Step One: Check which direction the camera is going over.
             * Step Two: Limit the camera delta along the axis that is not outside the contained shape 
             */
            

            PreviousCameraPosition = state.GetCorrectedPosition();
        }

        private static float GetDistanceToEdge(in PhysicsShape shape, in LensSettings settings, in Vector3 cameraOriginPos)
        {
            if (!shape.isValid)
                return 0;
            
            /* Camera.orthographicSize => Camera's half-size when in orthographic mode.
             * The orthographicSize property defines the viewing volume of an orthographic Camera.
             * To edit orthographicSize, you must set the Camera projection to orthographic.
             * The height of the viewing volume is (orthographicSize * 2).
             * Unity calculates the width of the viewing volume using orthographicSize and the camera's aspect.
             */
            
            float frustumHalfHeight = CalculateFrustumHalfHeight(settings, cameraOriginPos.z);
            float frustumHalfWidth  = frustumHalfHeight * settings.Aspect;

            Vector2       frustum       = new Vector2(frustumHalfWidth * 2, frustumHalfHeight * 2);
            // Create a shape using the camera frustum for the geometry outline and physical transform.
            PolygonGeometry  frustumGeometry  = PolygonGeometry.CreateBox(frustum);
            PhysicsTransform frustumTransform = new PhysicsTransform(cameraOriginPos);
            PhysicsQuery.DistanceInput distanceInput = new PhysicsQuery.DistanceInput()
            {
                shapeProxyA = shape.CreateShapeProxy(),
                shapeProxyB = new PhysicsShape.ShapeProxy(frustumGeometry),
                transformA  = shape.transform, 
                transformB  = frustumTransform,
                useRadii    = false
            };
            
            var distanceResult = PhysicsQuery.ShapeDistance(distanceInput);
            float distance       = distanceResult.distance;
            
            // Camera frustum is confined in the confiner shape.
            if (distance == 0)
            {
             
            }
            shape.world.DrawPoint(distanceResult.pointA,5f,Color.red);
            shape.world.DrawPoint(distanceResult.pointB,5f,Color.red);
            return distanceResult.distance;
        }

        [BurstCompile]
        private static bool IsConfined(in PhysicsShape shape, in LensSettings settings, in Vector3 cameraOriginPos)
        {
            if (!shape.isValid)
                return false;
            
            /* Camera.orthographicSize => Camera's half-size when in orthographic mode.
             * The orthographicSize property defines the viewing volume of an orthographic Camera.
             * To edit orthographicSize, you must set the Camera projection to orthographic.
             * The height of the viewing volume is (orthographicSize * 2). 
             * Unity calculates the width of the viewing volume using orthographicSize and the camera's aspect.
             */
            
            float frustumHalfHeight = CalculateFrustumHalfHeight(settings, cameraOriginPos.z);
            float frustumHalfWidth = frustumHalfHeight * settings.Aspect;

            Vector2 frustum = new Vector2(frustumHalfWidth * 2, frustumHalfHeight * 2);

            // Create a shape using the camera frustum for the geometry outline and physical transform.
            PolygonGeometry  frustumGeometry  = PolygonGeometry.CreateBox(frustum);
            PhysicsTransform frustumTransform = new PhysicsTransform(cameraOriginPos); 
            
            return shape.aabb.Contains(frustumGeometry.CalculateAABB(frustumTransform));
        }
        
        /// <summary>
        /// Calculates half frustum height for orthographic or perspective camera.
        /// </summary>
        /// <param name="lens">Camera Lens for checking if Orthographic or Perspective</param>
        /// <param name="cameraPosLocalZ">camera's z pos in local space</param>
        /// <returns>Frustum height of the camera</returns>
        /// <remarks>
        /// This method assumes the passed in Z has already been transformed from the 
        /// </remarks>
        [BurstCompile]
        public static float CalculateFrustumHalfHeight(in LensSettings lens, in float cameraPosLocalZ)
        {
            float frustumHeight;
            
            // in Orthographic mode the half height of the frustum is literally just the lens.OrthographicSize
            if (lens.Orthographic) 
                frustumHeight = lens.OrthographicSize;
            else
            {
                // distance between the collider's plane and the camera
                float distance = cameraPosLocalZ;
                frustumHeight = distance * Mathf.Tan(lens.FieldOfView * 0.5f * Mathf.Deg2Rad);
            }

            return Mathf.Abs(frustumHeight);
        }
    }
}
