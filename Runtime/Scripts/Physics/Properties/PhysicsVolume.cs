using UnityEngine;

namespace SF.PhysicsLowLevel
{
    /// <summary>
    /// The type of physics volume or zone that is manipulating the surrounding characters and objects.
    /// <remarks>
    /// Physics volumes can also effect stuff other than character controllers.
    /// </remarks>
    /// </summary>
    public enum PhysicsVolumeType
    {
        None,
        Water,
        Gravity
    }
    
    /// <summary>
    /// A physics volume that updates the movement properties of characters that 
    /// enter it.
    /// Look at <see cref="SF.AbilityModule.Characters.GlideAbility"/> for a working example of how to simulate a lower gravty effect.
    /// </summary>
    public class PhysicsVolume : MonoBehaviour
    {
        [SerializeField] private MovementProperties _volumeProperties = new(Vector2.zero);
        [SerializeField] private PhysicsVolumeType _physicsVolumeType = PhysicsVolumeType.Water;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out ControllerBody2D controller2D))
            {
                controller2D.UpdatePhysicsProperties(_volumeProperties, _physicsVolumeType);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out ControllerBody2D controller2D))
            {
                controller2D.ResetPhysics(_volumeProperties);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.TryGetComponent(out ControllerBody2D controller2D))
            {
                controller2D.UpdatePhysicsProperties(_volumeProperties, _physicsVolumeType);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if(collision.gameObject.TryGetComponent(out ControllerBody2D controller2D))
            {
                controller2D.ResetPhysics(_volumeProperties);
            }
        }
    }
}
