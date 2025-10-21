using System;
using SF.LevelModule;
using SF.Managers;
using Unity.Cinemachine;

using UnityEngine;

namespace SF.CameraModule
{
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
                    _instance = Camera.main.gameObject.AddComponent<CameraController>();

                return _instance;
            }
            set { _instance = value; }
        }
        private static CameraController _instance;

        public Transform CameraTarget;
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
        }

        public void Start()
        {
            if (GameManager.Instance != null && LevelPlayData.Instance.spawnedPlayerController != null)
            {
                _instance.CameraTarget = LevelPlayData.Instance.spawnedPlayerController.transform;
            }
        }

        public static void SwitchPlayerCMCamera(CinemachineCamera cmCamera, int priority = ActivePriority)
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

        
        public static void ChangeCameraConfiner(CinemachineCamera cmCamera)
        {
            if(ActiveRoomCamera != null)
            {
                cmCamera.Prioritize();
                ActiveRoomCamera = cmCamera;
            }
        }
        
        /* There is a bug in here on Unity's side confirmed by devs in Cinemachine 3.1.3 and other Cinemachine 3.x.x versions.
        // See link for devs talking about it being fixed in 3.1.4
        // Bounding shapes are not being updated properly.
        // https://discussions.unity.com/t/cinemachine-confiner2d-not-respected-in-v3-1-3/1607171/3
        // https://discussions.unity.com/t/latest-cinemachine-3-1-3-not-handling-confiners-as-well-as-previous-versions-2-10-3/1631522/4
        public static void ChangeCameraConfiner(Collider2D collider2D)
        {
            if (Instance.CameraConfiner?.BoundingShape2D == collider2D)
                return;
            if(Instance.ActiveRoomCamera != null)
            {
                if (Instance.CameraConfiner == null)
                    Instance.CameraConfiner = Instance.ActiveRoomCamera.GetComponent<CinemachineConfiner2D>();
                
                Instance.CameraConfiner.InvalidateBoundingShapeCache();
                Instance.CameraConfiner.BoundingShape2D = collider2D;
                Instance.CameraConfiner.BakeBoundingShape(Instance.ActiveRoomCamera, 1);
            }
        }
        */
    }
}
