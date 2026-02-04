using Unity.Burst;
using Unity.Cinemachine;
using UnityEngine;

namespace SF.RoomModule
{
    [AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Rectangle Confiner 2D ")]
    [SaveDuringPlay]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [BurstCompile]
    public class CinemachineRectangleConfiner2D : CinemachineExtension
    {
        [SerializeField] private Bounds _confinerBounds;
        [SerializeField] private Transform _confinerCenter;

        private Vector3 _correctedPosition;
        
        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage != CinemachineCore.Stage.Body)
                return;

            if (_confinerCenter == null)
                return;
            
            var camPosition = vcam.transform.position;
            var settings = state.Lens;
            float frustumHalfHeight = CalculateFrustumHalfHeight(settings.OrthographicSize,camPosition.z,settings.FieldOfView);
            float frustumHalfWidth = frustumHalfHeight * settings.Aspect;
            Vector2          frustum          = new Vector2(frustumHalfWidth * 2, frustumHalfHeight * 2);
            
            _confinerBounds.center = _confinerCenter.transform.position;
            Vector3 pos = vcam.transform.position;

            _correctedPosition.x = Mathf.Max(_confinerBounds.min.x + frustumHalfWidth,Mathf.Min(_confinerBounds.max.x - frustumHalfWidth, pos.x) );
            _correctedPosition.y = Mathf.Max(_confinerBounds.min.y + frustumHalfHeight,Mathf.Min(_confinerBounds.max.y - frustumHalfHeight, pos.y) );
            _correctedPosition.z = state.RawPosition.z;

            state.PositionCorrection += _correctedPosition - state.GetCorrectedPosition();
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

        public void UpdateConfinerBounds(in Bounds newConfinerBounds)
        {
            // TODO: Add logic for making sure the bounds don't have invalid values like negative sizes or 0.
            _confinerBounds = newConfinerBounds;
        }
    }
}
