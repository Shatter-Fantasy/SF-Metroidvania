using Unity.Cinemachine;
using UnityEngine;

using SF.SpawnModule;

namespace SF.CameraModule
{
    /// <summary>
    /// The manager for the active main camera in playable levels.
    /// Contains helper methods for switching active cameras.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        /// <summary>
        /// This is the default priority that is set on the old virtual cameras that are being switched away from.
        /// </summary>
        public const int DeactivatedPriority = -1;

        /// <summary>
        /// This is the virtual camera priority value for the currently active player camera.
        /// </summary>
        public const int ActivePriority = 1;
        
        /// <summary>
        /// This is the virtual camera priority value for the cutscene virtual cameras when a cutscene is playing requiring camera overriding.
        /// </summary>
        public const int CutsceneCameraPriority = 6;

        /// <summary>
        /// How far away the virtual cameras camera is set 
        /// </summary>
        public const int CameraDistance = 10;
        public static CameraController Instance
        {
            get 
            {
                if (_instance == null)
                    _instance = FindFirstObjectByType<CameraController>();

                if(_instance == null)
                    _instance = Camera.main?.gameObject.AddComponent<CameraController>();

                return _instance;
            }
            set { _instance = value; }
        }
        private static CameraController _instance;

        public Transform CameraTarget;

        public static Camera MainCamera;
        public static CinemachineBrain MainCameraBrain;
        public static CinemachineCamera ActiveRoomCamera;
        public static CinemachineCamera ActiveCutsceneCamera;

        private void Awake()
        {
            if(Instance != null && _instance  != this)
                Destroy(this);
            else // done in an else statement for times when the component is not destroyed instantly and continues into the Awake call.
            {
                Instance = this;
            }

            MainCamera = GetComponent<Camera>();
            if (MainCamera != null)
                MainCamera.TryGetComponent(out MainCameraBrain);
           
            SpawnSystem.InitialPlayerSpawnHandler += SetInitialCameraTarget;
        }
        
        private void OnDestroy()
        { 
            _instance = null;

            CameraTarget = null;
            MainCamera = null;
            MainCameraBrain = null;
            ActiveRoomCamera = null;
            ActiveCutsceneCamera = null;
            
            SpawnSystem.InitialPlayerSpawnHandler -= SetInitialCameraTarget;
        }
        
        /// <summary>
        /// Set's the <see cref="CameraTarget"/> of the CameraManager <see cref="Instance"/>.
        /// </summary>
        /// <param name="spawnedPlayer"></param>
        private void SetInitialCameraTarget(GameObject spawnedPlayer)
        {
            _instance.CameraTarget = SpawnSystem.SpawnedPlayer.transform;
            
            if(MainCameraBrain != null 
               && MainCameraBrain.ActiveVirtualCamera as CinemachineCamera != null
               && _instance.CameraTarget != null)
                SwitchPlayerCMCamera(MainCameraBrain.ActiveVirtualCamera as CinemachineCamera);
        }
        
        /// <summary>
        /// Switches between the current <see cref="ActiveRoomCamera"/> and makes a new room camera the <see cref="ActiveRoomCamera"/>.
        /// </summary>
        /// <param name="cmCamera"></param>
        /// <param name="priority"></param>
        public static void SwitchPlayerCMCamera(CinemachineCamera cmCamera, int priority = ActivePriority)
        {
            if(cmCamera == null)
                return;
            
            // If the Virtual Camera has a CinemachinePositionComposer on it set it's distance to our set default.
            if (cmCamera.TryGetComponent(out CinemachinePositionComposer positionComposer))
                positionComposer.CameraDistance = CameraDistance;

            if (ActiveRoomCamera != null)
            {
                // Reset the previous/old virtual camera priority.
                // At this point Instance.ActiveRoomCamera is still the old camera.
                // We also clear the old camera follow to prevent it from following the player while not the active camera.
                ActiveRoomCamera.Follow = null;
                ActiveRoomCamera.Priority = DeactivatedPriority;
            }
            
            ActiveRoomCamera = cmCamera;        
            
            // From here Instance.ActiveRoomCamera is the new camera.
            if(Instance.CameraTarget != null)
                ActiveRoomCamera.transform.position = Instance.CameraTarget.position;
            
            ActiveRoomCamera.Priority = ActivePriority;
            
            // We don't add setting the ActiveRoomCamera.Follow in the null check above for when we need to do cutscenes and not have a follow target
            ActiveRoomCamera.Follow = Instance.CameraTarget;  
            ActiveRoomCamera.Target.TrackingTarget = Instance.CameraTarget;  
            ActiveRoomCamera.Target.LookAtTarget = Instance.CameraTarget;  
        }
        public static void ActivateCutsceneCMCamera(CinemachineCamera cmCamera)
        {
            if(cmCamera == null)
                return;

            /* Not an error if this check is null: This is an expected result in some cases.
                This can happen when loading the first room in an area,
                 loading a game file into a save room, or when doing certain types of RoomTransitions from scene to scene.
            */

            // If the Virtual Camera has a CinemachinePositionComposer on it set it's distance to our set default.
            if (cmCamera.TryGetComponent(out CinemachinePositionComposer positionComposer))
                positionComposer.CameraDistance = CameraDistance;

            if (ActiveCutsceneCamera != null)
            {
                // Reset the previous/old virtual camera priority.
                // At this point Instance.ActiveCutsceneCamera is still the old camera.
                // We also clear the old camera follow to prevent it from following the player while not the active camera.
                ActiveCutsceneCamera.Follow = null;
                ActiveCutsceneCamera.Priority = DeactivatedPriority;
            }
            
            ActiveCutsceneCamera = cmCamera;             
            // From here Instance.ActiveCutsceneCamera is the new camera.
            ActiveCutsceneCamera.Priority = CutsceneCameraPriority;
        }
        public static void SetCameraFollow(CinemachineCamera camera,Transform target)
        {
            if (camera == null || target == null)
                return;

            camera.Follow = target;
        }
    }
}
