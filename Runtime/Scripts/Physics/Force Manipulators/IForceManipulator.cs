using UnityEngine;

namespace SF.Physics
{
    public interface IForceManipulator
    {
        public void ExtertForce(IForceReciever forceReciever, Vector2 force);
    }
}
