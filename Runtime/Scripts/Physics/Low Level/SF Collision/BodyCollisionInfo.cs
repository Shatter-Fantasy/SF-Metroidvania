using System;
using SF.Physics;

namespace SF.PhysicsLowLevel
{

    [System.Serializable]
    public class BodyCollisionInfo : CollisionInfoBase
    {
        [NonSerialized] public ControllerBody2D ControllerBody2D;

        private ContactExtensions.ContactFilterFunction _filterFunction;
        public override void SideCollisionChecks()
        {
            using var contacts = ControllerBody2D.PhysicsShape.GetContacts();
            
            if (contacts.Length > 0)
            {
                for (int i = 0; i < contacts.Length; i++)
                {
                    if (contacts[i].manifold.normal.x > 0.95f)
                        IsCollidingLeft = true;
                    
                    if (contacts[i].manifold.normal.x < -0.95f)
                        IsCollidingRight = true;
                }
            }
        }
        
        public override void GroundCollisionChecks()
        {
            using var contacts = ControllerBody2D.PhysicsShape.GetContacts();
            
            int groundHits = 0;
            if (contacts.Length > 0)
            {
                for (int i = 0; i < contacts.Length; i++)
                {
                    if (contacts[i].manifold.normal.y > 0.95f)
                    {
                        groundHits++;
                    }
                }
            }
            
            if (groundHits > 0)
            {
                IsGrounded = true;
            }
            else // If we are not colliding with anything below.
            {
                StandingOnObject = null;
                IsGrounded = false;
                
                /* TODO Moving Platform logic here.
                if(transform.parent != null)
                    transform.SetParent(null);
                */
            }
            
            // If not grounded last frame, but grounded this frame call OnGrounded
            if(!WasGroundedLastFrame && IsGrounded)
            {
                OnGroundedHandler?.Invoke();
            }
        }

        public override void CeilingChecks()
        {
            using var contacts = ControllerBody2D.PhysicsShape.GetContacts();

            if (contacts.Length > 0)
            {
                for (int i = 0; i < contacts.Length; i++)
                {
                    if (contacts[i].manifold.normal.y < -0.95f)
                    {
                        IsCollidingAbove = true;
                        return;
                    }
                }
            }

            IsCollidingAbove = false;
        }

        public void FilterContacts()
        {
            using var contacts = ControllerBody2D.PhysicsShape.GetContacts();

            if (contacts.Length == 0)
                return;
            
          
        }
    }
}
