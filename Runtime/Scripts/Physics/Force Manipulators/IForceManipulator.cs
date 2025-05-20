using UnityEngine;

namespace SF
{
    public interface IForceManipulator
    {
        public void ExtertForce(IForceReciever forceReciever, Vector2 force);
    }
}
