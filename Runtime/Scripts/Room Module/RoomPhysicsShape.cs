using Unity.Burst;
using Unity.Cinemachine;
using Unity.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.RoomModule
{
    using PhysicsLowLevel;
    
    [AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Physic2D Shape")]
    [SaveDuringPlay]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [BurstCompile]
    public class RoomPhysicsShape : MonoBehaviour
    {
        [Header("Shape Properties")]
        public SFShapeComponent ConfinerShapeComponent;
        private PhysicsShape _confinerShape;
        [Header("Camera Properties")] 
        public CinemachineVirtualCameraBase VirtualCamera;
        
        private void Awake()
        {
            // No camera so let's not waste CPU by making shapes.
            if (VirtualCamera == null)
                TryGetComponent(out VirtualCamera);
        }
        
        protected void CreateShape()
        {
            // ConfinerShape is of type SFShapeComponent, my custom base class for PhysicShape/PhysicBody that act like Components for GameObjects.
            // Low Level Physics2D equivalent to Collider2D.
            if(ConfinerShapeComponent == null)
                return;
            
                       
            // Create the Chain Geometry for the room outline
        
            // Room Chain Outline.
            {
                var groundBody = _confinerShape.world.CreateBody();
                var aabb       = _confinerShape.aabb;
                using var verticesArray = new NativeList<Vector2>(Allocator.Temp)
                {
                    aabb.upperBound,                                   // Upper Right
                    new Vector2(aabb.upperBound.x, aabb.lowerBound.y), // Lower Right
                    aabb.lowerBound,                                   // Lower Left
                    new Vector2(aabb.lowerBound.x, aabb.upperBound.y), // Upper Left
                };

                groundBody.CreateChain(
                    new ChainGeometry(verticesArray.AsArray()),
                    PhysicsChainDefinition.defaultDefinition);
            }
            
            // Create the frustum camera shape.
            var state = VirtualCamera.State;
            var camPosition = VirtualCamera.transform.position;
            var settings = state.Lens;
            float frustumHalfHeight = CalculateFrustumHalfHeight(settings.OrthographicSize,camPosition.z,settings.FieldOfView);
            float frustumHalfWidth = frustumHalfHeight * settings.Aspect;
            Vector2          frustum          = new Vector2(frustumHalfWidth * 2, frustumHalfHeight * 2);
            
            // Create a shape using the camera frustum for the geometry outline and physical transform.
            PolygonGeometry  frustumGeometry  = PolygonGeometry.CreateBox(frustum);
            
            //var frustumBody  = _confinerShape.world.CreateBody(BodyDefinition);
            //frustumBody.CreateShape(frustumGeometry);
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
