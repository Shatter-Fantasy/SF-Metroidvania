using SF.CollectableModule;
using SF.PowerUpModule;
using UnityEngine;

namespace SF
{
    public class PowerUpCollectable : MonoBehaviour, ICollectable<PowerUpController>
    {
        [SerializeField] private FireBallPowerUp _powerUp;
        public void Collect(PowerUpController powerUpController)
        {
            _powerUp.ProjectileSpawn = powerUpController.transform;
            _powerUp.PowerUpProjectile.Init();
            powerUpController.CurrentPower = _powerUp;
            this.gameObject.SetActive(false);
        }
    }
}
