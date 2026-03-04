using Unity.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.Interactables
{
    using PhysicsLowLevel;
    /// <summary>
    /// Base class for allowing stuff to interact with other objects.
    /// This can be implemented to allow NPC and enemies to interact with objects.
    /// </summary>
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] protected PhysicsQuery.QueryFilter _interactableFilter;
        [SerializeField] protected SFShapeComponent _hitShape;
        [SerializeField] protected PhysicsQuery.CastShapeInput _castInput;
        protected NativeArray<PhysicsShape> _hitShapes;
    }
}
