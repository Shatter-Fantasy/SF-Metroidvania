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

            CameraController.Instance.CameraTarget = transform;

            if (GameManager.Instance != null && GameManager.Instance.PlayerController == null)
                GameManager.Instance.PlayerController = this;
        }
    }
}