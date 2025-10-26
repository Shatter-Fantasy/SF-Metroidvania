using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SF.Physics;
using Unity.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.PhysicsLowLevel
{

    [System.Serializable]
    public class BodyCollisionInfo : CollisionInfoBase
    {
        [NonSerialized] public ControllerBody2D ControllerBody2D;

        private ContactExtensions.ContactNormalFilterFunction _filterFunction;
        private NativeArray<PhysicsShape.Contact> _contacts;
        
        public override void CheckCollisions()
        {
            WasCollidingLeft = IsCollidingLeft;
            WasCollidingRight = IsCollidingRight;
            WasCollidingAbove = IsCollidingAbove;
            WasCollidingBelow = IsGrounded;
			
            WasGroundedLastFrame = IsGrounded;

            using (_contacts = ControllerBody2D.PhysicsShape.GetContacts())
            {
                GroundCollisionChecks();
                CeilingChecks();
                SideCollisionChecks();
                CheckOnCollisionActions();
            }
            
            // Since we are keeping track of the contact in the collision check functions we have to dispose manually for memory safety.
            _contacts.Dispose();
        }
        
        public override void GroundCollisionChecks()
        {
            if (_contacts.Length == 0)
            {
                IsGrounded = false;
                IsCollidingBelow = false;
                return;
            }
            
            // Normal hits are within the context of shapeA to shapeB  = below hit normals return positive 1
            var filteredContacts = _contacts.Filter(
                ContactFiltering.NormalYFilter, 
                ControllerBody2D.PhysicsShape, 
                0,
                FilterMathOperator.GreaterThan);

            if (filteredContacts.ToList().Count > 0)
            {
                IsGrounded = true;
                IsCollidingBelow = true;
            }
            else // If we are not colliding with anything below.
            {
                StandingOnObject = null;
                IsGrounded = false;
                IsCollidingBelow = false;

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
        
        public override void SideCollisionChecks()
        {
            if (_contacts.Length == 0)
            {
                IsCollidingRight = false;
                IsCollidingLeft = false;
                return;
            }
            
            // Left Collision Check
            var filteredContacts = _contacts.Filter(
                ContactFiltering.NormalXFilter, 
                ControllerBody2D.PhysicsShape, 
                0,
                FilterMathOperator.GreaterThan);
            
            IsCollidingLeft = (filteredContacts.ToList().Count > 0);
            
            
            // Right Collision Check
            filteredContacts = _contacts.Filter(
                ContactFiltering.NormalXFilter, 
                ControllerBody2D.PhysicsShape, 
                0,
                FilterMathOperator.LessThan);
            
            IsCollidingRight = (filteredContacts.ToList().Count > 0);
        }
        
        
        public override void CeilingChecks()
        {
            if (_contacts.Length == 0)
            {
                IsCollidingAbove = false;
                return;
            }
            
            var filteredContacts = _contacts.Filter(
                ContactFiltering.NormalYFilter, 
                ControllerBody2D.PhysicsShape, 
                0,
                FilterMathOperator.LessThan);

            IsCollidingAbove = (filteredContacts.ToList().Count > 0);;
        }
    }
}
