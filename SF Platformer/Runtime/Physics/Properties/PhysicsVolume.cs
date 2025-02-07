using SF.Characters.Controllers;

using UnityEngine;

namespace SF.Physics
{
    public enum PhysicsVolumeType
    {
        None,
        Water,
        Gravity
    }

    /// <summary>
    /// A physics volume that updates the movement properties of characters that 
    /// enter it.
    /// </summary>
    public class PhysicsVolume : MonoBehaviour
    {
        [SerializeField] private MovementProperties _volumeProperties = new(Vector2.zero);
        [SerializeField] private PhysicsVolumeType _physicsVolumeType = PhysicsVolumeType.Water;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out Controller2D controller2D))
            {
                controller2D.UpdatePhysics(_volumeProperties, _physicsVolumeType);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out Controller2D controller2D))
            {
                controller2D.ResetPhysics(_volumeProperties);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.TryGetComponent(out Controller2D controller2D))
            {
                controller2D.UpdatePhysics(_volumeProperties, _physicsVolumeType);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if(collision.gameObject.TryGetComponent(out Controller2D controller2D))
            {
                controller2D.ResetPhysics(_volumeProperties);
            }
        }
    }
}
