using Unity.Cinemachine;
using SF.CameraModule;
using SF.Managers;
using SF.Utilities;


namespace SF.Characters.Controllers
{
	public class PlayerController : GroundedController2D
    {
        protected override void OnAwake()
        {
            base.OnAwake();

            if(gameObject.TryGetComponentInChildren(out CinemachineCamera cinemachineCamera))
                CameraController.SetPlayerCMCamera(cinemachineCamera);

            if (GameManager.Instance != null && GameManager.Instance.PlayerController == null)
                GameManager.Instance.PlayerController = this;
        }
    }
}