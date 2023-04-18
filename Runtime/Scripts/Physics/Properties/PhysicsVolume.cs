using UnityEngine;

namespace SF.Physics
{
    /// <summary>
    /// A physics volume that updates the movement properties of characters that 
    /// enter it.
    /// </summary>
    public class PhysicsVolume
    {
        [SerializeField] private MovementProperties VolumeProperties;
    }
}
