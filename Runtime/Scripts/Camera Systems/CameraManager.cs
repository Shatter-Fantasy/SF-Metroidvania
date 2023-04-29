using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SF.CameraSystems
{
	[RequireComponent(typeof(Camera), typeof(CinemachineBrain))]
	public class CameraManager : MonoBehaviour
	{
		private static CameraManager _instance;
		public static CameraManager Instance
		{
			get
			{
				if(_instance == null)
					_instance = FindFirstObjectByType<CameraManager>();

				return _instance;
			}
		}
		[field: SerializeField] public CinemachineBrain CMBrain { get; private set; }
		[field:Header("Game Cameras")]
		[field: SerializeField] public Camera MainCamera { get; private set; }
		private UniversalAdditionalCameraData _cameraData;
		[field: SerializeField] public List<Camera> OverlayCameras { get; private set; } = new();

		[field:Header("Cinemachine Cameras")]
		[field: SerializeField] public CinemachineCamera CurrentCamera { get; private set; }
		[field: SerializeField] public CinemachineCamera PlayerCamera { get; private set; }

		[SerializeField] private List<CinemachineCamera> CMCameras = new();

		[field:Header("Camera Manager Properties")]
		[field:SerializeField] public bool IsCameraLocked { get; private set; }
		private List<CinemachineCamera> UnactiveCMCameras = new();

		#region Starting Lifecycle Functions
		private void Awake()
		{
			if(Instance != null && Instance != this)
			{
				Destroy(this);
				return;
			}

			CMBrain = GetComponent<CinemachineBrain>();
			MainCamera = GetComponent<Camera>();
			if(MainCamera != null)
				_cameraData = MainCamera.GetUniversalAdditionalCameraData();
		}
		private void Start()
		{
			FindAllActiveCMCamera();
			AddOverlayCameras();
		}
		private void AddOverlayCameras()
		{
			if(OverlayCameras == null) return;

			OverlayCameras.ForEach(cam =>
			{
				if(!_cameraData.cameraStack.Contains(cam))
					_cameraData.cameraStack.Add(cam);
			});
		}
		private void FindAllActiveCMCamera()
		{
			if(CinemachineCore.VirtualCameraCount <= 0)
				Debug.LogError("There were no cinemachines found inside of the scene.");

			for(int x = 0; x < CinemachineCore.VirtualCameraCount; x++ )
			{
				CMCameras.Add(CinemachineCore.GetVirtualCamera(x) as CinemachineCamera);
			}

			CMCameras.ForEach( vCam =>
			{
				if(vCam is not null)
				{
					if(vCam.gameObject.tag == "Player")
						PlayerCamera = PlayerCamera ?? vCam;
				}
			});

			CurrentCamera = CurrentCamera ?? CMBrain.ActiveVirtualCamera as CinemachineCamera;
		}
		#endregion
		public void AddCMCamera(CinemachineCamera newCMCamera)
		{
			CMCameras.Add(newCMCamera);
		}
		public void ChangeActiveCMCamera(CinemachineCamera activeVCamera)
		{
			if(activeVCamera == CurrentCamera) return;

			if(activeVCamera == null)
			{
				Debug.LogError("The camera being passed into the ChangeActiveCamera function is null.", gameObject);
				return;
			}

			UnactiveCMCameras = CMCameras.Where(vCam => vCam  != activeVCamera).ToList();

			UnactiveCMCameras.ForEach(vCam =>
			{
				if(vCam.isActiveAndEnabled)
					vCam.Priority.Value = 1; 
			});

			CurrentCamera = activeVCamera;
			CurrentCamera.Priority.Value = 5;
		}
		public static List<Camera> GetCameraStack()
		{
			return Instance._cameraData.cameraStack;
		}
		/// <summary>
		/// Helper function to quickly change the camera to either the player follow camera
		/// or the player shoulder overhead camera. 
		/// Pass in nothing to use player follow camera and pass in 1 for over shoulder camera.
		/// </summary>
		/// <param name="cameraIndex"></param>
		public static void ActivePlayerCamera(int cameraIndex = 0)
		{
			if(cameraIndex == 0 && Instance.PlayerCamera != null)
				Instance.ChangeActiveCMCamera(Instance.PlayerCamera);
		}
		public static List<Camera> GetOverLayCameras()
		{
			return Instance.MainCamera.GetUniversalAdditionalCameraData().cameraStack.
				Where((cam=> cam.GetUniversalAdditionalCameraData().renderType == CameraRenderType.Overlay)).ToList();
		}
		/// <summary>
		/// Adds an overlay camera to the main camera stack.
		/// The camera being passed in has to be type of overlay or
		/// this function just returns and does nothing.
		/// </summary>
		/// <param name="camera"></param>
		public static void AddCameraToStack(Camera camera)
		{
			// Only add overlay cameras to the main camera stack.
			if(camera.GetUniversalAdditionalCameraData().renderType != CameraRenderType.Overlay)
				return;

			Instance._cameraData.cameraStack.Add(camera);
		}
		// Can probably fuse the ToggleCameraLock and the SetCameraLock function into one.
		public void ToggleCameraLock()
		{
			IsCameraLocked = !IsCameraLocked;
		}
	}
}