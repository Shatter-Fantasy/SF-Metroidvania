using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.DamageModule
{
    using PhysicsLowLevel;
    
    [System.Flags]
    public enum Direction : short
    {
        Any = 0,
        Left = 1,
        Right = 2,
        Sides = 3,
        Up = 4,
        Down = 8,
    }

    public class Hazard : MonoBehaviour,
        ITriggerShapeCallback,
        IContactShapeCallback
    {
        private Vector2 _collisionNormal;
        public Direction DamageDirection;
        public int DamageAmount = 1;
        [SerializeField] private Vector2 _knockBackForce;
        
        private void Start()
        {
            if (TryGetComponent(out SFShapeComponent component))
            {
                component.AddTriggerCallbackTarget(this);
            }
        }
        
        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            var visitingComponent = beginEvent.GetCallbackComponentOnVisitor<SFShapeComponent>();
            
            if (!visitingComponent.TryGetComponent(out IDamagable damagable))
                return;
            
            damagable.TakeDamage(DamageAmount,_knockBackForce);
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent, SFShapeComponent callingShapeComponent)
        {
            // noop - No Operation.
        }

        
        // TODO: Add this to an utility class where the direction is just passed in as a parameter.
        private bool CheckCollisionDirection()
        {
            switch(DamageDirection)
            {
                case Direction.Any:
                    return true;
                case Direction.Left:
                    if(_collisionNormal.x < 0) 
                        return true;
                    break;
                case Direction.Right:
                    if(_collisionNormal.x > 0)
                        return true;
                    break;
                case Direction.Sides:
                    if(_collisionNormal.x < 0 || _collisionNormal.x > 0)
                        return true;
                    break;
                case Direction.Up:
                    if(_collisionNormal.y > 0)
                        return true;
                    break;
                case Direction.Down:
                    if(_collisionNormal.y < 0)
                        return true;
                    break;
            }

            return false;

        }
        public void OnContactBegin2D(PhysicsEvents.ContactBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            // TODO: Important: Update the LowLevelPhysicsUtilities with a method that matches the TriggerBeginEvent, but for ContactBeginEvent.
            /*
             var visitingComponent = beginEvent.GetCallbackComponentOnVisitor<SFShapeComponent>();
            
            if (!visitingComponent.TryGetComponent(out IDamagable damagable))
                return;
            
            damagable.TakeDamage(DamageAmount,_knockBackForce);
            */
        }

        public void OnContactEnd2D(PhysicsEvents.ContactEndEvent endEvent, SFShapeComponent callingShapeComponent)
        {
            
        }
    }
}
