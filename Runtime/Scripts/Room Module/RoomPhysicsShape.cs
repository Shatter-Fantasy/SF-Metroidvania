using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.U2D.Physics.LowLevelExtras;

namespace SF.RoomModule
{
    [AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Physic2D Shape")]
    [SaveDuringPlay]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class RoomPhysicsShape : CinemachineExtension
    {
        public SceneShape ConfinerShape;

        public PhysicsShape Shape;

        [Header("Shape Properties")]
        public Vector3 PreviousCameraPosition;
        
        [SerializeField] private bool _isConfined;
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
            
            
            _isConfined = IsConfined(state.Lens,vcam.transform.position);

            if (!_isConfined)
                state.PositionCorrection = PreviousCameraPosition - state.GetCorrectedPosition();
            
            PreviousCameraPosition = state.GetCorrectedPosition();


            // Step 1: Get current camera state and properties
            // 1. aspect ratio
            // 2. correct camera position => state.GetCorrectedPosition()

            /* Step 2 Make sure the current frustum size has a possible solution for the calculation.
             *  Note: deltaWorldToBaked.lossyScale is always uniform.
             *  We might be able to use some of the new low level PhysicsMath utilities to make this easier or a PhysicsShape.isvalid.
             *
             * 1. Get frustum height from the baked confine space and amke sure it is a valid shape.
             * 2. Convert frustum height from world to baked space - makes it easier to do calculations if we are inside of it.
             */

            /* Step 3: get corrected GetCorrectedOrientation and do the calculation for confining the position */

            /* Step 4: Do the slowing distance check for when we are moving closer to the edge.
             * Check previous position and get the magnitude of the direction by (new position - previous pos).magnitude */

            /* Step 5: Cached the previous displacement for next frame. This is used for damping in multiple camera stages.
             * Not doing this can mess with other cinemachine extensions that calculate after the Body Stage. */

            /* Step 6: Set the corrected position and the previous camera position.*/
        }

        private bool IsConfined(in LensSettings settings, in Vector3 cameraOriginPos)
        {
            /* Camera.orthographicSize => Camera's half-size when in orthographic mode.
             * The orthographicSize property defines the viewing volume of an orthographic Camera.
             * To edit orthographicSize, you must set the Camera projection to orthographic.
             * The height of the viewing volume is (orthographicSize * 2). 
             * Unity calculates the width of the viewing volume using orthographicSize and the camera's aspect.
             */
            float frustumHalfHeight = CalculateFrustumHalfHeight(settings, cameraOriginPos.z);
            float frustumHalfWidth = frustumHalfHeight * settings.Aspect;
            
            // Starting with top right corner
            Vector2 topRightPos = new Vector2(cameraOriginPos.x + frustumHalfWidth, cameraOriginPos.y + frustumHalfHeight);
            Vector2 topLeftPos = new Vector2(cameraOriginPos.x - frustumHalfWidth, cameraOriginPos.y + frustumHalfHeight);
            Vector2 bottomRightPos = new Vector2(cameraOriginPos.x + frustumHalfWidth, cameraOriginPos.y - frustumHalfHeight);
            Vector2 bottomLeftPos = new Vector2(cameraOriginPos.x - frustumHalfWidth, cameraOriginPos.y - frustumHalfHeight);


            Debug.DrawLine(topRightPos, bottomRightPos);
            
            return Shape.OverlapPoint(topRightPos) 
                && Shape.OverlapPoint(topLeftPos) 
                && Shape.OverlapPoint(bottomRightPos) 
                && Shape.OverlapPoint(bottomLeftPos);
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
