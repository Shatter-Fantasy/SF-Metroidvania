using SF.InputModule;
using SF.PowerUpModule;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SF
{
    public class PowerUpController : MonoBehaviour
    {
        [SerializeReference] public IPowerUp CurrentPower;
        private void Start()
        {
            CurrentPower = new FireBallPowerUp(transform);
        }
        private void OnEnable()
        {
            InputManager.Controls.Player.Enable();
            //InputManager.Controls.Player.PowerUp.performed += OnPowerUp;
        }   

        private void OnPowerUp(InputAction.CallbackContext context)
        {
            CurrentPower.Activate();
        }
    }
}
