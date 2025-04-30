using Unity.Cinemachine;

using UnityEngine;

namespace SF.CameraModule
{
    public class CameraController : MonoBehaviour
    {
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
        public CinemachineCamera PlayerCamera;
        public CinemachineConfiner2D CameraConfiner;

        private void Awake()
        {
            if(Instance != null && _instance  != this)
                Destroy(this);

            Instance = this;
        }

        public static void SetPlayerCMCamera(CinemachineCamera cmCamera)
        {
            if(cmCamera == null)
                return;
                
            Instance.PlayerCamera = cmCamera;
            Instance.PlayerCamera.Follow = Instance.CameraTarget;
            Instance.CameraConfiner = Instance.PlayerCamera.GetComponent<CinemachineConfiner2D>();
        }

        public static void ChangeCameraConfiner(CinemachineCamera cmCamera)
        {
            if(Instance.PlayerCamera != null)
            {
                cmCamera.Prioritize();
                Instance.PlayerCamera = cmCamera;
            }
        }

        // There is a bug in here on Unity's side confirmed by devs in Cinemachine 3.1.3 and other Cinemachine 3.x.x versions.
        // See link for devs talking about it being fixed in 3.1.4
        // Bounding shapes are not being updated properly.
        // https://discussions.unity.com/t/cinemachine-confiner2d-not-respected-in-v3-1-3/1607171/3
        // https://discussions.unity.com/t/latest-cinemachine-3-1-3-not-handling-confiners-as-well-as-previous-versions-2-10-3/1631522/4
        public static void ChangeCameraConfiner(Collider2D collider2D)
        {
            if (Instance.CameraConfiner?.BoundingShape2D == collider2D)
                return;
            if(Instance.PlayerCamera != null)
            {
                if (Instance.CameraConfiner == null)
                    Instance.CameraConfiner = Instance.PlayerCamera.GetComponent<CinemachineConfiner2D>();
                
                Instance.CameraConfiner.InvalidateBoundingShapeCache();
                Instance.CameraConfiner.BoundingShape2D = collider2D;
                Instance.CameraConfiner.BakeBoundingShape(Instance.PlayerCamera, 1);
            }
        }
    }
}
