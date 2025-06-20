using SF.CameraModule;
using Unity.Cinemachine;
using UnityEngine;

namespace SF.CommandModule
{
    public enum CinemachineCommandTypes : ushort
    {
        SwitchPlayerCamera = 0,
        ChangePriority = 1,
        ActivateCutsceneCamera = 2,
    }
    
    [System.Serializable]
    [CommandMenu("Camera/Cinemachine Priority")]
    public class CinemachineCameraCommand : CommandNode, ICommand
    {
        [SerializeField] private int _priority = -1;
        [SerializeField] private CinemachineCamera _cinemachineCamera;
        [SerializeField] private CinemachineCommandTypes _cinemachineCommandType;
        protected override bool CanDoCommand()
        {
            return _cinemachineCamera != null;
        }

        protected override void DoCommand()
        {
            switch (_cinemachineCommandType)
            {
                case CinemachineCommandTypes.SwitchPlayerCamera:
                {
                    CameraController.SwitchPlayerCMCamera(_cinemachineCamera, _priority);
                    break;
                }
                case CinemachineCommandTypes.ChangePriority:
                {
                    _cinemachineCamera.Priority = _priority;
                    break;
                }
                case CinemachineCommandTypes.ActivateCutsceneCamera:
                {
                   CameraController.ActivateCutsceneCMCamera(_cinemachineCamera);
                    break;
                }
            }
        }

        protected override async Awaitable DoAsyncCommand()
        {
            switch (_cinemachineCommandType)
            {
                case CinemachineCommandTypes.SwitchPlayerCamera:
                {
                    CameraController.SwitchPlayerCMCamera(_cinemachineCamera, _priority);
                    break;
                }
                case CinemachineCommandTypes.ChangePriority:
                {
                    _cinemachineCamera.Priority = _priority;
                    break;
                }
                case CinemachineCommandTypes.ActivateCutsceneCamera:
                {
                    CameraController.ActivateCutsceneCMCamera(_cinemachineCamera);
                    break;
                }
            }
            
            await Awaitable.MainThreadAsync();
        }
    }
}
