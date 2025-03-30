using Unity.Cinemachine;

using SF.CameraModule;
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
        }
    }
    
}